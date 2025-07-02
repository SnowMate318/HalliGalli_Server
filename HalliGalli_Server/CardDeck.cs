using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 카드덱 클래스
    public class CardDeck
    {
        public Queue<Card> deck = new();

        public void MergeDeck(CardDeck otherDeck) { }
        public Card DrawCard() { return null; }
        public void AddCard(Card card) { }
        public int GetCardCount() { return deck.Count; }
    }
}
