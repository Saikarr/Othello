using System;
using System.Collections.Generic;

namespace Othello
{
	public enum Player { Black, White }

	public class OthelloGame
	{
		private const int BoardSize = 8;
		private char[,] board;
		private Player currentPlayer;
		private Dictionary<Player, char> playerSymbols;
		private int blackScore;
		private int whiteScore;

		public OthelloGame()
		{
			board = new char[BoardSize, BoardSize];
			playerSymbols = new Dictionary<Player, char>
			{
				{ Player.Black, 'B' },
				{ Player.White, 'W' }
			};
			InitializeBoard();
			currentPlayer = Player.Black;
			UpdateScores();
		}

		private void InitializeBoard()
		{
			// Initialize empty board
			for (int row = 0; row < BoardSize; row++)
			{
				for (int col = 0; col < BoardSize; col++)
				{
					board[row, col] = '.';
				}
			}

			// Set up starting pieces
			board[3, 3] = 'W';
			board[3, 4] = 'B';
			board[4, 3] = 'B';
			board[4, 4] = 'W';
		}

		public void Play()
		{
			Console.WriteLine("Welcome to Othello!");
			Console.WriteLine("Black (B) moves first. Enter moves as row and column numbers (0-7).");

			while (true)
			{
				PrintBoard();
				Console.WriteLine($"Current player: {currentPlayer} ({playerSymbols[currentPlayer]})");
				Console.WriteLine($"Score - Black: {blackScore}, White: {whiteScore}");

				if (!HasValidMove(currentPlayer))
				{
					Console.WriteLine($"No valid moves for {currentPlayer}. Passing turn.");
					if (!HasValidMove(GetOpponent(currentPlayer)))
					{
						Console.WriteLine("No valid moves for either player. Game over!");
						break;
					}
					currentPlayer = GetOpponent(currentPlayer);
					continue;
				}

				Console.Write("Enter your move (row col): ");
				string input = Console.ReadLine();
				if (input.ToLower() == "quit")
				{
					Console.WriteLine("Game ended by user.");
					break;
				}

				string[] parts = input.Split(' ');
				if (parts.Length != 2 || !int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
				{
					Console.WriteLine("Invalid input. Please enter two numbers separated by space.");
					continue;
				}

				if (IsValidMove(row, col, currentPlayer))
				{
					MakeMove(row, col, currentPlayer);
					currentPlayer = GetOpponent(currentPlayer);
				}
				else
				{
					Console.WriteLine("Invalid move. Try again.");
				}
			}

			PrintBoard();
			DetermineWinner();
		}

		private bool IsValidMove(int row, int col, Player player)
		{
			// Check if position is on the board
			if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
				return false;

			// Check if position is empty
			if (board[row, col] != '.')
				return false;

			char playerSymbol = playerSymbols[player];
			char opponentSymbol = playerSymbols[GetOpponent(player)];

			// Check all 8 directions
			for (int rowDir = -1; rowDir <= 1; rowDir++)
			{
				for (int colDir = -1; colDir <= 1; colDir++)
				{
					if (rowDir == 0 && colDir == 0) continue; // Skip current position

					int r = row + rowDir;
					int c = col + colDir;
					bool foundOpponent = false;

					while (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize)
					{
						if (board[r, c] == opponentSymbol)
						{
							foundOpponent = true;
						}
						else if (board[r, c] == playerSymbol && foundOpponent)
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

		private void MakeMove(int row, int col, Player player)
		{
			char playerSymbol = playerSymbols[player];
			char opponentSymbol = playerSymbols[GetOpponent(player)];

			board[row, col] = playerSymbol;

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

					while (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize)
					{
						if (board[r, c] == opponentSymbol)
						{
							piecesToFlip.Add(Tuple.Create(r, c));
							foundOpponent = true;
						}
						else if (board[r, c] == playerSymbol && foundOpponent)
						{
							// Flip all opponent pieces in this direction
							foreach (var piece in piecesToFlip)
							{
								board[piece.Item1, piece.Item2] = playerSymbol;
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

			UpdateScores();
		}

		private bool HasValidMove(Player player)
		{
			for (int row = 0; row < BoardSize; row++)
			{
				for (int col = 0; col < BoardSize; col++)
				{
					if (IsValidMove(row, col, player))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateScores()
		{
			blackScore = 0;
			whiteScore = 0;

			for (int row = 0; row < BoardSize; row++)
			{
				for (int col = 0; col < BoardSize; col++)
				{
					if (board[row, col] == 'B')
						blackScore++;
					else if (board[row, col] == 'W')
						whiteScore++;
				}
			}
		}

		private Player GetOpponent(Player player)
		{
			return player == Player.Black ? Player.White : Player.Black;
		}

		private void PrintBoard()
		{
			Console.WriteLine();
			Console.Write("  ");
			for (int col = 0; col < BoardSize; col++)
			{
				Console.Write(col + " ");
			}
			Console.WriteLine();

			for (int row = 0; row < BoardSize; row++)
			{
				Console.Write(row + " ");
				for (int col = 0; col < BoardSize; col++)
				{
					Console.Write(board[row, col] + " ");
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private void DetermineWinner()
		{
			Console.WriteLine($"Final Score - Black: {blackScore}, White: {whiteScore}");
			if (blackScore > whiteScore)
			{
				Console.WriteLine("Black wins!");
			}
			else if (whiteScore > blackScore)
			{
				Console.WriteLine("White wins!");
			}
			else
			{
				Console.WriteLine("It's a tie!");
			}
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			OthelloGame game = new OthelloGame();
			game.Play();
		}
	}
}