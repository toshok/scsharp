#region License
/*
MIT License
Copyright ©2003-2005 Tao Framework Team
http://www.taoframework.com
All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

#region Original Credits / License
/*
 * (c) Copyright 1993, Silicon Graphics, Inc.
 * ALL RIGHTS RESERVED 
 * Permission to use, copy, modify, and distribute this software for 
 * any purpose and without fee is hereby granted, provided that the above
 * copyright notice appear in all copies and that both the copyright notice
 * and this permission notice appear in supporting documentation, and that 
 * the name of Silicon Graphics, Inc. not be used in advertising
 * or publicity pertaining to distribution of the software without specific,
 * written prior permission. 
 *
 * THE MATERIAL EMBODIED ON THIS SOFTWARE IS PROVIDED TO YOU "AS-IS"
 * AND WITHOUT WARRANTY OF ANY KIND, EXPRESS, IMPLIED OR OTHERWISE,
 * INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL SILICON
 * GRAPHICS, INC.  BE LIABLE TO YOU OR ANYONE ELSE FOR ANY DIRECT,
 * SPECIAL, INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY
 * KIND, OR ANY DAMAGES WHATSOEVER, INCLUDING WITHOUT LIMITATION,
 * LOSS OF PROFIT, LOSS OF USE, SAVINGS OR REVENUE, OR THE CLAIMS OF
 * THIRD PARTIES, WHETHER OR NOT SILICON GRAPHICS, INC.  HAS BEEN
 * ADVISED OF THE POSSIBILITY OF SUCH LOSS, HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, ARISING OUT OF OR IN CONNECTION WITH THE
 * POSSESSION, USE OR PERFORMANCE OF THIS SOFTWARE.
 * 
 * US Government Users Restricted Rights 
 * Use, duplication, or disclosure by the Government is subject to
 * restrictions set forth in FAR 52.227.19(c)(2) or subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013 and/or in similar or successor
 * clauses in the FAR or the DOD or NASA FAR Supplement.
 * Unpublished-- rights reserved under the copyright laws of the
 * United States.  Contractor/manufacturer is Silicon Graphics,
 * Inc., 2011 N.  Shoreline Blvd., Mountain View, CA 94039-7311.
 *
 * OpenGL(TM) is a trademark of Silicon Graphics, Inc.
 */
#endregion Original Credits / License

