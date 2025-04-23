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

		public static int CalculateMobilityImpact(int row, int col, Player player, char[,] boardState, Board board)
		{
			// Make a copy of the board to simulate the move
			char[,] tempBoard = (char[,])boardState.Clone();
			tempBoard[row, col] = board.PlayerSymbols[player];
			board.MakeMove(row, col, player, tempBoard);

			// Count valid moves for player after this move
			int playerMoves = CountValidMoves(player, tempBoard, board);

			// Count valid moves for opponent after this move
			int opponentMoves = CountValidMoves(OthelloGame.GetOpponent(player), tempBoard, board);

			return playerMoves - opponentMoves;
		}

		public static int CountValidMoves(Player player, char[,] boardState, Board board)
		{
			int count = 0;
			for (int row = 0; row < board.Size; row++)
			{
				for (int col = 0; col < board.Size; col++)
				{
					//if (IsValidMove(row, col, player, boardState)) //TODO: fix it
					//{
					//	count++;
					//}
				}
			}
			return count;
		}

		public static int CalculateStability(int row, int col, Player player)
		{
			// Simple stability measure - corners are most stable, edges next, center least
			if ((row == 0 || row == 7) && (col == 0 || col == 7))
				return 10; // Corner
			else if (row == 0 || row == 7 || col == 0 || col == 7)
				return 5;  // Edge
			else
				return 1;  // Center
		}
	}
}
