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
		public static (int row, int col, int score) FindBestMove(Player player, Board board, int minimaxDepth, int option = 0)
		{
			var validMoves = board.GetValidMoves(player);
			if (!validMoves.Any()) return (-1, -1, 0);

			int bestScore = int.MinValue;
			(int row, int col) bestMove = validMoves[0];

			foreach (var (row, col) in validMoves)
			{
				char[,] newBoard = (char[,])board.BoardState.Clone();
				board.MakeMove(row, col, player, newBoard);

				int score = Minimax(board, newBoard, OthelloGame.GetOpponent(player), minimaxDepth - 1, int.MinValue, int.MaxValue, false, option);

				if (score > bestScore)
				{
					bestScore = score;
					bestMove = (row, col);
				}
			}

			return (bestMove.row, bestMove.col, bestScore);
		}

		private static int Minimax(Board board, char[,] boardState, Player currentPlayer, int depth, int alpha, int beta, bool isMaximizing, int option)
		{
			// Base case: terminal node or depth limit reached
			if (depth == 0 || !board.HasValidMove(currentPlayer, boardState))
			{
				return EvaluateBoard(board, boardState, isMaximizing ? currentPlayer : OthelloGame.GetOpponent(currentPlayer), option);
			}

			var validMoves = board.GetValidMoves(currentPlayer, boardState);

			if (isMaximizing)
			{
				int maxEval = int.MinValue;
				foreach (var (row, col) in validMoves)
				{
					char[,] newBoard = (char[,])boardState.Clone();
					board.MakeMove(row, col, currentPlayer, newBoard);

					int eval = Minimax(board, newBoard, OthelloGame.GetOpponent(currentPlayer), depth - 1, alpha, beta, false, option);
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

					int eval = Minimax(board, newBoard, OthelloGame.GetOpponent(currentPlayer), depth - 1, alpha, beta, true, option);
					minEval = Math.Min(minEval, eval);

					beta = Math.Min(beta, eval);
					if (beta <= alpha) break;
				}
				return minEval;
			}
		}

		private static int EvaluateBoard(Board board, char[,] boardState, Player player, int option)
		{
			float totalScore = 0;

			int pieceCount = Heuristics.CalculatePieceCount(board, player);
			float mobilityScore = Heuristics.CalculateMobilityImpact(player, boardState, board);
			int cornerScore = Heuristics.CalculateCornerControl(board, player);
			int internalStabilityScore = Heuristics.CalculateInternalStability(board, player);
			int edgeStabilityScore = Heuristics.CalculateEdgeStability(player, board);
			int positionalScore = Heuristics.CalculatePositionalWeight(board, boardState, player);

			// Combine factors with weights
			switch (option)
			{
				case 0:
					totalScore = pieceCount * 1
						+ mobilityScore * 5
						+ cornerScore * 50
						+ internalStabilityScore * 5
						+ edgeStabilityScore * 10
						+ positionalScore * 1;
					break;
				case 1:
					totalScore = pieceCount * 4
						+ mobilityScore * 2
						+ cornerScore * 10
						+ internalStabilityScore * 2 
						+ edgeStabilityScore * 2 
						+ positionalScore * 0;
					break;
			}
	
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
