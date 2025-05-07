using System;
using System.Collections.Generic;

namespace Othello
{
	class Program
	{
		static void Main(string[] args)
		{
			_ = EdgeStability.StabilityTable;
			Console.WriteLine("Choose game mode:");
			Console.WriteLine("1. Human vs Human");
			Console.WriteLine("2. Human vs AI (AI plays White)");
			Console.WriteLine("3. Human vs AI (AI plays Black)"); // TODO: add no print version, add choosing heuristic
			Console.WriteLine("4. AI vs AI (Watch simulation)");
			Console.Write("Enter choice (1-4): ");

			int mode;
			while (!int.TryParse(Console.ReadLine(), out mode) || mode < 1 || mode > 4)
			{
				Console.Write("Invalid input. Enter 1, 2, or 3: ");
			}

			bool blackIsAI = mode == 3 || mode == 4;
			bool whiteIsAI = mode == 2 || mode == 4;
			int aiDelay = mode == 4 ? 0 : 0; // Slow down AI vs AI for watching

			//bool aiEnabled = mode != 1;
			//Player aiPlayer = mode == 3 ? Player.Black : Player.White;

			OthelloGame game = new OthelloGame(blackIsAI, whiteIsAI, aiDelay);
			game.Play();
		}
	}
}