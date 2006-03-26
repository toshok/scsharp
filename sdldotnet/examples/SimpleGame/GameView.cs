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
using System.Drawing;
using System.Collections;
using SdlDotNet;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Derived class
	/// </summary>
	public class GameView : IDisposable
	{
		Map map;
		int height;
		int width;
		Surface surf;
		//Names names;
		EntitySprite entitySprite;
		Rectangle[] mapRectangles;
		ArrayList backSprites;
		ArrayList frontSprites;
		Sound sound;
		string data_directory = @"Data/";
		string filepath = @"../../";
		/// <summary>
		/// constructor
		/// </summary>
		public GameView (EventManager eventManager)
		{
			if (eventManager == null)
			{
				throw new ArgumentNullException("eventManager");
			}
			eventManager.OnMapBuiltEvent += new EventManager.MapBuiltEventHandler(Subscribe);
			eventManager.OnEntityPlaceEvent += new EventManager.EntityPlaceEventHandler(Subscribe);
			eventManager.OnEntityMoveEvent += new EventManager.EntityMoveEventHandler(Subscribe);
			Events.Tick +=new TickEventHandler(this.Tick);
			Events.Quit += new QuitEventHandler(this.Quit);
			this.width = 424;
			this.height = 440;
			this.backSprites = new ArrayList();
			this.frontSprites = new ArrayList();
			if (File.Exists(data_directory + "boing.wav"))
			{
				filepath = "";
			}
			try
			{
				this.sound = Mixer.Sound(filepath + data_directory + "boing.wav");
			}
			catch (SdlException)
			{
				// Linux audio problem
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ShowMap(Map map)
		{
			mapRectangles = new Rectangle[9];
			int x = 10;
			int y = 10;
			int width = 128;
			int height = 128;
			int i = 0;

			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			foreach (Sector sec in map.GetSectors())
			{
				if (i < 3)
				{
					mapRectangles[i] = new Rectangle(x, y, width ,height);
					LogFile.WriteLine(mapRectangles[i].ToString());
					x+=138;
				} 
				else if (i >= 3 && i < 6)
				{
					if (i == 3)
					{
						x = 10;
					}
					y = 148;					
				
					mapRectangles[i] = new Rectangle(x, y, width ,height);
					LogFile.WriteLine(mapRectangles[i].ToString());
					x+=138;
				} 
				else if (i >= 6)
				{
					if (i == 6)
					{
						x = 10;
					}
					y = 286;					
			
					mapRectangles[i] = new Rectangle(x, y, width ,height);
					LogFile.WriteLine(mapRectangles[i].ToString());
					x+=138;
				}	
				backSprites.Add(new SectorSprite(Video.Screen, sec, mapRectangles[i]));
				i++;
			}					
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateView()
		{
			Video.WindowIcon();
			Video.WindowCaption = Names.WindowCaption;
			Video.SetVideoModeWindow(this.width, this.height);
			this.surf = Video.Screen.CreateCompatibleSurface(width, height);
			//fill the surface with black
			this.surf.Fill(new Rectangle(new Point(0, 0), surf.Size), Color.Black);
			Mouse.ShowCursor = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateView()
		{
			this.surf.Fill(new Rectangle(new Point(0, 0), surf.Size), Color.Black);
			foreach (SectorSprite i in this.backSprites)
			{
				Video.Screen.Blit(i.Surface, i.Rectangle);
			}
			foreach (EntitySprite j in this.frontSprites)
			{
				Video.Screen.Blit(j.Surface, j.Rectangle);
			}
			Video.Screen.Flip();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void ShowEntity(Entity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			this.entitySprite = new EntitySprite(surf);
			this.frontSprites.Add(this.entitySprite);
			SectorSprite sectSprite = this.GetSectorSprite(entity.Sector);
			this.entitySprite.Center = sectSprite.Center;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void MoveEntity(Entity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			SectorSprite sectSprite = this.GetSectorSprite(entity.Sector);
			this.entitySprite.Center = sectSprite.Center;
			if (this.sound != null)
			{
				this.sound.Play();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventManager"></param>
		/// <param name="e"></param>
		void Subscribe(object eventManager, MapBuiltEventArgs e)
		{
			LogFile.WriteLine("GameView received a MapBuilt event");
			this.map = e.Map;
			ShowMap(this.map);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventManager"></param>
		/// <param name="e"></param>
		void Subscribe(object eventManager, EntityPlaceEventArgs e)
		{
			LogFile.WriteLine("GameView received a EntityPlace event");
			ShowEntity(e.Entity);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventManager"></param>
		/// <param name="e"></param>
		void Subscribe(object eventManager, EntityMoveEventArgs e)
		{
			LogFile.WriteLine("GameView received a EntityMove event");
			MoveEntity(e.Entity);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sector"></param>
		/// <returns></returns>
		public SectorSprite GetSectorSprite(Sector sector)
		{
			SectorSprite sectSprite = null;
			foreach (SectorSprite s in this.backSprites)
			{
				if (s.Sector == sector)
				{
					sectSprite = s;
				}
			}
			return sectSprite;
		}

		private void Tick(object sender, TickEventArgs e)
		{
			LogFile.WriteLine("GameView received a Tick event");
			UpdateView();
		}
		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy sprite
		/// </summary>
		/// <param name="disposing">If true, remove all unamanged resources</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.entitySprite != null)
					{
						this.entitySprite.Dispose();
						this.entitySprite = null;
					}
				}
				this.disposed = true;
			}
		}
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
		~GameView() 
		{
			Dispose(false);
		}

		#endregion

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}
	}
}
