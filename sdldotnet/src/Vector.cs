/*
 * $RCSfile$
 * Copyright (C) 2004 D. R. E. Moonfire (d.moonfire@mfgames.com)
 *               2005 Rob Loach (http://www.robloach.net)
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
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SdlDotNet
{
	/// <summary>
	/// Class for coordinates in three dimensions.
	/// </summary>
	[Serializable]
	public class Vector : ISerializable, ICloneable, IComparable
	{

		#region Constructors

		/// <summary>
		/// Creates point at 0, 0
		/// </summary>
		public Vector() : this(0, 0) 
		{ 
		}

		/// <summary>
		/// Creates a vector with a specific direction in 
		/// degrees and a length of 1.
		/// </summary>
		/// <param name="directionDeg">
		/// The direction of the vector, in degrees.
		/// </param>
		public Vector(int directionDeg)
		{
			Length = 1;
			DirectionDeg = directionDeg;
		}

		/// <summary>
		/// Creates a vector with a specific direction in 
		/// radians and a length of 1.
		/// </summary>
		/// <param name="directionRadians">
		/// The direction of the vector, in radians.
		/// </param>
		public Vector(float directionRadians)
		{
			Length = 1;
			Direction = directionRadians;
		}

		/// <summary>
		/// Creates a vector using integers.
		/// </summary>
		/// <param name="positionX">Coordinate on X-axis</param>
		/// <param name="positionY">Coordinate on Y-axis</param>
		public Vector(int positionX, int positionY)
		{
			m_x = (float)positionX;
			m_y = (float)positionY;
		}

		/// <summary>
		/// Creates a vector using floats.
		/// </summary>
		/// <param name="positionX">Coordinate on X-axis</param>
		/// <param name="positionY">Coordinate on Y-axis</param>
		public Vector(float positionX, float positionY)
		{
			m_x = positionX;
			m_y = positionY;
		}

		/// <summary>
		/// Creates a vector using doubles.
		/// </summary>
		/// <param name="positionX">Coordinate on X-axis</param>
		/// <param name="positionY">Coordinate on Y-axis</param>
		public Vector(double positionX, double positionY)
		{
			m_x = (float)positionX;
			m_y = (float)positionY;
		}

		/// <summary>
		/// Creates a vector based on a Point object.
		/// </summary>
		/// <param name="point">
		/// The point representing the XY values.
		/// </param>
		public Vector(Point point) : this(point.X, point.Y) 
		{
		}

		/// <summary>
		/// Creates a vector based on a PointF object.
		/// </summary>
		/// <param name="point">
		/// The point representing the XY values.
		/// </param>
		public Vector(PointF point) : this(point.X, point.Y) 
		{
		}

		/// <summary>
		/// Creates a vector based on the difference between the two given points.
		/// </summary>
		/// <param name="p1">The first point.</param>
		/// <param name="p2">The second offset point.</param>
		public Vector(PointF p1, PointF p2)
		{
			m_x = p2.X - p1.X;
			m_y = p2.Y - p1.Y;
		}

		/// <summary>
		/// Creates a vector based on the difference between the two given points.
		/// </summary>
		/// <param name="x1">The X coordinate of the first point.</param>
		/// <param name="y1">The Y coordinate of the first point.</param>
		/// <param name="x2">The X coordinate of the second point.</param>
		/// <param name="y2">The Y coordinate of the second point.</param>
		public Vector(float x1, float y1, float x2, float y2)
		{
			m_x = x2 - x1;
			m_y = y2 - y1;
		}

		/// <summary>
		/// Creates a vector based on the difference between the two given points.
		/// </summary>
		/// <param name="p1">The first point.</param>
		/// <param name="p2">The second offset point.</param>
		public Vector(Point p1, Point p2)
		{
			m_x = p2.X - p1.X;
			m_y = p2.Y - p1.Y;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="vector">The vector to copy.</param>
		public Vector(Vector vector)
		{
			if (vector != null)
			{
				m_x = vector.m_x;
				m_y = vector.m_y;
			}
			else
			{
				m_x = m_y = 0;
			}
		}

		#region Static Constructors

		/// <summary>
		/// Creates a new vector based on the given length and direction (in radians).
		/// </summary>
		/// <param name="directionRadians">The direction of the vector in radians.</param>
		/// <param name="length">The length of the vector.</param>
		/// <returns>The newly created vector.</returns>
		public static Vector FromDirection(float directionRadians, float length)
		{
			Vector vec = new Vector(directionRadians);
			vec.Length = length;
			return vec;
		}

		/// <summary>
		/// Creates a new vector based on the given length and direction (in degrees).
		/// </summary>
		/// <param name="directionDeg">The direction of the vector in degrees.</param>
		/// <param name="length">The length of the vector.</param>
		/// <returns>The newly created vector.</returns>
		public static Vector FromDirection(int directionDeg, float length)
		{
			Vector vec = new Vector(directionDeg);
			vec.Length = length;
			return vec;
		}

		/// <summary>
		/// Creates a new vector based on the given length and direction (in degrees) with a length of 1.
		/// </summary>
		/// <param name="directionDeg">The direction of the vector in degrees.</param>
		/// <returns>The newly created vector.</returns>
		public static Vector FromDirection(int directionDeg)
		{
			Vector vec = new Vector(directionDeg);
			vec.Length = 1;
			return vec;
		}

		/// <summary>
		/// Creates a new vector based on the given direction (in radians) with a length of 1.
		/// </summary>
		/// <param name="directionRadians">The direction of the vector in radians.</param>
		/// <returns>The newly created vector.</returns>
		public static Vector FromDirection(float directionRadians)
		{
			Vector vec = new Vector(directionRadians);
			vec.Length = 1;
			return vec;
		}

		#endregion Static Constructors

		#endregion Constructors

		#region Operators

		/// <summary>
		/// Converts to String
		/// </summary>
		/// <returns>
		/// A string containing something like "X, Y".
		/// </returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "{0}, {1}", X.ToString("#.000", CultureInfo.CurrentCulture), Y.ToString("#.000", CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>If true, objects are equal</returns>
		public override bool Equals(object obj)
		{
			return (obj is Vector) ? (this == (Vector)obj) : false;
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator== (Vector c1, Vector c2)
		{
			if(object.ReferenceEquals(c1,c2))
				return true;
			else if(object.ReferenceEquals(c1, null) || object.ReferenceEquals(c2, null))
				return false;
			return ((c1.m_x == c2.m_x) && (c1.m_y == c2.m_y));
		}

		/// <summary>
		/// Not equal operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator!= (Vector c1, Vector c2)
		{
			return !(c1 == c2);
		}

		/// <summary>
		/// Greater than or equals operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator >= (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return (c1.X >= c2.X) && (c1.Y >= c2.Y);
		}
		/// <summary>
		/// Greater than operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator > (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return (c1.X > c2.X) && (c1.Y > c2.Y);
		}
		/// <summary>
		/// Less than operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator < (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return (c1.X < c2.X) && (c1.Y < c2.Y);
		}
		/// <summary>
		/// Less than or equals operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator <= (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return (c1.X <= c2.X) && (c1.Y <= c2.Y);
		}

		/// <summary>
		/// Addition operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Vector operator + (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return new Vector(c1.m_x + c2.m_x, c1.m_y + c2.m_y);
		}

		/// <summary>
		/// Minus operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Vector operator - (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return new Vector(c1.m_x - c2.m_x, c1.m_y - c2.m_y);
		}

		/// <summary>
		/// Multiplication operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Vector operator * (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return new Vector(c1.m_x * c2.m_x, c1.m_y * c2.m_y);
		}

		/// <summary>
		/// Division operator
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Vector operator / (Vector c1, Vector c2)
		{
			if (c1 == null)
			{
				throw new ArgumentNullException("c1");
			}
			if (c2 == null)
			{
				throw new ArgumentNullException("c2");
			}
			return new Vector(c1.m_x / c2.m_x, c1.m_y / c2.m_y);
		}

		/// <summary>
		/// Addition operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator +(Vector vector, float scalar)
		{
			return new Vector(vector.m_x + scalar, vector.m_y + scalar);
		}

		/// <summary>
		/// Addition operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector Add(Vector vector, float scalar)
		{
			return new Vector(vector.m_x + scalar, vector.m_y + scalar);
		}
		/// <summary>
		/// Minus operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator -(Vector vector, float scalar)
		{
			return new Vector(vector.m_x - scalar, vector.m_y - scalar);
		}

		/// <summary>
		/// Minus operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector Subtract(Vector vector, float scalar)
		{
			return new Vector(vector.m_x - scalar, vector.m_y - scalar);
		}
		/// <summary>
		/// Multiplication operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator *(Vector vector, float scalar)
		{
			return new Vector(vector.m_x * scalar, vector.m_y * scalar);
		}

		/// <summary>
		/// Multiplication operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector Multiply(Vector vector, float scalar)
		{
			return new Vector(vector.m_x * scalar, vector.m_y * scalar);
		}

		/// <summary>
		/// Division operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator /(Vector vector, float scalar)
		{
			return new Vector(vector.m_x / scalar, vector.m_y / scalar);
		}

		/// <summary>
		/// Division operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector Divide(Vector vector, float scalar)
		{
			return new Vector(vector.m_x / scalar, vector.m_y / scalar);
		}
		/// <summary>
		/// Addition operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator +(float scalar, Vector vector)
		{
			return new Vector(scalar + vector.m_x, scalar + vector.m_y);
		}
		/// <summary>
		/// Minus operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator -(float scalar, Vector vector)
		{
			return new Vector(scalar - vector.m_x, scalar - vector.m_y);
		}
		/// <summary>
		/// Muliplication operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator *(float scalar, Vector vector)
		{
			return new Vector(scalar * vector.m_x, scalar * vector.m_y);
		}
		/// <summary>
		/// Division operator
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Vector operator /(float scalar, Vector vector)
		{
			return new Vector(scalar / vector.m_x, scalar / vector.m_y);
		}

		/// <summary>
		/// Gets the hash code used by the vector.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (int)m_x ^ (int)m_y;
		}
		#endregion

		#region Properties

		/// <summary>
		/// The x coordinate
		/// </summary>
		private float m_x = 0.00001F;
		/// <summary>
		/// The y coordinate
		/// </summary>
		private float m_y = 0.00001F;

		/// <summary>
		/// Contains the x coordinate of the vector.
		/// </summary>
		public float X
		{
			get 
			{ 
				return m_x; 
			}
			set 
			{ 
				m_x = value; 
			}
		}

		/// <summary>
		/// Contains the y coordinate of the vector.
		/// </summary>
		public float Y
		{
			get 
			{ 
				return m_y; 
			}
			set 
			{ 
				m_y = value; 
			}
		}

		/// <summary>
		/// Gets and sets the length of the vector.
		/// </summary>
		public float Length
		{
			get
			{
				return (float)(Math.Sqrt(m_x * m_x + m_y * m_y));
			}
			set
			{
				float direction = this.Direction;
				m_x = (float)(Math.Cos(direction) * value);
				m_y = (float)(Math.Sin(direction) * value);
			}
		}

		/// <summary>
		/// Gets and sets the direction of the vector, in radians.
		/// </summary>
		public float Direction
		{
			get
			{
				return (float)Math.Atan2(m_y, m_x);
			}
			set
			{
				float length = this.Length;
				m_x = (float)(Math.Cos(value) * length);
				m_y = (float)(Math.Sin(value) * length);
			}
		}

		/// <summary>
		/// Gets and sets the direction of the vector, in degrees.
		/// </summary>
		public float DirectionDeg
		{
			get
			{
				return (float)(this.Direction * 180 / Math.PI);
			}
			set
			{
				this.Direction = (float)(value * Math.PI / 180);
			}
		}


		/// <summary>
		/// Returns true if all coordinates are equal to zero.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return (m_x == 0.00001 && m_y == 0.00001);
			}
		}

		/// <summary>
		/// Gets and sets the vectors x and y points using integers.
		/// </summary>
		public Point Point
		{
			get
			{
				return new Point((int)m_x, (int)m_y);
			}
			set
			{
				m_x = value.X;
				m_y = value.Y;
			}
		}

		/// <summary>
		/// Offsets the vector by the given x, y and z coordinates.
		/// </summary>
		/// <param name="offsetX"></param>
		/// <param name="offsetY"></param>
		public void Offset(float offsetX, float offsetY)
		{
			this.m_x += offsetX;
			this.m_y += offsetY;
		}
		#endregion

		#region Math

		/// <summary>
		/// Gets the dot product of the current vector along with the given one.
		/// </summary>	
		/// <param name="other">The other vector to use when getting the dot product.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public float DotProduct(Vector other) 
		{
			return (m_x * other.m_x) + (m_y * other.m_y);
		}

		/// <summary>
		/// Gets the midpoint between the two vectors.
		/// </summary>
		/// <param name="vector">The other vector to compare this one to.</param>
		/// <returns>A new vector representing the midpoint between the two vectors.</returns>
		public Vector Midpoint(Vector vector)
		{
			if (vector == null)
			{
				throw new ArgumentNullException("vector");
			}
			return new Vector(( m_x + vector.X ) * 0.5, ( m_y + vector.Y ) * 0.5 );
		}

		/// <summary>
		/// Normalizes the vector.
		/// </summary>
		/// <returns>The original length.</returns>
		public float Normalize()
		{
			float length = this.Length;
			float invLength = (float)(1.0 / length);
			m_x = m_x * invLength;
			m_y = m_y * invLength;
			return length;
		}
		
		/// <summary>
		/// Returns a new vector equal to the normalized version of this one.
		/// </summary>
		/// <returns>
		/// A new vector representing the normalized vector.
		/// </returns>
		public Vector Normalized()
		{
			Vector ret = new Vector(this);
			ret.Normalize();
			return ret;
		}

		/// <summary>
		/// Inverts the vector.
		/// </summary>
		public void Invert()
		{
			m_x*=-1;
			m_y*=-1;
		}

		/// <summary>
		/// Calculates the reflection angle of the current 
		/// vector using the given normal vector.
		/// </summary>
		/// <param name="normal">
		/// The normal angle.
		/// </param>
		/// <returns>
		/// A new vector representing the reflection angle.
		/// </returns>
		/// <remarks>
		/// Make sure the length of the vector is 1 or it will 
		/// have an effect on the resulting vector.
		/// </remarks>
		public Vector Reflection(Vector normal)
		{
			return this - ( 2 * this.DotProduct(normal) * normal );
		}

		/// <summary>
		/// Calculates the reflection angle of the current 
		/// vector using the given normal in degrees.
		/// </summary>
		/// <param name="normalDeg">
		/// The normal angle in degrees.
		/// </param>
		/// <returns>
		/// A new vector representing the reflection angle.
		/// </returns>
		public Vector Reflection(int normalDeg)
		{
			Vector vecNormal = new Vector(0, 1);
			vecNormal.DirectionDeg = normalDeg;
			return Reflection(vecNormal);
		}

		/// <summary>
		/// Calculates the reflection angle of the current vector 
		/// using the given normal in radians.
		/// </summary>
		/// <param name="normalRadians">
		/// The normal angle in radians.
		/// </param>
		/// <returns>
		/// A new vector representing the reflection angle.
		/// </returns>
		public Vector Reflection(float normalRadians)
		{
			Vector vecNormal = new Vector(0, 1);
			vecNormal.Direction = normalRadians;
			return Reflection(vecNormal);
		}

		#endregion Math

		#region Interface Members
		#region ICloneable Members

		/// <summary>
		/// Clones the base object vector.
		/// </summary>
		/// <returns>A new instance with the same values.</returns>
		public Object Clone()
		{
			return new Vector(this);
		}

		#endregion ICloneable Members
		#region ISerializable Members
		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Vector(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			//Get the values from info and assign them to the appropriate properties
			m_x = (float)info.GetValue("x", typeof(float));
			m_y = (float)info.GetValue("y", typeof(float));
		}

		/// <summary>
		/// Serialization function
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param> 
		[SecurityPermissionAttribute(
			 SecurityAction.Demand, SerializationFormatter=true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			//You can use any custom name for your name-value pair. But make sure you
			// read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
			// then you should read the same with "EmployeeId"
			info.AddValue("x", m_x);
			info.AddValue("y", m_y);
		}
		#endregion ISerializable Members
		#region IComparable Members

		/// <summary>
		/// IComparable.CompareTo implementation.
		/// </summary>
		/// <param name="obj">
		/// The object to compare to.
		/// </param>
		/// <returns>
		/// -1 if its length is less then obj, 0 if it's equal and 1 if it's greater.
		/// </returns>
		public int CompareTo(object obj)
		{
			Vector temp = (Vector)obj;
			return Length.CompareTo(temp.Length);
		}

		#endregion IComparable Members
		#endregion Interface Members
	}
}
