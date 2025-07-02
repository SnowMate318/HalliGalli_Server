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

        public void MergeDeck(CardDeck otherDeck)
        {
            //Todo: 다른 카드 덱을 합치기
        }
        public Card DrawCard()
        {
            //Todo: 카드 반환
            return null;
        }
        public void AddCard(Card card)
        {
            //Todo: 자신의 덱에 카드를 추가함
        }
        public int GetCardCount()
        {
            //Todo: 저장된 카드 갯수를 호출
            return deck.Count;
        }
    }
}
