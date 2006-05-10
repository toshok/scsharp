//
// SCSharp.UI.UIScreen
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;

namespace SCSharp.UI
{
	public abstract class UIScreen
	{
		Surface background;
		protected CursorAnimator Cursor;
		protected UIPainter UIPainter;
		protected Bin Bin;
		protected Mpq mpq;
		protected string prefix;
		protected string binFile;

		protected string background_path;
		protected int background_transparent;
		protected int background_translucent;
		protected string fontpal_path;
		protected string effectpal_path;
		protected string arrowgrp_path;

		protected Pcx fontpal;
		protected Pcx effectpal;

		protected List<UIElement> Elements;

		protected Painter painter;
		protected UIDialog dialog; /* the currently popped up dialog */

		protected UIScreen (Mpq mpq, string prefix, string binFile)
		{
			this.mpq = mpq;
			this.prefix = prefix; 
			this.binFile = binFile;

			if (prefix != null) {
				background_path = prefix + "\\Backgnd.pcx";
				fontpal_path = prefix + "\\tFont.pcx";
				effectpal_path = prefix + "\\tEffect.pcx";
				arrowgrp_path = prefix + "\\arrow.grp";
			}

			background_transparent = 0;
			background_translucent = 254;
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
			this.painter = painter;

			if (background != null)
				painter.Add (Layer.Background, BackgroundPainter);

			if (UIPainter != null)
				painter.Add (Layer.UI, UIPainter.Paint);

			if (Cursor != null)
				Game.Instance.Cursor = Cursor;
		}

		public virtual void RemoveFromPainter (Painter painter)
		{
			if (background != null)
				painter.Remove (Layer.Background, BackgroundPainter);
			if (UIPainter != null)
				painter.Remove (Layer.UI, UIPainter.Paint);
			if (Cursor != null)
				Game.Instance.Cursor = null;

			this.painter = null;
		}

		public virtual bool UseTiles {
			get { return false; }
		}

		public virtual Painter Painter {
			get { return painter; }
		}

		public Mpq Mpq {
			get { return mpq; }
		}

		public Surface Background {
			get { return background; }
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

				if (e.Visible && e.PointInside (x, y))
					return e;
			}
			return null;
		}

		UIElement mouseDownElement;
		UIElement mouseOverElement;
		public virtual void MouseEnterElement (UIElement element)
		{
			element.MouseEnter ();
		}

		public virtual void MouseLeaveElement (UIElement element)
		{
			element.MouseLeave ();
		}

		public virtual void ActivateElement (UIElement element)
		{
			if (!element.Visible || !element.Sensitive)
				return;

			Console.WriteLine ("activating element {0}", Elements.IndexOf (element));
			element.OnActivate ();
		}

		// SDL Event handling
		public virtual void MouseButtonDown (MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.PrimaryButton &&
			    args.Button != MouseButton.WheelUp &&
			    args.Button != MouseButton.WheelDown)
				return;

			if (mouseDownElement != null)
				Console.WriteLine ("mouseDownElement already set in MouseButtonDown");

