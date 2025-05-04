using System;
using System.Collections.Generic;

namespace Othello
{
	public static class EdgeStability
	{
		public const int EdgeLength = 8;
		public const int EMPTY = 0;
		public const int BLACK = 1;
		public const int WHITE = 2;


		public static int[] StabilityTable { get; private set; } = new int[6561]; // 3^8

		static EdgeStability()
		{
			ComputeStabilityTable();
		}

		private static void ComputeStabilityTable()
		{
			for (int i = 0; i < StabilityTable.Length; i++)
			{
				int[] config = DecodeConfiguration(i);
				StabilityTable[i] = StaticEvaluation(config);
			}

			bool changed;
			do
			{
				changed = false;
				for (int i = 0; i < StabilityTable.Length; i++)
				{
					int[] config = DecodeConfiguration(i);
					if (CountDiscs(config) == 0) continue;

					int bestValue = StabilityTable[i];
					foreach (int move in GetPlayableMoves(config, BLACK))
					{
						int[] newConfig = (int[])config.Clone();
						newConfig[move] = BLACK;
						int newIndex = EncodeConfiguration(newConfig);
						int newValue = StabilityTable[newIndex];
						if (newValue > bestValue)
						{
							bestValue = newValue;
							changed = true;
						}
					}

					StabilityTable[i] = bestValue;
				}
			}
			while (changed);
		}

		private static int StaticEvaluation(int[] config)
		{
			int value = 0;
			for (int i = 0; i < EdgeLength; i++)
			{
				int disc = config[i];
				if (disc == EMPTY) continue;

				int weight = GetDiscWeight(i, disc, config);
				value += (disc == BLACK) ? weight : -weight;
			}
			return value;
		}

		private static int GetDiscWeight(int index, int disc, int[] config)
		{
			if (IsCorner(index)) return 700;

			bool isStable = IsStable(index, disc, config);
			bool isSemiStable = IsSemiStable(index, disc, config);

			if (IsASquare(index))
			{
				return isStable ? 1000 : isSemiStable ? 200 : 75;
			}
			else if (IsBSquare(index))
			{
				return isStable ? 1000 : isSemiStable ? 200 : 50;
			}
			else if (IsCSquare(index))
			{
				return isStable ? 1200 : isSemiStable ? 200 : -25;
			}

			return 0;
		}

		public static int GetValue(int[] config, bool isBlackTurn)
		{
			int index = EncodeConfiguration(config);
			if (isBlackTurn)
				return StabilityTable[index];
			else
			{
				int[] inverse = InvertColors(config);
				int invIndex = EncodeConfiguration(inverse);
				return -StabilityTable[invIndex];
			}
		}


		private static int[] DecodeConfiguration(int index)
		{
			int[] config = new int[EdgeLength];
			for (int i = EdgeLength - 1; i >= 0; i--)
			{
				config[i] = index % 3;
				index /= 3;
			}
			return config;
		}

		private static int EncodeConfiguration(int[] config)
		{
			int index = 0;
			for (int i = 0; i < EdgeLength; i++)
			{
				index = index * 3 + config[i];
			}
			return index;
		}

		private static int CountDiscs(int[] config)
		{
			int count = 0;
			foreach (int cell in config)
				if (cell != EMPTY)
					count++;
			return count;
		}

		private static List<int> GetPlayableMoves(int[] config, int player)
		{
			var moves = new List<int>();
			for (int i = 0; i < EdgeLength; i++)
			{
				if (config[i] == EMPTY)
				{
					if ((i > 0 && config[i - 1] == 3 - player) || (i < EdgeLength - 1 && config[i + 1] == 3 - player))
						moves.Add(i);
				}
			}
			return moves;
		}

		private static int[] InvertColors(int[] config)
		{
			int[] inverted = new int[EdgeLength];
			for (int i = 0; i < EdgeLength; i++)
			{
				inverted[i] = config[i] == BLACK ? WHITE :
							  config[i] == WHITE ? BLACK : EMPTY;
			}
			return inverted;
		}

		private static bool IsCorner(int i) => i == 0 || i == 7;
		private static bool IsCSquare(int i) => i == 1 || i == 6;
		private static bool IsASquare(int i) => i == 2 || i == 5;
		private static bool IsBSquare(int i) => i == 3 || i == 4;

		private static bool IsStable(int i, int disc, int[] config)
		{
			if (IsCorner(i)) return true;
			if (i < 4)
			{
				for (int j = i; j >= 0; j--)
					if (config[j] != disc) return false;
			}
			else
			{
				for (int j = i; j < EdgeLength; j++)
					if (config[j] != disc) return false;
			}
			return true;
		}

		private static bool IsSemiStable(int i, int disc, int[] config)
		{
			return (i > 0 && config[i - 1] == disc) || (i < EdgeLength - 1 && config[i + 1] == disc);
		}
	}
}