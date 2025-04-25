using System;
using System.Collections.Generic;

namespace Othello
{
	public class EdgeStabilityCalculator
	{
		private const int EdgeLength = 8;
		private const int TotalConfigurations = 6561;
		private const int EMPTY = 0;
		private const int BLACK = 1;
		private const int WHITE = 2;

		private int[] stabilityTable = new int[TotalConfigurations];

		public void ComputeStabilityTable()
		{
			for (int i = 0; i < TotalConfigurations; i++)
			{
				int[] config = DecodeConfiguration(i);
				stabilityTable[i] = StaticEvaluation(config);
			}

			bool changed;
			do
			{
				changed = false;
				for (int i = 0; i < TotalConfigurations; i++)
				{
					int[] config = DecodeConfiguration(i);
					if (CountDiscs(config) == 0) continue;

					int bestValue = stabilityTable[i];
					foreach (int move in GetPlayableMoves(config, BLACK))
					{
						int[] newConfig = (int[])config.Clone();
						newConfig[move] = BLACK;
						int newIndex = EncodeConfiguration(newConfig);
						int newValue = stabilityTable[newIndex];
						if (newValue > bestValue)
						{
							bestValue = newValue;
							changed = true;
						}
					}

					stabilityTable[i] = bestValue;
				}
			}
			while (changed);
		}

		private int StaticEvaluation(int[] config)
		{
			int value = 0;
			for (int i = 0; i < EdgeLength; i++)
			{
				int disc = config[i];
				if (disc == EMPTY) continue;

				int weight = GetDiscWeight(i, disc, config);
				value += disc == BLACK ? weight : -weight;
			}
			return value;
		}

		private int GetDiscWeight(int index, int disc, int[] config)
		{
			if (IsCorner(index)) return 700;

			bool isStable = IsStable(index, disc, config);
			bool isSemiStable = IsSemiStable(index, disc, config);
			bool isUnstable = !isStable && !isSemiStable;

			if (IsASquare(index))
			{
				if (isStable) return 1000;
				if (isSemiStable) return 200;
				return 75;
			}
			else if (IsBSquare(index))
			{
				if (isStable) return 1000;
				if (isSemiStable) return 200;
				return 50;
			}
			else if (IsCSquare(index))
			{
				if (isStable) return 1200;
				if (isSemiStable) return 200;
				return -25;
			}

			return 0;
		}

		private int[] DecodeConfiguration(int index)
		{
			int[] config = new int[EdgeLength];
			for (int i = EdgeLength - 1; i >= 0; i--)
			{
				config[i] = index % 3;
				index /= 3;
			}
			return config;
		}

		private int EncodeConfiguration(int[] config)
		{
			int index = 0;
			for (int i = 0; i < EdgeLength; i++)
			{
				index = index * 3 + config[i];
			}
			return index;
		}

		private int CountDiscs(int[] config)
		{
			int count = 0;
			foreach (int cell in config)
				if (cell != EMPTY)
					count++;
			return count;
		}

		private List<int> GetPlayableMoves(int[] config, int player)
		{
			var moves = new List<int>();
			for (int i = 0; i < EdgeLength; i++)
			{
				if (config[i] == EMPTY)
				{
					if (i > 0 && config[i - 1] == 3 - player || i < EdgeLength - 1 && config[i + 1] == 3 - player)
						moves.Add(i);
				}
			}
			return moves;
		}

		public int GetValue(int[] config, bool isBlackTurn)
		{
			int index = EncodeConfiguration(config);
			if (isBlackTurn)
				return stabilityTable[index];
			else
			{
				int[] inverse = InvertColors(config);
				int invIndex = EncodeConfiguration(inverse);
				return -stabilityTable[invIndex];
			}
		}

		private int[] InvertColors(int[] config)
		{
			int[] inverted = new int[EdgeLength];
			for (int i = 0; i < EdgeLength; i++)
			{
				if (config[i] == BLACK) inverted[i] = WHITE;
				else if (config[i] == WHITE) inverted[i] = BLACK;
				else inverted[i] = EMPTY;
			}
			return inverted;
		}


		private bool IsCorner(int i) => i == 0 || i == 7;
		private bool IsCSquare(int i) => i == 1 || i == 6;
		private bool IsASquare(int i) => i == 2 || i == 5;
		private bool IsBSquare(int i) => i == 3 || i == 4;

		private bool IsStable(int i, int disc, int[] config)
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

		private bool IsSemiStable(int i, int disc, int[] config)
		{
			return i > 0 && config[i - 1] == disc || i < EdgeLength - 1 && config[i + 1] == disc;
		}
	}
}