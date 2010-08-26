//
// SCSharp.Mpq.Dat
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
using System.Collections.Generic;

namespace SCSharp
{
	public abstract class Dat : MpqResource
	{

		protected byte[] buf;
		List<DatVariable> variables;
		Dictionary<int,DatCollection> collections;

		protected Dat ()
		{
			variables = new List<DatVariable> ();
			collections = new Dictionary<int,DatCollection> ();
		}

		public void ReadFromStream (Stream stream)
		{
			buf = new byte[(int)stream.Length];

			stream.Read (buf, 0, buf.Length);

			//Console.WriteLine ("buf size = {0}, offset = {0}", buf.Length, offset);
		}

		int offset = 0;

		protected int AddVariableBlock (int num_entries, DatVariableType var)
		{
			DatVariable new_variable = new DatVariable (var, offset, num_entries);

			int rv = variables.Count;

			variables.Add (new_variable);

			offset += new_variable.BlockSize ();

			return rv;
		}

		protected void AddUnknownBlock (int size)
		{
			offset += size;
		}

		protected int AddPlacedVariableBlock (int place, int num_entries, DatVariableType var)
		{
			offset = place;

			DatVariable new_variable = new DatVariable (var, offset, num_entries);

			int rv = variables.Count;

			variables.Add (new_variable);

			offset += new_variable.BlockSize ();

			return rv;
		}

		public int GetVariableOffset (int variableId)
		{
			return variables[variableId].Offset;
		}

		protected DatCollection GetCollection (int variableId)
		{
			if (collections.ContainsKey (variableId))
				return collections[variableId];

			DatCollection rv = variables[variableId].CreateCollection (buf);

			collections[variableId] = rv;
			return rv;
		}
	}


	public enum DatVariableType {
		Byte,
		Word,
		DWord
	}

	public class DatCollection {
	}

	public class DatCollection<T> : DatCollection {
		byte[] buf;
		DatVariable var;

		public DatCollection (byte[] buf, DatVariable var)
			: base ()
		{
			this.buf = buf;
			this.var = var;
		}

		public int Count {
			get { return var.NumEntries; }
		}

		public T this [int index] {
			get { return (T)var[buf, index]; }
		}
	}

	public class DatVariable {
		DatVariableType type;
		int offset;
		int num_entries;

		public DatVariable (DatVariableType type, int offset, int num_entries)
		{
			this.type = type;
			this.offset = offset;
			this.num_entries = num_entries;
		}

		public int Offset {
			get { return offset; }
		}

		public int NumEntries {
			get { return num_entries; }
		}

		public int BlockSize ()
		{
			switch (type) {
			case DatVariableType.Byte:
				return num_entries;
			case DatVariableType.Word:
				return num_entries * 2;
			case DatVariableType.DWord:
				return num_entries * 4;
			default:
				return 0;
			}
		}

		public object this [byte[] buf, int index] {
			get {
				switch (type) {
				case DatVariableType.Byte:
					return buf[offset + index];
				case DatVariableType.Word:
					return Util.ReadWord (buf, offset + index * 2);
				case DatVariableType.DWord:
					return Util.ReadDWord (buf, offset + index * 4);
				default:
					return null;
				}
			}
		}

		public DatCollection CreateCollection (byte[] buf)
		{
			switch (type) {
			case DatVariableType.Byte:
				return new DatCollection<byte>(buf, this);
			case DatVariableType.Word:
				return new DatCollection<ushort>(buf, this);
			case DatVariableType.DWord:
				return new DatCollection<uint>(buf, this);
			default:
				return null;
			}
		}
	}
}
