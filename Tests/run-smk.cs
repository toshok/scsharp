
using System;
using System.IO;

using Starcraft;

using Gtk;
using Gdk;
using GLib;

public class RunSMK {

	Painter painter;
	Gtk.Window window;
	Gtk.DrawingArea drawing_area;

	Smk smk;

	Gdk.Pixbuf current_frame;

	public RunSMK (string filename)
	{
		FileStream fs = File.OpenRead (filename);

		smk = new SMK();
		((MPQResource)smk).ReadFromStream (fs);

		smk.FrameReady += SmkFrameReady;
		smk.AnimationDone += SmkAnimationDone;

		smk.Play ();

		CreateWindow ();
	}

	void CreateWindow ()
	{
		window = new Gtk.Window ("animation viewer");
		window.SetDefaultSize (640, 480);

		drawing_area = new Gtk.DrawingArea ();

		painter = new Painter (drawing_area, 200);
		painter.Add (Layer.Background, DrawFrame);

		window.Add (drawing_area);
		window.ShowAll ();
	}

	void DrawFrame (Gdk.Pixbuf pb, DateTime now)
	{
		Console.WriteLine ("draw frame");
		smk.NextFrame ();
		if (current_frame != null)
			current_frame.Composite (pb, 0, 0, (int)smk.Width, (int)smk.Height,
						 0, 0, 1, 1, InterpType.Nearest, 0xff);
	}

	FileStream[] audioFiles;
	uint[] dataChunkPosition;
	int frame_count;

	void StartupAudio (int channel)
	{
		if (audioFiles[channel] == null) {
			audioFiles[channel] = File.OpenWrite (String.Format ("audio-{0}.wav", channel));

			/* write the RIFF header */
			Util.WriteDWord (audioFiles[channel], 0x46464952 /* RIFF */);
			Util.WriteDWord (audioFiles[channel], 0); /* this gets filled in in FinishAudio */
			Util.WriteDWord (audioFiles[channel], 0x45564157 /* WAVE */);

			/* write the fmt chunk */

			uint sampleRate = smk.AudioRate[0];
			ushort sigBitsPerSample = 16;
			uint avgBytesPerSec = sampleRate * sigBitsPerSample / 8;

			Util.WriteDWord (audioFiles[channel], 0x20746D66 /* "fmt " */);
			Util.WriteDWord (audioFiles[channel], 16);
			Util.WriteWord (audioFiles[channel], 1); /* not compressed */
			Util.WriteWord (audioFiles[channel], 2); /* number of channels */
			Util.WriteDWord (audioFiles[channel], sampleRate); /* sample rate (8kHZ ?) XXX */
			Util.WriteDWord (audioFiles[channel], avgBytesPerSec);
			Util.WriteWord (audioFiles[channel], 4); /* 16bit stereo */
			Util.WriteWord (audioFiles[channel], sigBitsPerSample);

			/* write the header for the data chunk */
			Util.WriteDWord (audioFiles[channel], 0x61746164 /* "data" */);
			dataChunkPosition[channel] = (uint)audioFiles[channel].Position;
			Util.WriteDWord (audioFiles[channel], 0); /* this gets filled in in FinishAudio */
		}
	}

	void FinishAudio()
	{
		if (audioFiles != null) {
			for (int i = 0; i < 7; i ++) {
				if (audioFiles[i] == null)
					continue;

				uint file_size = (uint)audioFiles[i].Position;
				/* write the size to the DATA chunk header */
				audioFiles[i].Position = dataChunkPosition[i];
				Util.WriteDWord (audioFiles[i], file_size - dataChunkPosition[i] - 4);

				/* write the full filesize to the RIFF header */
				audioFiles[i].Position = 4;
				Util.WriteDWord (audioFiles[i], file_size - 8);
				audioFiles[i].Close();
				audioFiles[i] = null;
			}
		}
	}

	void SmkFrameReady (byte[] pixelBuffer, byte[] palette,
			    byte[][] audioBuffers)
	{
		Console.WriteLine ("SmkFrameReady");
		frame_count++;
		byte[] pixbuf_data = new byte[pixelBuffer.Length * 3];

		for (int i = 0; i < pixelBuffer.Length; i ++)
			Array.Copy (palette, pixelBuffer[i] * 3, pixbuf_data, i * 3, 3);

		current_frame = new Gdk.Pixbuf (pixbuf_data,
						Colorspace.Rgb,
						false,
						8,
						(int)smk.PaddedWidth, (int)smk.Height,
						(int)smk.PaddedWidth * 3,
						null);

		TGA.WriteTGA (String.Format ("frame{0:0000}.tga", frame_count),
			      pixbuf_data, smk.PaddedWidth, smk.PaddedHeight);

		if (audioFiles == null) {
			audioFiles = new FileStream[7];
			dataChunkPosition = new uint[7];
		}

		for (int ch = 0; ch < 7; ch ++) {
			if (audioBuffers[ch] != null) {
				if (audioFiles[ch] == null)
					StartupAudio (ch);

				audioFiles[ch].Write (audioBuffers[ch], 0, audioBuffers[ch].Length);
			}
		}
	}

	void SmkAnimationDone ()
	{
		smk.FrameReady -= SmkFrameReady;
		FinishAudio();
	}

	public static void Main (string[] args) {
		Application.Init();

		string filename;

		if (args == null || args.Length == 0)
			filename = "/home/toshok/starcraft/stardat-uncompressed/portrait/UDisk/UDdFid00.smk";
		else
			filename = args[0];

		new RunSMK (filename);

		Application.Run ();
	}
}
