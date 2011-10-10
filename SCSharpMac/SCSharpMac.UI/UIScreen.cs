//
// SCSharpMac.UI.UIScreen
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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

using MonoMac.CoreAnimation;
using MonoMac.AppKit;

using SCSharp;

namespace SCSharpMac.UI
{
	public abstract class UIScreen : CALayer
	{
		CALayer background;
		protected CursorAnimator Cursor;
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

			background_transparent = -1;
			background_translucent = -1;
			
			Bounds = new RectangleF (0, 0, 640, 480);
			AnchorPoint = new PointF (0, 0);
		}

		protected UIScreen (Mpq mpq)
		{
			this.mpq = mpq;
			
			Bounds = new RectangleF (0, 0, 640, 480);
			AnchorPoint = new PointF (0, 0);
		}

#if notyet
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
#endif
		
		
		public virtual void AddToPainter ()
		{
#if notyet
			if (Cursor != null)
				Game.Instance.Cursor = Cursor;
#endif
		}

		public virtual void RemoveFromPainter ()
		{
			if (Cursor != null)
				Game.Instance.Cursor = null;
		}

		public virtual bool UseTiles {
			get { return false; }
		}

		public Mpq Mpq {
			get { return mpq; }
		}

		public CALayer Background {
			get { return background; }
		}
		
		public PointF ScreenToLayer (PointF point)
		{
			return new PointF (point.X - Position.X, point.Y - Position.Y);
		}
		
		public PointF LayerToScreen (PointF point)
		{
			return new PointF (point.X + Position.X, point.Y + Position.Y);
		}

		
		// FIXME we should be using CoreAnimation's HitTest code for this..
		UIElement XYToElement (PointF p, bool onlyUI)
		{			
			if (Elements == null)
				return null;

			PointF ui_pt = ScreenToLayer (p);

			foreach (UIElement e in Elements) {
				if (e.Type == ElementType.DialogBox)
					continue;

				if (onlyUI &&
				    e.Type == ElementType.Image)
					continue;
				
				if (e.Visible && e.PointInside (ui_pt))
					return e;
			}
			return null;
		}

		protected UIElement mouseDownElement;
		protected UIElement mouseOverElement;
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

		public virtual void MouseButtonDown (NSEvent theEvent)
		{
#if notyet
			if (args.Button != MouseButton.PrimaryButton &&
			    args.Button != MouseButton.WheelUp &&
			    args.Button != MouseButton.WheelDown)
				return;
#endif

			if (mouseDownElement != null)
				Console.WriteLine ("mouseDownElement already set in MouseButtonDown");
			
			UIElement element = XYToElement (theEvent.LocationInWindow, true);
			if (element != null && element.Visible && element.Sensitive) {
				mouseDownElement = element;
				if (theEvent.Type == NSEventType.LeftMouseDown)				
					mouseDownElement.MouseButtonDown (theEvent);
				else
					mouseDownElement.MouseWheel (theEvent);
			}
		}

		public void HandleMouseButtonDown (NSEvent theEvent)
		{
			if (dialog != null)
				dialog.HandleMouseButtonDown (theEvent);
			else
				MouseButtonDown (theEvent);
		}

		public virtual void MouseButtonUp (NSEvent theEvent)
		{
#if notyet
			if (args.Button != MouseButton.PrimaryButton &&
			    args.Button != MouseButton.WheelUp &&
			    args.Button != MouseButton.WheelDown)
				return;
#endif

			if (mouseDownElement != null) {
				if (theEvent.Type == NSEventType.LeftMouseUp)
					mouseDownElement.MouseButtonUp (theEvent);

				mouseDownElement = null;
			}
		}

		public void HandleMouseButtonUp (NSEvent theEvent)
		{
			if (dialog != null)
				dialog.HandleMouseButtonUp (theEvent);
			else
				MouseButtonUp (theEvent);
		}
		
		public virtual void PointerMotion (NSEvent theEvent)
		{
			if (mouseDownElement != null) {
				mouseDownElement.PointerMotion (theEvent);
			}
			else {
				UIElement newMouseOverElement = XYToElement (theEvent.LocationInWindow, true);

				if (newMouseOverElement != mouseOverElement) {
					if (mouseOverElement != null)
						MouseLeaveElement (mouseOverElement);
					if (newMouseOverElement != null)
						MouseEnterElement (newMouseOverElement);
				}
				
				mouseOverElement = newMouseOverElement;				
			}
		}

		public void HandlePointerMotion (NSEvent theEvent)
		{
			if (dialog != null)
				dialog.HandlePointerMotion (theEvent);
			else
				PointerMotion (theEvent);
		}

		public virtual void KeyboardUp (NSEvent theEvent)
		{
		}