namespace SdlDotNet.Examples.RedBook
{
	#region Public Structs
	/// <summary>
	/// 
	/// </summary>
	public struct JitterPoint 
	{
		/// <summary>
		/// 
		/// </summary>
		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}
		float x;
		/// <summary>
		/// 
		/// </summary>
		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		float y;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public JitterPoint(float x, float y) 
		{
			this.x = x;
			this.y = y;
		}
	};
	#endregion Public Structs

	#region Class Documentation
	/// <summary>
	///     <para>
	///         Contains jitter point arrays for 2, 3, 4, 8, 15, 24 and 66 jitters.  The arrays
	///         are named j2, j3, etc. Each element in the array has the form, for example,
	///         j8[0].x and j8[0].y.
	///     </para>
	///     <para>
	///         Values are floating point in the range [-0.5 &lt; x &lt; 0.5],
	///         [-0.5 &lt; y &lt; 0.5], and have a gaussian distribution around the origin.
	///     </para>
	///     <para>
	///         Use these to do model jittering for scene anti-aliasing and view volume jittering
	///         for depth of field effects.  Use in conjunction with the accwindow() routine.
	///     </para>
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/jitter.h
	///     </para>
	///     <para>
	///         C# Implementation:  Randy Ridge
	///         http://www.taoframework.com
	///     </para>
	/// </remarks>
	#endregion Class Documentation
    public sealed class Jitter 
	{
        // --- Fields ---
        #region Public Constants
		/// <summary>
		/// 
		/// </summary>
        public const int MaxSamples = 66;
        #endregion Public Constants

        #region Public Fields
        /// <summary>
        ///     <para>
        ///         2 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J2 = {
            new JitterPoint( 0.246490f,  0.249999f),
            new JitterPoint(-0.246490f, -0.249999f)
        };

        /// <summary>
        ///     <para>
        ///         3 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J3 = {
            new JitterPoint(-0.373411f, -0.250550f),
            new JitterPoint( 0.256263f,  0.368119f),
            new JitterPoint( 0.117148f, -0.117570f)
        };

        /// <summary>
        ///     <para>
        ///         4 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J4 = {
            new JitterPoint(-0.208147f,  0.353730f),
            new JitterPoint( 0.203849f, -0.353780f),
            new JitterPoint(-0.292626f, -0.149945f),
            new JitterPoint( 0.296924f,  0.149994f)
        };

        /// <summary>
        ///     <para>
        ///         8 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J8 = {
            new JitterPoint(-0.334818f,  0.435331f),
            new JitterPoint( 0.286438f, -0.393495f),
            new JitterPoint( 0.459462f,  0.141540f),
            new JitterPoint(-0.414498f, -0.192829f),
            new JitterPoint(-0.183790f,  0.082102f),
            new JitterPoint(-0.079263f, -0.317383f),
            new JitterPoint( 0.102254f,  0.299133f),
            new JitterPoint( 0.164216f, -0.054399f)
        };

        /// <summary>
        ///     <para>
        ///         15 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J15 = {
            new JitterPoint( 0.285561f,  0.188437f),
            new JitterPoint( 0.360176f, -0.065688f),
            new JitterPoint(-0.111751f,  0.275019f),
            new JitterPoint(-0.055918f, -0.215197f),
            new JitterPoint(-0.080231f, -0.470965f),
            new JitterPoint( 0.138721f,  0.409168f),
            new JitterPoint( 0.384120f,  0.458500f),
            new JitterPoint(-0.454968f,  0.134088f),
            new JitterPoint( 0.179271f, -0.331196f),
            new JitterPoint(-0.307049f, -0.364927f),
            new JitterPoint( 0.105354f, -0.010099f),
            new JitterPoint(-0.154180f,  0.021794f),
            new JitterPoint(-0.370135f, -0.116425f),
            new JitterPoint( 0.451636f, -0.300013f),
            new JitterPoint(-0.370610f,  0.387504f)
        };

        /// <summary>
        ///     <para>
        ///         24 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J24 = {
            new JitterPoint( 0.030245f,  0.136384f),
            new JitterPoint( 0.018865f, -0.348867f),
            new JitterPoint(-0.350114f, -0.472309f),
            new JitterPoint( 0.222181f,  0.149524f),
            new JitterPoint(-0.393670f, -0.266873f),
            new JitterPoint( 0.404568f,  0.230436f),
            new JitterPoint( 0.098381f,  0.465337f),
            new JitterPoint( 0.462671f,  0.442116f),
            new JitterPoint( 0.400373f, -0.212720f),
            new JitterPoint(-0.409988f,  0.263345f),
            new JitterPoint(-0.115878f, -0.001981f),
            new JitterPoint( 0.348425f, -0.009237f),
            new JitterPoint(-0.464016f,  0.066467f),
            new JitterPoint(-0.138674f, -0.468006f),
            new JitterPoint( 0.144932f, -0.022780f),
            new JitterPoint(-0.250195f,  0.150161f),
            new JitterPoint(-0.181400f, -0.264219f),
            new JitterPoint( 0.196097f, -0.234139f),
            new JitterPoint(-0.311082f, -0.078815f),
            new JitterPoint( 0.268379f,  0.366778f),
            new JitterPoint(-0.040601f,  0.327109f),
            new JitterPoint(-0.234392f,  0.354659f),
            new JitterPoint(-0.003102f, -0.154402f),
            new JitterPoint( 0.297997f, -0.417965f)
        };

        /// <summary>
        ///     <para>
        ///         66 jitter points.
        ///     </para>
        /// </summary>
        public static readonly JitterPoint[] J66 = {
            new JitterPoint( 0.266377f, -0.218171f),
            new JitterPoint(-0.170919f, -0.429368f),
            new JitterPoint( 0.047356f, -0.387135f),
            new JitterPoint(-0.430063f,  0.363413f),
            new JitterPoint(-0.221638f, -0.313768f),
            new JitterPoint( 0.124758f, -0.197109f),
            new JitterPoint(-0.400021f,  0.482195f),
            new JitterPoint( 0.247882f,  0.152010f),
            new JitterPoint(-0.286709f, -0.470214f),
            new JitterPoint(-0.426790f,  0.004977f),
            new JitterPoint(-0.361249f, -0.104549f),
            new JitterPoint(-0.040643f,  0.123453f),
            new JitterPoint(-0.189296f,  0.438963f),
            new JitterPoint(-0.453521f, -0.299889f),
            new JitterPoint( 0.408216f, -0.457699f),
            new JitterPoint( 0.328973f, -0.101914f),
            new JitterPoint(-0.055540f, -0.477952f),
            new JitterPoint( 0.194421f,  0.453510f),
            new JitterPoint( 0.404051f,  0.224974f),
            new JitterPoint( 0.310136f,  0.419700f),
            new JitterPoint(-0.021743f,  0.403898f),
            new JitterPoint(-0.466210f,  0.248839f),
            new JitterPoint( 0.341369f,  0.081490f),
            new JitterPoint( 0.124156f, -0.016859f),
            new JitterPoint(-0.461321f, -0.176661f),
            new JitterPoint( 0.013210f,  0.234401f),
            new JitterPoint( 0.174258f, -0.311854f),
            new JitterPoint( 0.294061f,  0.263364f),
            new JitterPoint(-0.114836f,  0.328189f),
            new JitterPoint( 0.041206f, -0.106205f),
            new JitterPoint( 0.079227f,  0.345021f),
            new JitterPoint(-0.109319f, -0.242380f),
            new JitterPoint( 0.425005f, -0.332397f),
            new JitterPoint( 0.009146f,  0.015098f),
            new JitterPoint(-0.339084f, -0.355707f),
            new JitterPoint(-0.224596f, -0.189548f),
            new JitterPoint( 0.083475f,  0.117028f),
            new JitterPoint( 0.295962f, -0.334699f),
            new JitterPoint( 0.452998f,  0.025397f),
            new JitterPoint( 0.206511f, -0.104668f),
            new JitterPoint( 0.447544f, -0.096004f),
            new JitterPoint(-0.108006f, -0.002471f),
            new JitterPoint(-0.380810f,  0.130036f),
            new JitterPoint(-0.242440f,  0.186934f),
            new JitterPoint(-0.200363f,  0.070863f),
            new JitterPoint(-0.344844f, -0.230814f),
            new JitterPoint( 0.408660f,  0.345826f),
            new JitterPoint(-0.233016f,  0.305203f),
            new JitterPoint( 0.158475f, -0.430762f),
            new JitterPoint( 0.486972f,  0.139163f),
            new JitterPoint(-0.301610f,  0.009319f),
            new JitterPoint( 0.282245f, -0.458671f),
            new JitterPoint( 0.482046f,  0.443890f),
            new JitterPoint(-0.121527f,  0.210223f),
            new JitterPoint(-0.477606f, -0.424878f),
            new JitterPoint(-0.083941f, -0.121440f),
            new JitterPoint(-0.345773f,  0.253779f),
            new JitterPoint( 0.234646f,  0.034549f),
            new JitterPoint( 0.394102f, -0.210901f),
            new JitterPoint(-0.312571f,  0.397656f),
            new JitterPoint( 0.200906f,  0.333293f),
            new JitterPoint( 0.018703f, -0.261792f),
            new JitterPoint(-0.209349f, -0.065383f),
            new JitterPoint( 0.076248f,  0.478538f),
            new JitterPoint(-0.073036f, -0.355064f),
            new JitterPoint( 0.145087f,  0.221726f)
        };
        #endregion Public Fields
    }
}
