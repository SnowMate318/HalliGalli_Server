using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    public class Message
    {
        public int playerId;
        public bool isTurnActive;
        public Card Card;
        public int userState;
        public Card[] openCards;

        public Message() // 테스트데이터 (디폴트값)
        {
            playerId = 0;
            isTurnActive = false;
            Card = new Card("서버로 잘 전달됨", 200);
            userState = 0;
            openCards = new Card[2];
            openCards[0] = new Card("서버로 잘 전달됨", 200);
            openCards[1] = new Card("서버로 잘 전달됨", 200);
        }

        public Message(int playerId, bool isTurnActivate, Card card, int userState, Card[] openCards)
        {
            this.playerId = playerId;
            this.isTurnActive = isTurnActivate;
            this.Card = card;
            this.userState = userState;
            this.openCards = openCards;
        }
    }

    public class MessageCliToServer
    {
        public int playerId;
        public string key;
        public DateTime? timestamp;
        public bool penalty;

        public MessageCliToServer()
        {
            this.playerId = 0;
            this.key = "";
            this.timestamp = DateTime.Now;
            this.penalty = false;
        }
        public MessageCliToServer(int playerId, string key, bool penalty)
        {
            this.playerId = playerId;
            this.key = key;
            this.penalty = penalty;
        }
        public MessageCliToServer(int playerId, string key, DateTime timestamp, bool penalty)
        {
            this.playerId = playerId;
            this.key = key;
            this.timestamp = timestamp;
            this.penalty = penalty;
        }

    }
}
