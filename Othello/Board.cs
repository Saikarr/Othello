using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public class Board
	{
		public int Size { get; private set; }
		public char[,] BoardState;
		public Dictionary<Player, char> PlayerSymbols;
		private OthelloGame Game;
		public Board(int size, OthelloGame game)
		{
			Size = size;
			BoardState = new char[Size, Size];
			InitializeBoard();
			PlayerSymbols = new Dictionary<Player, char>
			{
				{ Player.Black, 'B' },
				{ Player.White, 'W' }
			};
			Game = game;
		}

		private void InitializeBoard()
		{
			// Initialize empty board
			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					BoardState[row, col] = '.';
				}
			}

			// Set up starting pieces
			BoardState[3, 3] = 'W';
			BoardState[3, 4] = 'B';
			BoardState[4, 3] = 'B';
			BoardState[4, 4] = 'W';
		}

		public bool IsValidMove(int row, int col, Player player, char[,]? boardState = null)
		{
			if (boardState == null)
			{
				boardState = BoardState;
			}
			// Check if position is on the board
			if (row < 0 || row >= Size || col < 0 || col >= Size)
				return false;

			// Check if position is empty
			if (boardState[row, col] != '.')
				return false;

			char playerSymbol = PlayerSymbols[player];
			char opponentSymbol = PlayerSymbols[OthelloGame.GetOpponent(player)];

			// Check all 8 directions
			for (int rowDir = -1; rowDir <= 1; rowDir++)
			{
				for (int colDir = -1; colDir <= 1; colDir++)
				{
					if (rowDir == 0 && colDir == 0) continue; // Skip current position

					int r = row + rowDir;
					int c = col + colDir;
					bool foundOpponent = false;

					while (r >= 0 && r < Size && c >= 0 && c < Size)
					{
						if (boardState[r, c] == opponentSymbol)
						{
							foundOpponent = true;
						}
						else if (boardState[r, c] == playerSymbol && foundOpponent)
						{
							return true; // Valid move
						}
						else
						{
							break; // Empty space or same color without opponent in between
						}

						r += rowDir;
						c += colDir;
					}
				}
			}

			return false;
		}

		public List<(int row, int col)> GetValidMoves(Player player, char[,]? boardState = null)
		{
			if (boardState == null)
			{
				boardState = BoardState;
			}
			var validMoves = new List<(int, int)>();

			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					if (IsValidMove(row, col, player, boardState))
					{
						validMoves.Add((row, col));
					}
				}
			}

			return validMoves;
		}

		public bool HasValidMove(Player player, char[,]? boardState = null)
		{
			if (boardState == null)
			{
				boardState = BoardState;
			}

			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					if (IsValidMove(row, col, player, boardState))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void MakeMove(int row, int col, Player player, char[,]? boardState = null)
		{
			bool updateScores = false;
			if (boardState == null)
			{
				boardState = BoardState;
				updateScores = true;
			}
			char playerSymbol = PlayerSymbols[player];
			char opponentSymbol = PlayerSymbols[OthelloGame.GetOpponent(player)];

			boardState[row, col] = playerSymbol;

			// Flip opponent's pieces in all valid directions
			for (int rowDir = -1; rowDir <= 1; rowDir++)
			{
				for (int colDir = -1; colDir <= 1; colDir++)
				{
					if (rowDir == 0 && colDir == 0) continue;

					int r = row + rowDir;
					int c = col + colDir;
					List<Tuple<int, int>> piecesToFlip = new List<Tuple<int, int>>();
					bool foundOpponent = false;

					while (r >= 0 && r < Size && c >= 0 && c < Size)
					{
						if (boardState[r, c] == opponentSymbol)
						{
							piecesToFlip.Add(Tuple.Create(r, c));
							foundOpponent = true;
						}
						else if (boardState[r, c] == playerSymbol && foundOpponent)
						{
							// Flip all opponent pieces in this direction
							foreach (var piece in piecesToFlip)
							{
								boardState[piece.Item1, piece.Item2] = playerSymbol;
							}
							break;
						}
						else
						{
							break;
						}

						r += rowDir;
						c += colDir;
					}
				}
			}
			if (updateScores)
			{
				Game.UpdateScores();
			}
		}


	}
}
