// Copyright 2005 David Hudson (jendave@yahoo.com)
// This file is part of SimpleGame.
//
// SimpleGame is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SimpleGame is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SimpleGame; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;
using System.Globalization;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Summary description for LogFile.
	/// </summary>
	public sealed class LogFile
	{
		static FileStream fs = new FileStream("log.txt",  FileMode.Append);

		static LogFile()
		{
			Initialize();
		}

		LogFile()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Initialize()
		{
			RotateLogs();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		public static void WriteLine(string format)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.WriteLine(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		public static void WriteLine(string format, Object arg0)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.WriteLine(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		public static void WriteLine(string format, Object arg0, Object arg1)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.WriteLine(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0, arg1);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		public static void WriteLine(string format, Object arg0, Object arg1, Object arg2)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.WriteLine(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0, arg1, arg2);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		public static void Write(string format)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			// Write to the file using StreamWriter class 
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.Write(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		public static void Write(string format, Object arg0)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.Write(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		public static void Write(string format, Object arg0, Object arg1)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.Write(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0, arg1);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		public static void Write(string format, Object arg0, Object arg1, Object arg2)
		{
			
			StreamWriter streamWriter = new StreamWriter(fs);
			streamWriter.BaseStream.Seek(0, SeekOrigin.End); 
			streamWriter.Write(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " " + format, arg0, arg1, arg2);
			streamWriter.Flush(); 
		}

		/// <summary>
		/// 
		/// </summary>
		public static void RotateLogs()
		{
			LogFile.fs = null;
			if (File.Exists(Names.LogFile3))
			{
				File.Delete(Names.LogFile3);
			}
			if (File.Exists(Names.LogFile2) & !File.Exists(Names.LogFile3))
			{
				File.Move(Names.LogFile2, Names.LogFile3);
			}
			if (File.Exists(Names.LogFile) & !File.Exists(Names.LogFile2))
			{
				File.Move(Names.LogFile, Names.LogFile2);
			}
			LogFile.fs = new FileStream(Names.LogFile, FileMode.OpenOrCreate, FileAccess.Write); 
		}
	}
}
