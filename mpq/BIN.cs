
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {
	[Flags]
	public enum UIElementFlags {
		//???             = 0x00000001,
		//???             = 0x00000002,
		//???             = 0x00000004,
		Visible           = 0x00000008,
		RespondToMouse    = 0x00000010,
		//???             = 0x00000020,
		CancelButton      = 0x00000040,
		NoSoundOnMouseOvr = 0x00000080,
		//???             = 0x00000100,
		HasHotkey         = 0x00000200,
		FontSmall         = 0x00000400,
		FontMedium        = 0x00000800,
		//???             = 0x00001000,
		Transparent       = 0x00002000,
		FontLarge         = 0x00004000,
		Unused00008000    = 0x00008000,
		Unused00010000    = 0x00010000,
		Unused00020000    = 0x00020000,
		ApplyTranslucency = 0x00040000,
		DefaultButton     = 0x00080000,
		Unused00100000    = 0x00100000,
		GreenText         = 0x00200000,
		Unused00400000    = 0x00400000,
		Unused00800000    = 0x00800000,
		Unused01000000    = 0x01000000,
		Unused02000000    = 0x02000000,
		NoClickSound      = 0x04000000,
		Unused08000000    = 0x08000000
	}

	public enum UIElementType {
		DialogBox = 0,
		DefaultButton = 1,
		Button = 2,
		OptionButton = 3,
		CheckBox = 4,
		Image = 5,
		Slider = 6,
		TextBox = 8,
		LabelLeftAlign = 9,
		LabelCenterAlign = 10,
		LabelRightAlign = 11,
		ListBox = 12,
		ComboBox = 13,
		ButtonWithoutBorder = 14
	}

	public class UIElement
	{
		public ushort x1;
		public ushort y1;
		public ushort x2;
		public ushort y2;

		public ushort width;
		public ushort height;

		public char hotkey;
		public string text;
		public uint text_offset;

		public UIElementFlags flags;
		public UIElementType type;

		public object resolvedData;

		public UIElement (byte[] buf, int position, uint stream_length)
		{
			x1 = Util.ReadWord (buf, position + 4);
			y1 = Util.ReadWord (buf, position + 6);
			x2 = Util.ReadWord (buf, position + 8);
			y2 = Util.ReadWord (buf, position + 10);
			width = Util.ReadWord (buf, position + 12);
			height = Util.ReadWord (buf, position + 14);
			text_offset = Util.ReadDWord (buf, position + 20);

			flags = (UIElementFlags)Util.ReadDWord (buf, position + 24);
			type = (UIElementType)buf[position + 34];

			if (text_offset < stream_length) {
				uint text_length = 0;
				while (buf[text_offset + text_length] != 0) text_length ++;

				text = Encoding.ASCII.GetString (buf, (int)text_offset, (int)text_length);

				if ((flags & UIElementFlags.HasHotkey) == UIElementFlags.HasHotkey) {
					hotkey = text[0];
					text = text.Substring (1);
				}
			}
			else
				text = "";

			Console.WriteLine ("flags = {0:x}", flags);

			Console.WriteLine ("flags1 = {0}", buf[position+24]);
			Console.WriteLine ("flags2 = {0}", buf[position+24 + 1]);
			Console.WriteLine ("flags3 = {0}", buf[position+24 + 2]);
			Console.WriteLine ("flags4 = {0}", buf[position+24 + 3]);
		}

		public void DumpFlags ()
		{
			foreach (UIElementFlags f in Enum.GetValues (typeof (UIElementFlags)))
				if ((flags & f) == f)
					Console.WriteLine (f);
		}
	}

	public class BIN : MPQResource {
		Stream stream;
		List<UIElement> elements;

		public BIN ()
		{
			elements = new List<UIElement> ();
		}

		void MPQResource.ReadFromStream (Stream stream)
		{
			this.stream = stream;
			ReadElements ();
		}

		void ReadElements ()
		{
			int position;

			byte[] buf = new byte[stream.Length];

			stream.Read (buf, 0, (int)stream.Length);

			position = 0;
			do {
				UIElement element = new UIElement (buf, position, (uint)stream.Length);

				elements.Add (element);

				position += 86;
			} while (position < elements[0].text_offset);
		}

		UIElement[] arr;
		public UIElement[] Elements {
			get {
				if (arr == null) arr = elements.ToArray();
				return arr;
			}
		}
	}
}
