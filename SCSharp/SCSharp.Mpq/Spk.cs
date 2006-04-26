//
// SCSharp.Mpq.Spk
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
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{

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
