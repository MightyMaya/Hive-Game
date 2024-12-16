# Hive-Game
## Game Features 
### A) Gameplay Mechanics:

         1- Turn-Based Strategy: Players take turns moving their pieces following Hive's rules.
         2- Endgame: Game ends when a Queen Bee is surrounded by pieces of any color.
         3- Core Rules: First piece must be placed at the center.
         4- No movement is allowed until the Queen is placed.
         5- Irremovable pieces: Pieces cannot be taken off the board once placed.
         6- Placement restrictions: Pieces cannot initially be placed adjacent to an opponent's piece unless it is the first move.
         
### B) AI Features:

         1- AI Difficulty Levels: Adjustable depth of search for varying challenge levels. (using iterative deepening)
         2- Move Simulation: AI simulates potential moves and evaluates them using heuristics.
         3- Placement Strategy: AI strategically evaluates piece placements based on the game state.
         
### C) Game Modes:

         1- Human vs. AI: Adjustable AI difficulty (e.g., easy or high-level).
         2- AI vs. AI: Simulated gameplay.
         3- Human vs. Human: Direct multiplayer gameplay.
         

### D) AI Implementation:
  
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
      

### C) Move Evaluation:

        AI scores each move by simulating its impact and selecting the best one.
        
### D) Placement Evaluation: 

        Evaluates placement positions for unplaced pieces.
        
### E) Game State Scoring:

        Combines multiple heuristics (mobility, Queen safety, and priorities) to score board states.


## Manual User

### A) Objective:

       Surround the opponent's Queen Bee while ensuring your own Queen Bee is safe from being surrounded.
       
### B) Piece Types:

       1- Queen Bee: Must be placed first and can move one space at a time in any direction.
       2- Ant: Moves any number of spaces, but cannot jump over other pieces.
       3- Spider: Moves exactly three spaces in a straight line.
       4- Grasshopper: Jumps over pieces to the next empty space.
### C) Turn Order:

       1- Players take turns to place and move pieces on the board.
       2- The first piece placed must be in the center of the board, and all subsequent pieces must be adjacent to the first piece.
       
### D) Movement and Placement:

       1- Once placed, pieces cannot be moved until the Queen Bee has been placed.
       2- Pieces cannot be placed next to an opponent’s piece except for the first piece placement.
       3- Movement depends on the piece type (Ant, Spider, Grasshopper).

### E) Endgame Condition:

      The game ends when a player’s Queen Bee is surrounded by the opponent’s pieces. The player who surrounds the opponent's Queen Bee wins. 
      
### F) Supported Game Modes:

      1- Human vs. Human: Two players on the same system.
      2- Human vs. AI: One player competes against the AI at low or high difficulty levels.
      3- AI vs. AI: Watch AI play against itself.
