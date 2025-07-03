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
            Card = new Card();
            userState = 0;
            openCards = new Card[2];
            openCards[0] = new Card();
            openCards[1] = new Card();
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

    public class Message2
    {
        public int playerId;
        public required string message;
    }

    public class Broadcaster
    {
        public static Broadcaster Instance { get; } = new Broadcaster();

        public void SendJson<T>(T obj, NetworkStream stream)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            stream.Write(dataLength, 0, 4);  // 데이터 길이 먼저 전송 (4바이트)
            stream.Write(data, 0, data.Length);  // 실제 데이터 전송
            stream.Flush();
        }

    }
}