			UIElement element = XYToElement (args.X, args.Y, true);
			if (element != null && element.Visible && element.Sensitive) {
				mouseDownElement = element;
				if (args.Button == MouseButton.PrimaryButton)
					mouseDownElement.MouseButtonDown (args);
				else
					mouseDownElement.MouseWheel (args);
			}
		}

		public void HandleMouseButtonDown (MouseButtonEventArgs args)
		{
			if (dialog != null)
				dialog.HandleMouseButtonDown (args);
			else
				MouseButtonDown (args);
		}

		public virtual void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.PrimaryButton &&
			    args.Button != MouseButton.WheelUp &&
			    args.Button != MouseButton.WheelDown)
				return;

			if (mouseDownElement != null) {
				if (args.Button == MouseButton.PrimaryButton)
					mouseDownElement.MouseButtonUp (args);

				mouseDownElement = null;
			}
		}

		public void HandleMouseButtonUp (MouseButtonEventArgs args)
		{
			if (dialog != null)
				dialog.HandleMouseButtonUp (args);
			else
				MouseButtonUp (args);
		}

		public virtual void PointerMotion (MouseMotionEventArgs args)
		{
			if (mouseDownElement != null) {
				mouseDownElement.PointerMotion (args);
			}
			else {
				UIElement newMouseOverElement = XYToElement (args.X, args.Y, true);

				if (newMouseOverElement != mouseOverElement) {
					if (mouseOverElement != null)
						MouseLeaveElement (mouseOverElement);
					if (newMouseOverElement != null)
						MouseEnterElement (newMouseOverElement);
				}

				mouseOverElement = newMouseOverElement;
			}
		}

		public void HandlePointerMotion (MouseMotionEventArgs args)
		{
			if (dialog != null)
				dialog.HandlePointerMotion (args);
			else
				PointerMotion (args);
		}

		public virtual void KeyboardUp (KeyboardEventArgs args)
		{
		}

		public void HandleKeyboardUp (KeyboardEventArgs args)
		{
			/* just return if the modifier keys are released */
			if (args.Key >= Key.NumLock && args.Key <= Key.Compose)
				return;

			if (dialog != null)
				dialog.HandleKeyboardUp (args);
			else
				KeyboardUp (args);
		}

		public virtual void KeyboardDown (KeyboardEventArgs args)
		{
			if (Elements != null) {
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
		}

		public void HandleKeyboardDown (KeyboardEventArgs args)
		{
			/* just return if the modifier keys are pressed */
			if (args.Key >= Key.NumLock && args.Key <= Key.Compose)
				return;
				
			if (dialog != null)
				dialog.HandleKeyboardDown (args);
			else
				KeyboardDown (args);
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
			surf.Blit (background,
				   new Point ((surf.Width - background.Width) / 2,
					      (surf.Height - background.Height) / 2));

		}

		protected virtual void ResourceLoader ()
		{
			Stream s;

			fontpal = null;
			effectpal = null;

			if (fontpal_path != null) {
				Console.WriteLine ("loading font palette");
				s = (Stream)mpq.GetResource (fontpal_path);
				if (s != null) {
					fontpal = new Pcx ();
					fontpal.ReadFromStream (s, -1, -1);
				}
			}
			if (effectpal_path != null) {
				Console.WriteLine ("loading cursor palette");
				s = (Stream)mpq.GetResource (effectpal_path);
				if (s != null) {
					effectpal = new Pcx ();
					effectpal.ReadFromStream (s, -1, -1);
				}
				if (effectpal != null && arrowgrp_path != null) {
					Console.WriteLine ("loading arrow cursor");
					Grp arrowgrp = (Grp)mpq.GetResource (arrowgrp_path);
					if (arrowgrp != null) {
						Cursor = new CursorAnimator (arrowgrp, effectpal.Palette);
						Cursor.SetHotSpot (64, 64);
					}
				}
			}

			if (background_path != null) {
				Console.WriteLine ("loading background");
				background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (background_path),
									background_translucent, background_transparent);
			}

			if (binFile != null) {
				Console.WriteLine ("loading ui elements");
				Bin = (Bin)mpq.GetResource (binFile);

				if (Bin == null)
					throw new Exception (String.Format ("specified file '{0}' does not exist",
									    binFile));

				/* convert all the BinElements to UIElements for our subclasses to use */
				Elements = new List<UIElement> ();
				foreach (BinElement el in Bin.Elements) {
					//					Console.WriteLine ("{0}: {1}", el.text, el.flags);

					UIElement ui_el = null;
					switch (el.type) {
					case ElementType.DialogBox:
						ui_el = new DialogBoxElement (this, el, fontpal.RgbData);
						break;
					case ElementType.Image:
						ui_el = new ImageElement (this, el, fontpal.RgbData);
						break;
					case ElementType.TextBox:
						ui_el = new TextBoxElement (this, el, fontpal.RgbData);
						break;
					case ElementType.ListBox:
						ui_el = new ListBoxElement (this, el, fontpal.RgbData);
						break;
					case ElementType.ComboBox:
						ui_el = new ComboBoxElement (this, el, fontpal.RgbData);
						break;
					case ElementType.LabelLeftAlign:
					case ElementType.LabelCenterAlign:
					case ElementType.LabelRightAlign:
						ui_el = new LabelElement (this, el, fontpal.RgbData);
						break;
					case ElementType.Button:
					case ElementType.DefaultButton:
					case ElementType.ButtonWithoutBorder:
						ui_el = new ButtonElement(this, el, fontpal.RgbData);
						break;
					case ElementType.Slider:
					case ElementType.OptionButton:
					case ElementType.CheckBox:
						ui_el = new UIElement (this, el, fontpal.RgbData);
						break;
					default:
						Console.WriteLine ("unhandled case {0}", el.type);
						ui_el = new UIElement (this, el, fontpal.RgbData);
						break;
					}

					Elements.Add (ui_el);
				}

				UIPainter = new UIPainter (Elements);
			}
		}

		void LoadResources ()
		{
			ResourceLoader ();
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		public void Load ()
		{
			if (loaded)
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (RaiseReadyEvent)));
			else
#if MULTI_THREADED
				ThreadPool.QueueUserWorkItem (delegate (object state) { LoadResources (); })
#else
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (LoadResources)));
#endif
		}

		void FinishedLoading ()
		{
			loaded = true;
			RaiseReadyEvent ();
		}

		public virtual void ShowDialog (UIDialog dialog)
		{
			Console.WriteLine ("showing {0}", dialog);

			if (this.dialog != null)
				throw new Exception ("only one active dialog is allowed");
			this.dialog = dialog;

			dialog.Load ();
			dialog.Ready += delegate () { dialog.AddToPainter (painter); };
		}

		public virtual void DismissDialog ()
		{
			if (dialog == null)
				return;

			dialog.RemoveFromPainter (painter);
			dialog = null;
		}
	}

}
