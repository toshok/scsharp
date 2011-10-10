//
// SCSharpMac.UI.MovieElement
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
using System.IO;
using System.Text;
using System.Threading;

using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class MovieElement : UIElement
	{
		public MovieElement (UIScreen screen, BinElement el, byte[] palette, bool scale)
			: base (screen, el, palette)
		{
			this.scale = scale;
		}

		public MovieElement (UIScreen screen, BinElement el, byte[] palette, string resource, bool scale)
			: this (screen, el, palette, resource)
		{
			this.scale = scale;
		}

		public MovieElement (UIScreen screen, BinElement el, byte[] palette, string resource)
			: base (screen, el, palette)
		{
			Player = new SmackerPlayer ((Stream)Mpq.GetResource (resource), 1);
			Player.FrameReady += NewFrame;
		}

		public MovieElement (UIScreen screen, BinElement el, byte[] palette, SmackerPlayer player)
			: base (screen, el, palette)
		{
			Player = player;
		}

		public MovieElement (UIScreen screen, int x, int y, int width, int height, bool scale)
			: base (screen, (ushort)x, (ushort)y, (ushort)width, (ushort)height)
		{
			this.scale = scale;
		}
		
		public override bool Sensitive {
			get { return false; }
			set { }
		}
		
		public SmackerPlayer Player {
			get { return player; }
			set {
				if (player != null) {
					player.FrameReady -= NewFrame;
					player.Stop ();
				}

				player = value;
				if (player != null) {
					ScalePlayer ();
					player.FrameReady += NewFrame;
				}
			}
		}

		public void Play ()
		{
			if (player != null)
				player.Play ();
		}

		public void Stop ()
		{
			if (player != null)
				player.Stop ();
		}

		public Size MovieSize {
			get {
				if (player == null)
					return new Size (0, 0);
				else
					return new Size (player.Width, player.Height);
			}
		}
		
		void CreateDimLayer ()
		{
			dimLayer = CALayer.Create ();
			dimLayer.Bounds = new RectangleF (0, 0, player.Width, player.Height);
			dimLayer.Position = new PointF (0, 0);
			dimLayer.AnchorPoint = new PointF (0, 0);
		}
		
		public void Dim (byte dimness)
		{
			if (dim == dimness)
				return;
			
			if (dim > 0) {
				if (dimness > 0)
					dimLayer.BackgroundColor = new CGColor (0, (float)dimness / 255);
				else
					dimLayer.RemoveFromSuperLayer ();
			}
			else {
				if (dimness > 0) {
					if (dimLayer == null)
						CreateDimLayer ();
					dimLayer.BackgroundColor = new CGColor (0, (float)dimness / 255);
					if (layer != null)
						layer.AddSublayer (dimLayer);
				}
			}
			
			dim = dimness;
		}
		
		void NewFrame ()
		{
			if (layer != null)
				layer.SetNeedsDisplay ();
		}
		
		SmackerPlayer player;
		CALayer layer;
		CALayer dimLayer;
		byte dim = 0;
		bool scale;
		internal float playerZoom = 1.0f;
		
		void ScalePlayer ()
		{
			if (layer == null)
				return;

			playerZoom = 1.0f;
			
			if (scale
				&& (player.Width != Width
		    	|| player.Height != Height)) {

				float horiz_zoom = (float)Width / player.Width;
				float vert_zoom = (float)Height / player.Height;

				if (horiz_zoom < vert_zoom)
					playerZoom = horiz_zoom;
				else
					playerZoom = vert_zoom;
			}
			
			layer.AffineTransform = CGAffineTransform.MakeScale (playerZoom, playerZoom);
		}
		
		protected override CALayer CreateLayer ()
		{
			layer = CALayer.Create ();
			layer.Bounds = new RectangleF (0, 0, Width, Height);
			layer.AnchorPoint = new PointF (0, 0);
			
			layer.Delegate = new MovieElementLayerDelegate (this);
			
			if (player == null)
				return layer;

			ScalePlayer ();
			
			if (dim != 0) {
				CreateDimLayer ();
				dimLayer.BackgroundColor = new CGColor (0, (float)dim / 255);
				layer.AddSublayer (dimLayer);
			}				
			
			return layer;
		}
	}
	
	class MovieElementLayerDelegate : CALayerDelegate {
		MovieElement el;
		
		public MovieElementLayerDelegate (MovieElement el)
		{
			this.el = el;
		}
		
		public override void DrawLayer (CALayer layer, CGContext context)
		{
			if (el.Player.CurrentFrame != null)
				context.DrawImage (new RectangleF ( 0, el.Height - el.Player.CurrentFrame.Height * el.playerZoom, el.Player.CurrentFrame.Width, el.Player.CurrentFrame.Height), el.Player.CurrentFrame);
		}
	}

}
