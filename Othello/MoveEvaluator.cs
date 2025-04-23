using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public static class MoveEvaluator // TODO: implement proper usage of heuristics, current is very crude (and not functional, we need to evaluate the state of the board, not the move itself
									  // because thats how minimax works)
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

		public static (int row, int col, int score) FindBestMove(Player player, Board board, int minimaxDepth)
		{
			var validMoves = board.GetValidMoves(player);
			if (!validMoves.Any()) return (-1, -1, 0);

			int bestScore = int.MinValue;
			(int row, int col) bestMove = validMoves[0];

			foreach (var (row, col) in validMoves)
			{
				char[,] newBoard = (char[,])board.BoardState.Clone();
				board.MakeMove(row, col, player, newBoard);

				int score = Minimax(board, newBoard, OthelloGame.GetOpponent(player), minimaxDepth - 1, int.MinValue, int.MaxValue, false);

				if (score > bestScore)
				{
					bestScore = score;
					bestMove = (row, col);
				}
			}

			return (bestMove.row, bestMove.col, bestScore);
		}

		private static int Minimax(Board board, char[,] boardState, Player currentPlayer, int depth, int alpha, int beta, bool isMaximizing)
		{
			// Base case: terminal node or depth limit reached
			if (depth == 0 || !board.HasValidMove(currentPlayer, boardState))
			{
				return EvaluateBoard(board, boardState, isMaximizing ? currentPlayer : OthelloGame.GetOpponent(currentPlayer));
			}

			var validMoves = board.GetValidMoves(currentPlayer, boardState);

			if (isMaximizing) // slightly changed, dont know if for better
			{
				int maxEval = int.MinValue;
				foreach (var (row, col) in validMoves)
				{
					char[,] newBoard = (char[,])boardState.Clone();
					board.MakeMove(row, col, currentPlayer, newBoard);

					int eval = Minimax(board, newBoard, OthelloGame.GetOpponent(currentPlayer), depth - 1, alpha, beta, false);
					maxEval = Math.Max(maxEval, eval);

					if (beta <= maxEval) break;
					alpha = Math.Max(alpha, maxEval);

					//alpha = Math.Max(alpha, eval);
					//if (beta <= alpha) break;
				}
				return maxEval;
			}
			else
			{
				int minEval = int.MaxValue;
				foreach (var (row, col) in validMoves)
				{
					char[,] newBoard = (char[,])boardState.Clone();
					newBoard[row, col] = board.PlayerSymbols[currentPlayer];
					board.MakeMove(row, col, currentPlayer, newBoard);

					int eval = Minimax(board, newBoard, OthelloGame.GetOpponent(currentPlayer), depth - 1, alpha, beta, true);
					minEval = Math.Min(minEval, eval);

					if (minEval <= alpha) break;
					beta = Math.Min(beta, minEval);

					//beta = Math.Min(beta, eval);
					//if (beta <= alpha) break;
				}
				return minEval;
			}
		}

		private static int EvaluateBoard(Board board, char[,] boardState, Player forPlayer)
		{
			int playerScore = 0;
			int opponentScore = 0;
			char playerSymbol = board.PlayerSymbols[forPlayer];
			char opponentSymbol = board.PlayerSymbols[OthelloGame.GetOpponent(forPlayer)];

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

			// Mobility calculation
			int playerMobility = board.GetValidMoves(forPlayer, boardState).Count;
			int opponentMobility = board.GetValidMoves(OthelloGame.GetOpponent(forPlayer), boardState).Count;

			// Corner control bonus
			int playerCorners = Heuristics.CountCorners(forPlayer, boardState, board);
			int opponentCorners = Heuristics.CountCorners(OthelloGame.GetOpponent(forPlayer), boardState, board);

			// Combine factors with weights
			int score = (playerScore - opponentScore)
					   + (playerMobility - opponentMobility) * 5
					   + (playerCorners - opponentCorners) * 25;

			return score;
		}

		public static int EvaluateMove(int row, int col, Player player, Board board, char[,] boardState) // This one isnt really functional because it evaluates the move, thats just an example
		{
			char playerSymbol = board.PlayerSymbols[player];
			char opponentSymbol = board.PlayerSymbols[OthelloGame.GetOpponent(player)];
			int totalScore = 0;

			// 1. Immediate point advantage (number of pieces flipped)
			int piecesFlipped = Heuristics.CountPiecesFlipped(row, col, player, boardState, board);

			// 2. Positional advantage (corner and edge control)
			int positionalValue = positionWeights[row, col];

			// 3. Mobility (number of future moves this move enables)
			int mobility = Heuristics.CalculateMobilityImpact(row, col, player, boardState, board);

			// 4. Stability (how likely the piece is to stay flipped)
			int stability = Heuristics.CalculateStability(row, col, player);

			// Combine factors with weights
			totalScore = (piecesFlipped * 10) + (positionalValue * 5) + (mobility * 3) + (stability * 2);

			return totalScore;
		}
	}
}
