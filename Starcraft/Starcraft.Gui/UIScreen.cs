using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;

namespace Starcraft
{
	public abstract class UIScreen
	{
		protected Surface Background;
		protected CursorAnimator Cursor;
		protected UIPainter UIPainter;
		protected Bin Bin;
		protected Mpq mpq;
		protected string prefix;
		protected string binFile;

		protected string background_path;
		protected string fontpal_path;
		protected string effectpal_path;
		protected string arrowgrp_path;

		protected List<UIElement> Elements;

		protected UIScreen (Mpq mpq, string prefix, string binFile)
		{
			this.mpq = mpq;
			this.prefix = prefix; 
			this.binFile = binFile;

			background_path = prefix + "\\Backgnd.pcx";
			fontpal_path = prefix + "\\tFont.pcx";
			effectpal_path = prefix + "\\tEffect.pcx";
			arrowgrp_path = prefix + "\\arrow.grp";
		}

		protected UIScreen (Mpq mpq)
		{
			this.mpq = mpq;
		}

		public virtual void SwooshIn ()
		{
			try {
				Console.WriteLine ("swooshing in");
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (RaiseDoneSwooshing)));
			}
			catch (Exception e) {
				Console.WriteLine ("failed pushing UIScreen.RiseDoneSwooshing: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		public virtual void SwooshOut ()
		{
			try {
				Console.WriteLine ("swooshing out");
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (RaiseDoneSwooshing)));
			}
			catch (Exception e) {
				Console.WriteLine ("failed pushing UIScreen.RiseDoneSwooshing: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		public virtual void AddToPainter (Painter painter)
		{
			if (Background != null)
				painter.Add (Layer.Background, BackgroundPainter);

			if (UIPainter != null)
				painter.Add (Layer.UI, UIPainter.Paint);

			if (Cursor != null)
				Game.Instance.Cursor = Cursor;
		}

		public virtual void RemoveFromPainter (Painter painter)
		{
			if (Background != null)
				painter.Remove (Layer.Background, BackgroundPainter);
			if (UIPainter != null)
				painter.Remove (Layer.UI, UIPainter.Paint);
			if (Cursor != null)
				Game.Instance.Cursor = null;
		}

		UIElement XYToElement (int x, int y, bool onlyUI)
		{
			if (Elements == null)
				return null;

			foreach (UIElement e in Elements) {
				if (e.Type == ElementType.DialogBox)
					continue;

				if (onlyUI &&
				    e.Type == ElementType.Image)
					continue;

				if (x >= e.X1 && x < e.X1 + e.Width &&
				    y >= e.Y1 && y < e.Y1 + e.Height)
					return e;
			}
			return null;
		}

		UIElement mouseDownElement;
		public virtual void MouseButtonDown (MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.PrimaryButton)
				return;

			mouseDownElement = XYToElement (args.X, args.Y, true);
			Console.WriteLine ("mouseDownElement = {0}", mouseDownElement);
		}

		public virtual void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.PrimaryButton)
				return;

			UIElement mouseUpElement = XYToElement (args.X, args.Y, true);

			Console.WriteLine ("mouseUpElement = {0}", mouseUpElement);

			if (mouseUpElement != null) {
				if (mouseUpElement == mouseDownElement)
					ActivateElement (mouseUpElement);
				else {
					mouseDownElement = null;
					return;
				}
			}

		}

		UIElement mouseOverElement;
		public virtual void PointerMotion (MouseMotionEventArgs args)
		{
			UIElement newMouseOverElement = XYToElement (args.X, args.Y, true);

			if (newMouseOverElement != null
			    && newMouseOverElement != mouseOverElement)
				MouseOverElement (newMouseOverElement);

			mouseOverElement = newMouseOverElement;
		}

		public virtual void MouseOverElement (UIElement element)
		{
			if (element.Type == ElementType.Button
			    || element.Type == ElementType.ButtonWithoutBorder
			    || element.Type == ElementType.DefaultButton) {

				if ((element.Flags & ElementFlags.RespondToMouse) == ElementFlags.RespondToMouse) {
					/* highlight the text */
					GuiUtil.PlaySound (mpq, Builtins.MouseoverWav);
				}
			}
		}

		public virtual void ActivateElement (UIElement element)
		{
			element.OnActivate ();
		}

		public virtual void KeyboardUp (KeyboardEventArgs args)
		{
		}

		public virtual void KeyboardDown (KeyboardEventArgs args)
		{
			foreach (UIElement e in Elements) {
				if ( (args.Key == e.Hotkey)
				     ||
				     (args.Key == Key.Return
				      && (e.Flags & ElementFlags.DefaultButton) == ElementFlags.DefaultButton)
				     ||
				     (args.Key == Key.Escape
				      && (e.Flags & ElementFlags.CancelButton) == ElementFlags.CancelButton)) {
					ActivateElement (e);
					return;
				}
			}
		}

		protected virtual void ScreenDisplayed ()
		{
		}

		public event ReadyDelegate DoneSwooshing;
		public event ReadyDelegate Ready;

		bool loaded;

		protected void RaiseReadyEvent ()
		{
			if (Ready != null)
				Ready ();
		}

		protected void RaiseDoneSwooshing ()
		{
			if (DoneSwooshing != null)
				DoneSwooshing ();
		}

		protected void BackgroundPainter (Surface surf, DateTime dt)
		{
			surf.Blit (Background);
		}

		protected virtual void ResourceLoader ()
		{
			Stream s;
			Pcx fontpal = null;
			Pcx effectpal = null;

			Console.WriteLine ("loading font palette");
			s = (Stream)mpq.GetResource (fontpal_path);
			if (s != null) {
				fontpal = new Pcx ();
				fontpal.ReadFromStream (s, false);
			}
			Console.WriteLine ("loading cursor palette");
			s = (Stream)mpq.GetResource (effectpal_path);
			if (s != null) {
				effectpal = new Pcx ();
				effectpal.ReadFromStream (s, false);
			}
			if (effectpal != null) {
				Console.WriteLine ("loading arrow cursor");
				Grp arrowgrp = (Grp)mpq.GetResource (arrowgrp_path);
				if (arrowgrp != null) {
					Cursor = new CursorAnimator (arrowgrp, effectpal.Palette);
					Cursor.SetHotSpot (64, 64);
				}
			}

			Console.WriteLine ("loading main menu background");
			Background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (background_path));

			Console.WriteLine ("loading main menu ui elements");
			Bin = (Bin)mpq.GetResource (binFile);

			if (Bin != null) {
				/* convert all the BinElements to UIElements for our subclasses to use */
				Elements = new List<UIElement> ();
				foreach (BinElement el in Bin.Elements)
					Elements.Add (new UIElement (mpq, el, fontpal.RgbData));
				UIPainter = new UIPainter (Elements);
			}
		}

		public void Load ()
		{
			if (loaded)
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (RaiseReadyEvent)));
			else
#if MULTI_THREADED
				ThreadPool.QueueUserWorkItem (delegate (object state) { ResourceLoader (); });
#else
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (ResourceLoader)));
#endif
		}

		protected virtual void FinishedLoading ()
		{
			loaded = true;
			RaiseReadyEvent ();
		}
	}

}
