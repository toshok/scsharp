using System;
using System.Collections.Generic;

using SdlDotNet;
using SdlDotNet.Sprites;
using System.Drawing;

namespace SCSharp {

	public enum Layer {
		Foo,

		Background,
		Map,
		Shadow,
		Selection,
		Unit,
		Health,
		Hud,
		UI,
		Popup,
		DialogDimScreenHack,
		DialogBackground,
		DialogUI,
		Tooltip,
		Cursor,

		Count
	}

	public delegate void PainterDelegate (Surface surf, DateTime now);

	public class Painter 
	{
		List<PainterDelegate>[] layers;
		int millis;
		int total_elapsed;

		DateTime now; /* the time of the last animation tick */

		Surface paintingSurface;
		Surface backbuffer;

		public Painter (Surface paintingSurface, int millis)
		{
			this.millis = millis;
			this.paintingSurface = paintingSurface;

			/* create an initialize our video surface */
			backbuffer = paintingSurface.CreateCompatibleSurface (paintingSurface.Size);
			backbuffer.Fill (new Rectangle (new Point (0, 0), backbuffer.Size), Color.Black);
			
			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				layers[(int)i] = new List<PainterDelegate>();

			/* and set ourselves up to invalidate at a regular interval*/
                        Events.Tick +=new TickEventHandler (Animate);
		}

		public void Add (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Add (painter);
		}

		public void Remove (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Remove (painter);
		}

		public void Clear (Layer layer)
		{
			layers[(int)layer].Clear ();
		}

		public void DrawText (int x, int y, string text)
		{
		}

		void Animate (object sender, TickEventArgs e)
		{
			total_elapsed += e.TicksElapsed;

			if (total_elapsed < millis)
				return;

			total_elapsed = 0;

			now = DateTime.Now;

                        backbuffer.Fill(new Rectangle(new Point(0, 0), backbuffer.Size), Color.Black);
			
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				DrawLayer (layers[(int)i]);

			paintingSurface.Blit (backbuffer);

			paintingSurface.Flip ();
		}

		void DrawLayer (List<PainterDelegate> painters)
		{
			foreach (PainterDelegate p in painters)
				p (backbuffer, now);
		}
	}
}
