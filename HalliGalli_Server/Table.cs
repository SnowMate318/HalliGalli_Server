using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 테이블 클래스
    public class Table
    {
        public List<Player> players = new();
        public List<Card> openedCards = new();
        public Dictionary<string, int> fruitCardCount = new();

        public Queue<Card> tableDeck = new();
        public int currentTurnPlayerId;
        public bool isActive;
        public Bell bell = new();

        public void StartGame() { }
        public void PlayCard(int playerId) { }
        public Card[] ShowCurrentResult() { return null; }
        public void MergeDeck(int playerId) { }
        public void MoveTurn() { }
        public void ApplyPenalty(int playerId) { }
        public void PlayerDeath(int playerId) { }
        public int CheckWinner() { return -1; }
    }
}
