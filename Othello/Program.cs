using System;
using System.Collections.Generic;

namespace Othello
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Choose game mode:");
			Console.WriteLine("1. Human vs Human");
			Console.WriteLine("2. Human vs AI (AI plays White)");
			Console.WriteLine("3. Human vs AI (AI plays Black)"); // TODO: add AI vs AI mode for evaluation purposes
			Console.Write("Enter choice (1-3): ");

			int choice;
			while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
			{
				Console.Write("Invalid input. Enter 1, 2, or 3: ");
			}

			bool aiEnabled = choice != 1;
			Player aiPlayer = choice == 3 ? Player.Black : Player.White;

			OthelloGame game = new OthelloGame(aiEnabled, aiPlayer);
			game.Play();
		}
	}
}