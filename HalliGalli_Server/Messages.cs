using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    using System.Text.Json.Serialization;

    public class MessageServerToCli
    {
        [JsonPropertyName("id")]
        public int? PlayerId { get; set; }

        [JsonPropertyName("name")]
        public string PlayerName { get; set; } = "";

        [JsonPropertyName("turn")]
        public bool IsTurnActive { get; set; }
        //[JsonPropertyName("card")]
        //public Card Card { get; set; }

        // 펼쳐진 카드 목록 → JSON의 "카드정보"
        [JsonPropertyName("card")]
        public Card[]? OpenCards { get; set; }

        [JsonPropertyName("user_status")]
        public int? UserState { get; set; }

        // 남은 카드 개수 배열 → JSON의 "남은카드개수"
        [JsonPropertyName("remaining_card_count")]
        public int[]? RemainingCardCounts { get; set; }

        public MessageServerToCli() { }

        public MessageServerToCli(int userState)
        {
            IsTurnActive = false;
            OpenCards = new Card[0];
            UserState = userState;
            RemainingCardCounts = new int[0];
        }
        public MessageServerToCli(int playerId, string playerName, int userState)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            IsTurnActive = false;
            OpenCards = new Card[0];
            UserState = userState;
            RemainingCardCounts = new int[0];

            
        }
        public MessageServerToCli(int playerId, string playerName, bool turn, Card card, Card[] openCards, int userState, int[] remainingCounts)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            IsTurnActive = turn;
            //Card = card;
            OpenCards = openCards;
            UserState = userState;
            RemainingCardCounts = remainingCounts;
        }
    }

    public class MessageCliToServer
    {
        public int id;
        public string name;
        public int key;
        public int? time_dif;
        public bool penalty; 

        public MessageCliToServer()
        {
            this.id = 0;
            this.name = "";
            this.key = 0;
            this.time_dif = 0;
            this.penalty = false;
        }

        public MessageCliToServer(int playerId, string playerName, int key, bool penalty)
        {
            this.id = playerId;
            this.name = playerName;
            this.key = key;
            this.penalty = penalty;
        }

        public MessageCliToServer(int playerId, string playerName, int key, int time_dif, bool penalty)
        {
            this.id = playerId;
            this.name = playerName;
            this.key = key;
            this.time_dif = time_dif;
            this.penalty = penalty;
        }
    }   
}
