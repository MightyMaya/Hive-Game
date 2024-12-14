using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameState
    {
        private Dictionary<(int x, int y), Stack<GameObject>> positions;
        private string currentPlayer;
        private bool isFirstMove;
        private int moveCount;
        private int b_turnCount;
        private int w_turnCount;

        public GameState(Game game, string currentPlayer)
        {
            // Save the current game state
            positions = new Dictionary<(int x, int y), Stack<GameObject>>();
            foreach (var pos in game.GetPositions())
            {
                positions[pos.Key] = new Stack<GameObject>(pos.Value); // Clone the stack
            }
            this.currentPlayer = currentPlayer;
            this.isFirstMove = game.isFirstMove;
            this.moveCount = game.moveCount;
            this.b_turnCount = game.b_turncount;
            this.w_turnCount = game.w_turncount;
        }

        public void Restore(Game game, string currentPlayer)
        {
            // Restore the saved game state
            game.SetPositions(positions);
            game.SetCurrentPlayer(currentPlayer);
            game.isFirstMove = isFirstMove;
            game.moveCount = moveCount;
            game.b_turncount = b_turnCount;
            game.w_turncount = w_turnCount;
        }
    }

}