		public void HandleKeyboardUp (NSEvent theEvent)
		{
			if (dialog != null)
				dialog.HandleKeyboardUp (theEvent);
			else
				KeyboardUp (theEvent);
		}

		public virtual void KeyboardDown (NSEvent theEvent)
		{
			if (Elements != null) {
				string eventChars = theEvent.CharactersIgnoringModifiers;
				foreach (UIElement e in Elements) {
					/* check for the uielement's hotkey if it has one */
					if ((e.Flags & ElementFlags.HasHotkey) == ElementFlags.HasHotkey &&
						eventChars.Length == 1 &&
						eventChars[0] == e.Hotkey)
					{
						ActivateElement (e);
						return;
					}

					/* check for the deafult button */
					if (((e.Flags & ElementFlags.DefaultButton) == ElementFlags.DefaultButton &&
						eventChars[0] == 13)
					 || ((e.Flags & ElementFlags.CancelButton) == ElementFlags.CancelButton &&
						eventChars[0] == 27)) {
						ActivateElement (e);
						return;
					}
				}
			}
		}

		public void HandleKeyboardDown (NSEvent theEvent)
		{				
			if (dialog != null)
				dialog.HandleKeyboardDown (theEvent);
			else
				KeyboardDown (theEvent);
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

		int translucentIndex = 254;
		protected int TranslucentIndex {
			get { return translucentIndex; }
			set { translucentIndex = value; }
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
					background = GuiUtil.LayerFromStream ((Stream)mpq.GetResource (background_path),
														  background_translucent, background_transparent);
				
				background.AnchorPoint = new PointF (0, 0);
				AddSublayer (background);
				// FIXME: we should center the background (and scale it?)
			}

			Elements = new List<UIElement> ();
			if (binFile != null) {
				Console.WriteLine ("loading ui elements");
				Bin = (Bin)mpq.GetResource (binFile);

				if (Bin == null)
					throw new Exception (String.Format ("specified file '{0}' does not exist",
									    binFile));

				/* convert all the BinElements to UIElements for our subclasses to use */
				foreach (BinElement el in Bin.Elements) {
					//					Console.WriteLine ("{0}: {1}", el.text, el.flags);

					UIElement ui_el = null;
					switch (el.type) {
					case ElementType.DialogBox:
						ui_el = new DialogBoxElement (this, el, fontpal.RGBData);
						break;
					case ElementType.Image:
						ui_el = new ImageElement (this, el, fontpal.RGBData, translucentIndex);
						break;
					case ElementType.TextBox:
						ui_el = new TextBoxElement (this, el, fontpal.RGBData);
						break;
					case ElementType.ListBox:
						ui_el = new ListBoxElement (this, el, fontpal.RGBData);
						break;
					case ElementType.ComboBox:
						ui_el = new ComboBoxElement (this, el, fontpal.RGBData);
						break;
					case ElementType.LabelLeftAlign:
					case ElementType.LabelCenterAlign:
					case ElementType.LabelRightAlign:
						ui_el = new LabelElement (this, el, fontpal.RGBData);
						break;
					case ElementType.Button:
					case ElementType.DefaultButton:
					case ElementType.ButtonWithoutBorder:
						ui_el = new ButtonElement(this, el, fontpal.RGBData);
						break;
					case ElementType.Slider:
					case ElementType.OptionButton:
					case ElementType.CheckBox:
						ui_el = new UIElement (this, el, fontpal.RGBData);
						break;
					default:
						Console.WriteLine ("unhandled case {0}", el.type);
						ui_el = new UIElement (this, el, fontpal.RGBData);
						break;
					}

					Elements.Add (ui_el);	
				}
			}
		}

		void LoadResources ()
		{
		
			ResourceLoader ();
			if (Elements != null) {
				foreach (var ui_el in Elements) {
					if (ui_el.Layer != null) {				
						ui_el.Layer.Position = new PointF (ui_el.X1, Bounds.Height - ui_el.Y1 - ui_el.Height);
						ui_el.Layer.AnchorPoint = new PointF (0, 0);
						AddSublayer (ui_el.Layer);
					}
				}
			}
			NSApplication.SharedApplication.InvokeOnMainThread (FinishedLoading);
		}

		public void Load ()
		{			
			if (loaded)
				NSApplication.SharedApplication.InvokeOnMainThread (RaiseReadyEvent);
			else
				NSApplication.SharedApplication.InvokeOnMainThread (LoadResources);				
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
			
			dialog.Ready += delegate () { dialog.AddToPainter (); };
			dialog.Load ();
		}

		public virtual void DismissDialog ()
		{
			if (dialog == null)
				return;

			dialog.RemoveFromPainter ();
			dialog = null;
		}
	}

}
