/* 
 * $RCSfile$ 
 * Copyright (C) 2005 Rob Loach (http://www.robloach.net) 
 * 
 * This library is free software; you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General Public 
 * License as published by the Free Software Foundation; either 
 * version 2.1 of the License, or (at your option) any later version. 
 * 
 * This library is distributed in the hope that it will be useful, 
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU 
 * Lesser General Public License for more details. 
 * 
 * You should have received a copy of the GNU Lesser General Public 
 * License along with this library; if not, write to the Free Software 
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 */ 

using System; 

using Tao.Sdl; 

namespace SdlDotNet 
{ 
	/// <summary> 
	/// The KeyboardState class interface describes the key state of every button on the keyboard. 
	/// </summary> 
	/// <example> 
	/// <code> 
	/// KeyboardState keys = new KeyboardState(); 
	/// 
	/// if( keys.IsKeyPressed(Key.A) ) 
	/// { 
	///     // The A key is pressed. 
	/// } 
	///      
	/// if( keys[Key.F] )   // Another way to check 
	/// { 
	///     // The F key is currently down. 
	/// } 
	/// else 
	/// { 
	///     // The F key is not down. 
	/// } 
	/// </code> 
	/// </example> 
	public class KeyboardState 
	{ 
		/// <summary> 
		/// The state information of all the keys in the current state. 
		/// </summary> 
		private byte[] m_Keys; 

		/// <summary> 
		/// Gets the current state of the given key (true means the key is pushed, false if not). 
		/// </summary> 
		public bool this[Key key] 
		{ 
			get 
			{ 
				return m_Keys[(int)key] == 1; 
			} 
			set 
			{ 
				m_Keys[(int)key] = value ? (byte)1 : (byte)0; 
			} 
		} 

		/// <summary> 
		/// Creates a new KeyboardState object with the current state of the keyboard. 
		/// </summary> 
		/// <remarks>This calls update on creation.</remarks>
		public KeyboardState() : this(true)
		{ 
		} 

		/// <summary>
		/// Creates a new KeyboardState with an option to update on creation.
		/// </summary>
		/// <param name="updateNow">Update the key array on object creation.  Defaults to true.</param>
		public KeyboardState(bool updateNow)
		{
			if(updateNow)
			{
				Update();
			}
		}

		/// <summary>
		/// Updates the keyboard state within this object.
		/// </summary>
		public void Update()
		{
			int numberOfKeys; 
			m_Keys = Sdl.SDL_GetKeyState(out numberOfKeys); 
		}

		/// <summary> 
		/// Checks a key state in the KeyboardState. 
		/// </summary> 
		/// <param name="key">The key to be checked.</param> 
		/// <returns>True if the key is pressed, false if it isn't.</returns> 
		public bool IsKeyPressed(Key key) 
		{ 
			return m_Keys[(int)key] == 1; 
		}
	} 
} 
