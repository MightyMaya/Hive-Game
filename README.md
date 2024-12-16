# Hive-Game
<h2>Game Features </h2>
<h3>A) Gameplay Mechanics:</h3>

         1- Turn-Based Strategy: Players take turns moving their pieces following Hive's rules.
         2- Endgame: Game ends when a Queen Bee is surrounded by pieces of any color.
         3- Core Rules: First piece must be placed at the center.
         4- No movement is allowed until the Queen is placed.
         5- Irremovable pieces: Pieces cannot be taken off the board once placed.
         6- Placement restrictions: Pieces cannot initially be placed adjacent to an opponent's piece unless it is the first move.
         
<h3>B) AI Features:</h3>

         1- AI Difficulty Levels: Adjustable depth of search for varying challenge levels. (using iterative deepening)
         2- Move Simulation: AI simulates potential moves and evaluates them using heuristics.
         3- Placement Strategy: AI strategically evaluates piece placements based on the game state.
         
<h3>C) Game Modes:</h3>

         1- Human vs. AI: Adjustable AI difficulty (e.g., easy or high-level).
         2- AI vs. AI: Simulated gameplay.
         3- Human vs. Human: Direct multiplayer gameplay.
         

<h3>D) AI Implementation:</h3>
  
        1- Heuristic Evaluation:
               - Piece Mobility: Scores based on the number of legal moves for each piece.
               - Penalizes blocked pieces and rewards high-priority pieces like the Queen.
               - Queen Safety: Evaluates the Queen's safety based on:  Proximity to opponent pieces (penalty)
                                                                       Support from friendly pieces (reward).
                                                                       Queen's mobility (reward).
                                                                       Weights are applied to balance defensive and offensive strategies.
                                                                       
        2- Minimax Algorithm:  Simulates potential game states to select optimal moves.
                               Incorporates Alpha-Beta Pruning to optimize decision-making by eliminating irrelevant moves.
                              
        3- Iterative Deepening:  Explores moves incrementally deeper within a time frame to balance computational efficiency and strategic depth.
      

<h3>C) Move Evaluation:</h3>
        AI scores each move by simulating its impact and selecting the best one.
<h3>D) Placement Evaluation: </h3>
        Evaluates placement positions for unplaced pieces.
<h3>E) Game State Scoring:</h3>
        Combines multiple heuristics (mobility, Queen safety, and priorities) to score board states.
