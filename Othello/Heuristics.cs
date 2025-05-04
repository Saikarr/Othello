using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public static class Heuristics // TODO: fix and add heuristics (these arent really functional, they have to evaluate the state of the board not the move)
	{
		public static int CountCorners(Player player, char[,] boardState, Board board)
		{
			char playerSymbol = board.PlayerSymbols[player];
			int count = 0;

			// Check all four corners
			if (boardState[0, 0] == playerSymbol) count++;
			if (boardState[0, 7] == playerSymbol) count++;
			if (boardState[7, 0] == playerSymbol) count++;
			if (boardState[7, 7] == playerSymbol) count++;

			return count;
		}

		public static int CountPiecesFlipped(int row, int col, Player player, char[,] boardState, Board board)
		{
			char playerSymbol = board.PlayerSymbols[player];
			char opponentSymbol = board.PlayerSymbols[OthelloGame.GetOpponent(player)];
			int totalFlipped = 0;

			for (int rowDir = -1; rowDir <= 1; rowDir++)
			{
				for (int colDir = -1; colDir <= 1; colDir++)
				{
					if (rowDir == 0 && colDir == 0) continue;

					int r = row + rowDir;
					int c = col + colDir;
					int flippedInDirection = 0;

					while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && boardState[r, c] == opponentSymbol)
					{
						flippedInDirection++;
						r += rowDir;
						c += colDir;
					}

					if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && boardState[r, c] == playerSymbol)
					{
						totalFlipped += flippedInDirection;
					}
				}
			}

			return totalFlipped;
		}

		public static float CalculateMobilityImpact(int row, int col, Player player, char[,] boardState, Board board)
		{
			// Make a copy of the board to simulate the move
			char[,] tempBoard = (char[,])boardState.Clone();
			tempBoard[row, col] = board.PlayerSymbols[player];
			board.MakeMove(row, col, player, tempBoard);

			// Count valid moves for player after this move
			int playerMoves = CountValidMoves(player, tempBoard, board);

			// Count valid moves for opponent after this move
			int opponentMoves = CountValidMoves(OthelloGame.GetOpponent(player), tempBoard, board);

			return (playerMoves-opponentMoves)/(float)(playerMoves+opponentMoves+2);
		}

		public static int CountValidMoves(Player player, char[,] boardState, Board board)
		{
			int count = 0;
			for (int row = 0; row < board.Size; row++)
			{
				for (int col = 0; col < board.Size; col++)
				{
					if (board.IsValidMove(row, col, player, boardState)) //TODO: fix it
					{
						count++;
					}
				}
			}
			return count;
		}

		public static int CalculateEdgeStability(int row, int col, Player player, Board board)
		{
			char[,] state = board.BoardState;
			int size = state.GetLength(0);

			// Convert board edge to 0=empty, 1=black, 2=white
			int Convert(char c)
			{
				return c == '.' ? 0 : c == 'B' ? 1 : 2;
			}

			int[] ExtractEdge(int direction)
			{
				int[] edge = new int[8];
				switch (direction)
				{
					case 0: // Top row
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[0, i]);
						break;
					case 1: // Bottom row
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[7, i]);
						break;
					case 2: // Left column
						for (int i = 0; i < 8; i++)
							edge[i] = Convert(state[i, 0]);
						break;
					case 3: // Right column
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
	}
}
