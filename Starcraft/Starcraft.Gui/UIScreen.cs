
using System;
using System.IO;
using System.Threading;

using SdlDotNet;

namespace Starcraft
{
	public abstract class UIScreen
	{
		Surface background;
		CursorAnimator cursor;
		UIPainter ui_painter;
		Bin ui;
		protected Mpq mpq;

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
			if (background != null)
				painter.Add (Layer.Background, BackgroundPainter);

			if (ui_painter != null)
				painter.Add (Layer.UI, ui_painter.Paint);

			if (cursor != null)
				Game.Instance.Cursor = cursor;
		}

		public virtual void RemoveFromPainter (Painter painter)
		{
			if (background != null)
				painter.Remove (Layer.Background, BackgroundPainter);
			if (ui_painter != null)
				painter.Remove (Layer.UI, ui_painter.Paint);
			if (cursor != null)
				Game.Instance.Cursor = null;
		}

		UIElement XYToElement (int x, int y, bool onlyUI)
		{
			if (UI == null)
				return null;

			foreach (UIElement e in UI.Elements) {
				if (e.type == UIElementType.DialogBox)
					continue;

				if (onlyUI &&
				    e.type == UIElementType.Image)
					continue;

				if (x >= e.x1 && x < e.x1 + e.width &&
				    y >= e.y1 && y < e.y1 + e.height)
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
			if (element.type == UIElementType.Button
			    || element.type == UIElementType.ButtonWithoutBorder
			    || element.type == UIElementType.DefaultButton) {

				GuiUtil.PlaySound (mpq, Builtins.MouseoverWav);
			}
		}

		public virtual void ActivateElement (UIElement element)
		{
		}

		public virtual void KeyboardDown (KeyboardEventArgs args)
		{
		}

		public virtual void KeyboardUp (KeyboardEventArgs args)
		{
			foreach (UIElement e in UI.Elements) {
				if ( (args.Key == (Key)e.hotkey)
				     ||
				     (args.Key == Key.Escape
				      && (e.flags & UIElementFlags.CancelButton) == UIElementFlags.CancelButton)) {
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

		protected Surface Background {
			get { return background; }
			set { background = value; }
		}

		protected CursorAnimator Cursor {
			get { return cursor; }
			set { cursor = value; }
		}

		protected Bin UI {
			get { return ui; }
			set { ui = value; }
		}

		protected UIPainter UIPainter {
			get { return ui_painter; }
			set { ui_painter = value; }
		}

		protected void BackgroundPainter (Surface surf, DateTime dt)
		{
			surf.Blit (background);
		}

		protected abstract void ResourceLoader (object state);

		public void Load ()
		{
			if (loaded)
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (RaiseReadyEvent)));
			else
				ThreadPool.QueueUserWorkItem (ResourceLoader);
		}

		protected virtual void FinishedLoading ()
		{
			loaded = true;
			RaiseReadyEvent ();
		}
	}

}
