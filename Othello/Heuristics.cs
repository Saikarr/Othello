using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public static class Heuristics // TODO: fix and add heuristics (these arent really functional, they have to evaluate the state of the board not the move)
	{
		public static char GetSymbol(Player player)
		{
			return player == Player.Black ? 'B' : 'W';
		}
		public static Player GetOpponent(Player player)
		{
			return player == Player.Black ? Player.White : Player.Black;
		}



		public static int CalculatePieceCount(Board board, Player player)
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

			return 25 * (playerCorners - opponentCorners);
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
					if (board.IsValidMove(row, col, player, boardState)) 
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
		public static int CalculateInternalStability(Board board,Player player)
		{
			if (player==Player.Black)return InternalStability.CalculateInternalStabilityDifference(board);
			else return -InternalStability.CalculateInternalStabilityDifference(board);
		}
	}
}
