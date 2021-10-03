//
// SCSharp.UI.AStarSolver
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
using System.Collections.Generic;

namespace SCSharp.UI
{
	public class MapPoint
	{
		public MapPoint (int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override String ToString ()
		{
			return string.Format ("({0},{1})", X, Y);
		}

		public override int GetHashCode ()
		{
			return (33 * X) ^ (91 * Y);
		}

		public override bool Equals (object o)
		{
			return this == (MapPoint)o;
		}

		public static bool operator ==(MapPoint a, MapPoint b)
		{
			// If both are null, or both are same instance, return true.
			if (object.ReferenceEquals(a, b))
				return true;

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
				return false;

			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(MapPoint a, MapPoint b)
		{
			return !(a == b);
		}

		public int X { get; private set; }
		public int Y { get; private set; }
	}

	public class AStarSolver
	{
		Dictionary<MapPoint,MapPoint> came_from;
		MapRenderer map;

		public AStarSolver (MapRenderer map)
		{
			came_from = new Dictionary<MapPoint,MapPoint> ();
			this.map = map;
		}

		double dist_between (MapPoint point2, MapPoint point1)
		{
			// euclidian distance
			return Math.Sqrt ((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y));
		}

		double heuristic_estimate_of_distance (MapPoint point, MapPoint goal)
		{
			return Math.Abs(point.X-goal.X) + Math.Abs(point.Y-goal.Y);
		}

		MapPoint FindLowestScoringNode (Dictionary<MapPoint,bool> openset, Dictionary<MapPoint,double> f_score)
		{
			MapPoint current_lowest = null;
			double current_lowest_score = Double.MaxValue;

			foreach (var mp in openset.Keys) {
				if (f_score[mp] < current_lowest_score) {
					current_lowest_score = f_score[mp];
					current_lowest = mp;
				}
			}

			return current_lowest;
		}

		List<MapPoint> NeighborNodes (MapPoint point)
		{
			List<MapPoint> neighbors = new List<MapPoint>();

			// Console.WriteLine ("checking neighbors of {0}", point);
			// Console.WriteLine ("map minitile size = {0}x{1}", map.Chk.Width, map.Chk.Height);

			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					MapPoint p = new MapPoint (x+point.X, y+point.Y);
					// Console.WriteLine ("checking neighbor {0}", p);
					if (x == 0 && y == 0) { // disallow non-moves
						// Console.WriteLine ("no (1)");
						continue;
					}

					if (p.X < 0 || p.X >= (map.Chk.Width << 3)) {
						// Console.WriteLine ("no (2)");
						continue;
					}

					if (p.Y < 0 || p.Y >= (map.Chk.Height << 3)) {
						// Console.WriteLine ("no (3)");
						continue;
					}

					// don't move into walls
					if (!map.Navigable (p)) {
						// Console.WriteLine ("no (4)");
						continue;
					}

					// Console.WriteLine ("adding neighbor of {0}", p);

					neighbors.Add (p);
				}
			}

			return neighbors;
		}

		Dictionary<MapPoint,bool> closedset;  // The set of nodes already evaluated.

		public List<MapPoint> FindPath (MapPoint start, MapPoint goal)
		{
			if (!map.Navigable (start))
				throw new Exception ("is it valid to start in a non-navigable spot?");

			closedset = new Dictionary<MapPoint,bool>();

			var openset = new Dictionary<MapPoint,bool>();  // The set of tentative nodes to be evaluated.
			openset.Add (start, true);

			var g_score = new Dictionary<MapPoint,double>();
			var h_score = new Dictionary<MapPoint,double>();
			var f_score = new Dictionary<MapPoint,double>();

			g_score[start] = 0.0;                      // Distance from start along optimal path.
			h_score[start] = heuristic_estimate_of_distance(start, goal);
			f_score[start] = h_score[start];           // Estimated total distance from start to goal through y.

			while (openset.Keys.Count > 0) {
				var x = FindLowestScoringNode (openset, f_score);
				if (x == goal) {
					Console.WriteLine ("done!");
					return ReconstructPath(goal);
				}

				openset.Remove (x);
				closedset.Add (x, true);

				var neighbors = NeighborNodes(x);
				foreach (var y in neighbors) {
					bool tentative_is_better;

					if (closedset.ContainsKey (y))
						continue;

					var tentative_g_score = g_score[x] + dist_between(x,y);
 
					if (!openset.ContainsKey (y)) {
						openset.Add (y, true);
						tentative_is_better = true;
					}
					else if (tentative_g_score < g_score[y]) {
						tentative_is_better = true;
					}
					else {
						tentative_is_better = false;
					}

					if (tentative_is_better) {
						came_from[y] = x;
						g_score[y] = tentative_g_score;
						h_score[y] = heuristic_estimate_of_distance(y, goal);
						f_score[y] = g_score[y] + h_score[y];
					}
				}
			}
			Console.WriteLine ("failed to find path");
			return null;
		}

		List<MapPoint> ReconstructPath (MapPoint current_node)
		{
			List<MapPoint> path = new List<MapPoint>();

			while (current_node != null) {
				path.Insert (0, current_node);

				if (came_from.ContainsKey (current_node))
					current_node = came_from[current_node];
				else
					current_node = null;
			}

			return path;
		}

#if false
		public static void Main (string[] args) {
			Map m = new Map (50, 50);

			for (int i = 0; i < 25; i ++)
				m.AddWall (new MapPoint (15, i));

			AStarSolver solver = new AStarSolver (m);

			List<MapPoint> path = solver.FindPath (new MapPoint (10, 10), new MapPoint (20, 10));

			Dictionary<MapPoint,bool> path_points = new Dictionary<MapPoint,bool>();
			foreach (var p in path)
				path_points.Add (p, true);

			for (int y = 0; y < 50; y ++) {
				for (int x = 0; x < 50; x ++) {
					MapPoint point = new MapPoint (x,y);
					if (point == new MapPoint (20,10))
						Console.Write ("X");
					else if (path_points.ContainsKey (point))
						Console.Write ("x");
					else if (!m.Navigable (point))
						Console.Write ("O");
					else if (solver.closedset.ContainsKey (point))
						Console.Write (".");
					else
						Console.Write (" ");
				}
				Console.WriteLine();
			}

			// Console.WriteLine ("Path = {{");
			// foreach (MapPoint point in path) {
			// 	Console.WriteLine ("   {0}, {1}", point.X, point.Y);
			// }
			// Console.WriteLine ("}");
		}
#endif
	}
}