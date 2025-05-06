using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
	public static class MoveEvaluator
	{
		private static readonly int MobilityCoef = 1000;
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

			if (isMaximizing)
			{
				int maxEval = int.MinValue;
				foreach (var (row, col) in validMoves)
				{
					char[,] newBoard = (char[,])boardState.Clone();
					board.MakeMove(row, col, currentPlayer, newBoard);

					int eval = Minimax(board, newBoard, OthelloGame.GetOpponent(currentPlayer), depth - 1, alpha, beta, false);
					maxEval = Math.Max(maxEval, eval);

					alpha = Math.Max(alpha, eval);
					if (beta <= alpha) break;
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

					beta = Math.Min(beta, eval);
					if (beta <= alpha) break;
				}
				return minEval;
			}
		}

		private static int EvaluateBoard(Board board, char[,] boardState, Player player)
		{
			float totalScore;

			int positionalScore = Heuristics.CalculatePositionalWeight(board, boardState, player);
			float mobilityScore = Heuristics.CalculateMobilityImpact(player, boardState, board);
			int stabilityScore = Heuristics.CalculateEdgeStability(player, board);
			int internalStabilityScore = Heuristics.CalculateInternalStability(board, player);
			int cornerScore = Heuristics.CalculateCornerControl(board, player);

			// Combine factors with weights
			totalScore = positionalScore + mobilityScore * 5 + stabilityScore * 2 + internalStabilityScore + cornerScore * 25;

			return (int)totalScore;
		}

		public static int EvaluateMove(int row, int col, Player player, Board board, char[,] boardState)
		{
			int totalScore = 0;

			float mobility = Heuristics.CalculateMobilityImpact(player, boardState, board);
			int stability = Heuristics.CalculateEdgeStability(player, board);
			int internalstability = Heuristics.CalculateInternalStability(board, player);
			// Combine factors with weights
			totalScore = (int)(mobility * MobilityCoef) + (stability * 2) + internalstability;

			return totalScore;
		}
	}
}
