using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    public class MessageServerToCli
    {
        public int? playerId;
        public string playerName;
        public bool? isTurnActive;
        public Card? Card;
        public int? userState;
        public Card[]? openCards;

        public MessageServerToCli() // 테스트데이터 (디폴트값)
        {
            playerId = 0;
            isTurnActive = false;
            Card = new Card("서버로 잘 전달됨", 200);
            userState = 0;
            openCards = new Card[2];
            openCards[0] = new Card("서버로 잘 전달됨", 200);
            openCards[1] = new Card("서버로 잘 전달됨", 200);
        }

        public MessageServerToCli(int state) // 테스트데이터 (디폴트값)
        {
            this.userState = state;
        }



        public MessageServerToCli(int playerId, string playerName, int userState)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.userState = userState;
        }

        public MessageServerToCli(int playerId, string playerName, bool isTurnActivate, Card card, int userState, Card[] openCards)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.isTurnActive = isTurnActivate;
            this.Card = card;
            this.userState = userState;
            this.openCards = openCards;
        }
    }

    public class MessageCliToServer
    {
        public int playerId;
        public string playerName;
        public string key;
        public int? timestamp;

        public MessageCliToServer()
        {
            this.playerId = 0;
            this.playerName = "";
            this.key = "";
            this.timestamp = 0;
        }
        public MessageCliToServer(int playerId, string playerName, string key)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.key = key;
        }
        public MessageCliToServer(int playerId, string playerName, string key, int timestamp)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.key = key;
            this.timestamp = timestamp;
        }

    }
}
