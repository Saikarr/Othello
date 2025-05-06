using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public static class Heuristics
	{

		// Positional weights for heuristic evaluation
		private static readonly int[,] positionWeights = new int[8, 8]
		{
			{ 120, -20,  20,   5,   5,  20, -20, 120 },
			{ -20, -40,  -5,  -5,  -5,  -5, -40, -20 },
			{  20,  -5,  15,   3,   3,  15,  -5,  20 },
			{   5,  -5,   3,   3,   3,   3,  -5,   5 },
			{   5,  -5,   3,   3,   3,   3,  -5,   5 },
			{  20,  -5,  15,   3,   3,  15,  -5,  20 },
			{ -20, -40,  -5,  -5,  -5,  -5, -40, -20 },
			{ 120, -20,  20,   5,   5,  20, -20, 120 }
		};
		public static char GetSymbol(Player player)
		{
			return player == Player.Black ? 'B' : 'W';
		}
		public static Player GetOpponent(Player player)
		{
			return player == Player.Black ? Player.White : Player.Black;
		}

		public static int CalculatePieceCount(Board board, Player player) // could be more efficient if counted during makemove
		{
			char playerSymbol = GetSymbol(player);
			char opponentSymbol = GetSymbol(GetOpponent(player));
			int playerCount = 0, opponentCount = 0;

			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					if (board.BoardState[i, j] == playerSymbol) playerCount++;
					else if (board.BoardState[i, j] == opponentSymbol) opponentCount++;
				}

			return playerCount - opponentCount;
		}

		public static float CalculateMobilityImpact(Player player, char[,] boardState, Board board)
		{
			// Count valid moves for player
			int playerMoves = CalculateValidMoves(player, boardState, board);

			// Count valid moves for opponent
			int opponentMoves = CalculateValidMoves(OthelloGame.GetOpponent(player), boardState, board);

			return (playerMoves - opponentMoves) / (float)(playerMoves + opponentMoves + 2);
		}

		// TODO: potential mobility

		public static int CalculateCornerControl(Board board, Player player)
		{
			char playerSymbol = GetSymbol(player);
			char opponentSymbol = GetSymbol(GetOpponent(player));

			(int, int)[] corners = { (0, 0), (0, 7), (7, 0), (7, 7) };
			int playerCorners = 0, opponentCorners = 0;

			foreach (var (x, y) in corners)
			{
				if (board.BoardState[x, y] == playerSymbol) playerCorners++;
				else if (board.BoardState[x, y] == opponentSymbol) opponentCorners++;
			}

			return playerCorners - opponentCorners;
		}

		public static int CalculateInternalStability(Board board, Player player)
		{
			if (player == Player.Black)
			{
				return InternalStability.CalculateInternalStabilityDifference(board);
			}
			else
			{
				return -InternalStability.CalculateInternalStabilityDifference(board);
			}
		}

		public static int CalculateEdgeStability(Player player, Board board)
		{
			char[,] state = board.BoardState;
			int size = state.GetLength(0);

			int Convert(char c)
			{
				return c == '.' ? 0 : c == 'B' ? 1 : 2;
			}

			int[] ExtractEdge(int direction)
			{
				int[] edge = new int[8];
				switch (direction)
				{
					case 0:
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[0, i]);
						break;
					case 1:
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[7, i]);
						break;
					case 2:
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[i, 0]);
						break;
					case 3:
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[i, 7]);
						break;
				}
				return edge;
			}

			int total = 0;
			bool isBlack = player == Player.Black;

			for (int dir = 0; dir < 4; dir++)
			{
				int[] edge = ExtractEdge(dir);
				total += EdgeStability.GetValue(edge, isBlackTurn: isBlack);
			}

			return total;
		}

		// TODO: edge control

		// TODO: center control

		public static int CalculatePositionalWeight(Board board, char[,] boardState, Player player) // could be more efficient if counted during makemove	
		{
			char playerSymbol = board.PlayerSymbols[player];
			char opponentSymbol = board.PlayerSymbols[OthelloGame.GetOpponent(player)];

			int playerScore = 0;
			int opponentScore = 0;

			for (int row = 0; row < board.Size; row++)
			{
				for (int col = 0; col < board.Size; col++)
				{
					if (boardState[row, col] == playerSymbol)
					{
						playerScore += positionWeights[row, col];
					}
					else if (boardState[row, col] == opponentSymbol)
					{
						opponentScore += positionWeights[row, col];
					}
				}
			}

			return playerScore - opponentScore;
		}

		public static int CalculateValidMoves(Player player, char[,] boardState, Board board)
		{
			int count = 0;
			for (int row = 0; row < board.Size; row++)
			{
				for (int col = 0; col < board.Size; col++)
				{
					if (board.IsValidMove(row, col, player, boardState))
					{
						count++;
					}
				}
			}
			return count;
		}

		
		
	}
}
