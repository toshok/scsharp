
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {

	public class ParallaxObject
	{
		public readonly int X;
		public readonly int Y;

		public ParallaxObject (int X, int Y, int offset)
		{
			this.X = X;
			this.Y = Y;
		}
	}

	public class ParallaxLayer
	{
		ParallaxObject[] objects;

		public ParallaxLayer (int num_objects)
		{
			objects = new ParallaxObject[num_objects];
		}

		public void ReadFromStream (Stream stream)
		{
			for (int i = 0; i < objects.Length; i ++) {
				ushort X = Util.ReadWord (stream);
				ushort Y = Util.ReadWord (stream);
				ushort offset = Util.ReadWord (stream);
				Util.ReadWord (stream); /* read that last, unused word */

				objects[i] = new ParallaxObject (X, Y, offset);
			}
		}

		public ParallaxObject[] Objects {
			get { return objects; }
		}
	}

	public class Spk : MpqResource {
		ParallaxLayer[] layers;

		public Spk ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			layers = new ParallaxLayer[Util.ReadWord (stream)];

			for (int i = 0; i < layers.Length; i ++) {
				int numobjects = Util.ReadWord (stream);
				layers[i] = new ParallaxLayer (numobjects);
				Console.WriteLine ("Layer {0} has {1} objects", i, numobjects);
			}

			for (int i = 0; i < layers.Length; i ++) {
				Console.WriteLine ("Reading layer {0}", i);
				layers[i].ReadFromStream (stream);
			}
		}

		public ParallaxLayer[] Layers {
			get { return layers; }
		}
	}

}
