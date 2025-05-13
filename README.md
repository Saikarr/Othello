# Othello
This repository contains a complete implementation of the classic Othello (Reversi) board game in C#. The game features:
- Console-based interface
- Advanced AI opponent using Minimax algorithm with alpha-beta pruning
- Multiple game modes (Human vs Human, Human vs AI, AI vs AI)
- Heuristic-based move evaluation
- Move suggestions/hints
## Features
- Game Engine: Complete Othello game logic with all standard rules
- AI Opponent:
  - Uses Minimax algorithm with depth 5 search
  - Alpha-beta pruning for optimization
  - Sophisticated heuristic evaluation function
- Multiple Game Modes:
  - Human vs Human
  - Human vs AI (play as either Black or White)
  - AI vs AI (watch simulation)
- Move Suggestions: Get AI-recommended moves during human play
- Score Tracking: Real-time score display
### During gameplay:
- Enter moves as row column (0-7, space-separated)
- Type hint to get AI move suggestion
- Type quit to exit game
## Customization Options
You can adjust the AI behavior by modifying:
- MinimaxDepth in OthelloGame.cs (higher = stronger but slower)
- positionWeights matrix for different board strategies
- aiDelay in simulation mode for faster/slower viewing
- coefficients used for different heuristics in the MoveEvaluator.cs to adjust how each heuristic impacts the AI
