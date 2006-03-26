/* This file is part of BombRun
 * (c) 2003 Sijmen Mulder
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;

using SdlDotNet;
using SdlDotNet.Sprites;

namespace SdlDotNet.Examples.BombRun
{
	/// <summary>
	/// 
	/// </summary>
	public class BombRun : IDisposable
	{
		Surface screen;
		static float bombSpeed = 100;
		Surface background;
		Surface alternateBackground;
		Surface temporary;
		Player player;
		Surface tempSurface;
		SpriteCollection bombs = new SpriteCollection();
		SpriteCollection players = new SpriteCollection();
		SpriteCollection bullets = new SpriteCollection();
		SpriteCollection master = new SpriteCollection();
		string data_directory = @"Data/";
		string filepath = @"../../";

		/// <summary>
		/// 
		/// </summary>
		public void Run()
		{
			if (File.Exists(data_directory + "Background1.png"))
			{
				filepath = "";
			}
			Video.WindowIcon();
			Video.WindowCaption =
				"SDL.NET - Bomb Run";
			screen = Video.SetVideoModeWindow(640, 480);
			tempSurface = new Surface(filepath + data_directory + "Background1.png");
			background = tempSurface.Convert();
			tempSurface = new Surface(filepath + data_directory + "Background2.png");
			alternateBackground = tempSurface.Convert();

			temporary = screen.CreateCompatibleSurface(32, 32);
			temporary.TransparentColor = Color.FromArgb(0, 255, 0, 255);

			player = new Player(new Surface(filepath + data_directory + "Head.bmp"), new Point(screen.Width / 2 - 16,
				screen.Height - 32));
			players.Add(player);
			players.EnableKeyboardEvent();
			bullets.EnableTickEvent();
			master.EnableTickEvent();

			for(int i = 0; i < 25; i++)
			{
				bombs.Add(new Bomb(new Surface(filepath + data_directory + "Bomb.bmp")));
			}

			master.Add(bombs);
			master.Add(players);

			Mouse.ShowCursor = false;
			Events.KeyboardDown +=
				new KeyboardEventHandler(this.Keyboard);
			Events.Quit += new QuitEventHandler(this.Quit);
			player.WeaponFired += new FireEventHandler(PlayerWeaponFired);

			Events.Tick += new TickEventHandler(this.OnTick);
			Events.Run();
		}

		[STAThread]
		static void Main()
		{
			BombRun bombRun = new BombRun();
			bombRun.Run();
		}

		private void PlayerWeaponFired(object sender, FireEventArgs e)
		{
			Bullet bullet = new Bullet(e.Location, 0, -250);
			bullets.Add(bullet);
		}

		private void Keyboard(object sender, KeyboardEventArgs e)
		{
			if(e.Key == Key.Escape || e.Key == Key.Q)
			{
				Events.QuitApplication();
			}
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		Rectangle src;
		Rectangle dest;

		private void OnTick(object sender, TickEventArgs args)
		{
			//Console.WriteLine(args.SecondsElapsed);
			screen.Blit(background);

			for(int i = 0; i < master.Count; i++)
			{
				src = new Rectangle(new Point(0, 0), master[i].Size);
				dest = new Rectangle(master[i].Position, master[i].Size);

				temporary.Blit(alternateBackground, src, dest);
				temporary.Blit(master[i].Surface, src);
				screen.Blit(temporary, dest);
			}

			screen.Blit(bullets);
			screen.Flip();
		}

		/// <summary>
		/// 
		/// </summary>
		public static float BombSpeed
		{
			get
			{ 
				return bombSpeed; 
			}
			set
			{
				bombSpeed = value; 
			}
		}
		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Close() 
		{
			Dispose();
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		~BombRun() 
		{
			Dispose(false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (tempSurface != null)
					{
						tempSurface.Dispose();
						tempSurface = null;
					}
					if (this.temporary != null)
					{
						this.temporary.Dispose();
						this.temporary = null;
					}
					if (this.background != null)
					{
						this.background.Dispose();
						this.background = null;
					}
					if (this.alternateBackground != null)
					{
						this.alternateBackground.Dispose();
						this.alternateBackground = null;
					}
					if (player != null)
					{
						player.Dispose();
						player = null;
					}
				}
				this.disposed = true;
			}
		}

		#endregion
	}
}
