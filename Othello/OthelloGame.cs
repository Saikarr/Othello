using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public class OthelloGame
	{
		private const int BoardSize = 8;
		private const int MinimaxDepth = 5;
		private Board Board;
		private Player currentPlayer;
		private Dictionary<Player, char> playerSymbols;
		private int blackScore;
		private int whiteScore;
		//private bool aiEnabled;
		//private Player aiPlayer;
		private readonly bool blackIsAI;
		private readonly bool whiteIsAI;
		private readonly int aiDelay;

		public OthelloGame(bool blackIsAI = false, bool whiteIsAI = false, int aiDelay = 0)
		{
			this.blackIsAI = blackIsAI;
			this.whiteIsAI = whiteIsAI;
			this.aiDelay = aiDelay;

			Board = new Board(BoardSize, this);
			playerSymbols = new Dictionary<Player, char>
			{
				{ Player.Black, 'B' },
				{ Player.White, 'W' }
			};
			//aiEnabled = enableAI;
			//aiPlayer = aiPlaysAs;
			currentPlayer = Player.Black;
			UpdateScores();
		}
		public void Play()
		{
			Console.WriteLine("Welcome to Othello!");
			//if (aiEnabled)
			//{
			//	Console.WriteLine($"AI is playing as {aiPlayer} (depth {MinimaxDepth}).");
			//}
			if (blackIsAI) Console.WriteLine("Black (B) is controlled by AI");
			if (whiteIsAI) Console.WriteLine("White (W) is controlled by AI");
			Console.WriteLine("Black (B) moves first. Enter moves as row and column numbers (0-7).");
			Console.WriteLine("Type 'hint' for a suggested move or 'quit' to end the game.");

			while (true)
			{
				PrintBoard();
				Console.WriteLine($"Current player: {currentPlayer} ({playerSymbols[currentPlayer]})");
				Console.WriteLine($"Score - Black: {blackScore}, White: {whiteScore}");

				if (!Board.HasValidMove(currentPlayer))
				{
					Console.WriteLine($"No valid moves for {currentPlayer}. Passing turn.");
					if (!Board.HasValidMove(GetOpponent(currentPlayer)))
					{
						Console.WriteLine("No valid moves for either player. Game over!");
						break;
					}
					currentPlayer = GetOpponent(currentPlayer);
					continue;
				}

				//// AI move
				//if (aiEnabled && currentPlayer == aiPlayer)
				//{
				//	Console.WriteLine("AI is thinking...");
				//	var (sugRow, sugCol, _) = MoveEvaluator.FindBestMove(currentPlayer, Board, MinimaxDepth);
				//	Console.WriteLine($"AI plays: {sugRow} {sugCol}");
				//	Board.MakeMove(sugRow, sugCol, currentPlayer);
				//	currentPlayer = GetOpponent(currentPlayer);
				//	continue;
				//}

				// Handle AI turn
				if ((currentPlayer == Player.Black && blackIsAI) ||
					(currentPlayer == Player.White && whiteIsAI))
				{
					if (aiDelay > 0) Thread.Sleep(aiDelay);

					Console.WriteLine("AI is thinking...");
					var (sugRow, sugCol, _) = MoveEvaluator.FindBestMove(currentPlayer, Board, MinimaxDepth, (int)currentPlayer);
					Console.WriteLine($"AI plays: {sugRow} {sugCol}");
					Board.MakeMove(sugRow, sugCol, currentPlayer);
					currentPlayer = GetOpponent(currentPlayer);
					continue;
				}

				Console.Write("Enter your move (row col), 'hint', or 'quit': ");
				string input = Console.ReadLine().Trim().ToLower();

				if (input == "quit")
				{
					Console.WriteLine("Game ended by user.");
					break;
				}
				else if (input == "hint")
				{
					var (sugRow, sugCol, score) = MoveEvaluator.FindBestMove(currentPlayer, Board, MinimaxDepth);
					Console.WriteLine($"Suggested move: {sugRow} {sugCol} (score: {score})");
					continue;
				}

				string[] parts = input.Split(' ');
				if (parts.Length != 2 || !int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
				{
					Console.WriteLine("Invalid input. Please enter two numbers separated by space.");
					continue;
				}

				if (Board.IsValidMove(row, col, currentPlayer))
				{
					Board.MakeMove(row, col, currentPlayer);
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

		public static Player GetOpponent(Player player)
		{
			return player == Player.Black ? Player.White : Player.Black;
		}

		public void UpdateScores()
		{
			blackScore = 0;
			whiteScore = 0;

			for (int row = 0; row < BoardSize; row++)
			{
				for (int col = 0; col < BoardSize; col++)
				{
					if (Board.BoardState[row, col] == 'B')
						blackScore++;
					else if (Board.BoardState[row, col] == 'W')
						whiteScore++;
				}
			}
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
					Console.Write(Board.BoardState[row, col] + " ");
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
}
