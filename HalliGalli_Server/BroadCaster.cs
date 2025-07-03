using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 브로드캐스터

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
            Card = new Card("서버로 잘 전달됨",200);
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
        //"id": int // 테이블 구분할 id
        //"key": int (enum) // 사용자가 누른 키
        //"time_dif": timestamp // 시간차(ms 단위, optional) 
        //"penalty": bool // 패널티 여부(확정 x)
        public int playerId;
        public string key;
        public DateTime? timestamp;
        public bool penalty;

        public MessageCliToServer() { 
            this.playerId=0;
            this.key = "";
            this.timestamp = DateTime.Now;
            this.penalty = false; 
        }
        public MessageCliToServer(int playerId, string key, bool penalty) { 
            this.playerId =playerId;
            this.key = key;
            this.penalty = penalty;
        }
        public MessageCliToServer(int playerId, string key, DateTime timestamp, bool penalty) { 
            this.playerId =playerId;
            this.key = key;
            this.timestamp = timestamp;
            this.penalty = penalty;
        }

    }

    public class Broadcaster
    {
        public static Broadcaster Instance { get; } = new Broadcaster();

        public void SendJson<T>(T obj, NetworkStream stream)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true, // ← 필드 포함
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // JSON 네이밍 정책
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(obj, options);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            stream.Write(dataLength, 0, 4);
            stream.Write(data, 0, data.Length);
            stream.Flush();

            Console.WriteLine("보낸 JSON:\n" + json);
        }

    }
}
