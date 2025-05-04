namespace Othello
{
	using System;
	using System.Collections.Generic;

	public static class InternalStability
	{
		private static readonly (int dr, int dc)[] Directions = new (int, int)[]
		{
		(-1, 0), (1, 0),    // Vertical
        (0, -1), (0, 1),    // Horizontal
        (-1, -1), (1, 1),   // Diagonal \
        (-1, 1), (1, -1)    // Diagonal /
		};

		public static int CalculateInternalStabilityDifference(Board board)
		{
			char[,] state = board.BoardState;
			int size = state.GetLength(0);
			bool[,] stable = new bool[size, size];
			Queue<(int, int)> queue = new();

			// Step 1 & 2: Initialize corners
			foreach (var (r, c) in new[] { (0, 0), (0, 7), (7, 0), (7, 7) })
			{
				if (state[r, c] != '.')
				{
					stable[r, c] = true;
					queue.Enqueue((r, c));
				}
			}

			// Step 3: Propagate stability
			while (queue.Count > 0)
			{
				var (r, c) = queue.Dequeue();
				char color = state[r, c];

				foreach (var (dr, dc) in Directions)
				{
					int nr = r + dr, nc = c + dc;
					if (!InBounds(nr, nc, size) || state[nr, nc] != color || stable[nr, nc])
						continue;

					if (IsStableInAllFourLines(nr, nc, color, stable, state, size))
					{
						stable[nr, nc] = true;
						queue.Enqueue((nr, nc));
					}
				}
			}

			// Step 4: Compute score difference
			int blackStable = 0, whiteStable = 0;
			for (int r = 0; r < size; r++)
			{
				for (int c = 0; c < size; c++)
				{
					if (stable[r, c])
					{
						if (state[r, c] == 'B') blackStable++;
						else if (state[r, c] == 'W') whiteStable++;
					}
				}
			}

			return blackStable - whiteStable;
		}

		private static bool IsStableInAllFourLines(int r, int c, char color, bool[,] stable, char[,] state, int size)
		{
			// Check stability along 4 directions: vertical, horizontal, two diagonals
			return
				HasStableOrEdge(r, c, -1, 0, color, stable, state, size) && // up
				HasStableOrEdge(r, c, 1, 0, color, stable, state, size) &&  // down
				HasStableOrEdge(r, c, 0, -1, color, stable, state, size) && // left
				HasStableOrEdge(r, c, 0, 1, color, stable, state, size);    // right
		}

		private static bool HasStableOrEdge(int r, int c, int dr, int dc, char color, bool[,] stable, char[,] state, int size)
		{
			int nr = r + dr, nc = c + dc;
			while (InBounds(nr, nc, size))
			{
				if (state[nr, nc] == color && stable[nr, nc])
					return true;
				else if (state[nr, nc] != color)
					return false;
				nr += dr;
				nc += dc;
			}
			return true; // Reached edge with no break in same-colored line
		}

		private static bool InBounds(int r, int c, int size)
		{
			return r >= 0 && c >= 0 && r < size && c < size;
		}
	}
}