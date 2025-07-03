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
    // 플레이어 클래스
    public class Player
    {
        public CardDeck cardDeck = new CardDeck();
        public int playerId;
        public string username = "없음";
        public bool isAlive = true;
        public bool isTurn = false;

        public TcpClient tcpClient;
        public NetworkStream stream;
        public StreamReader reader;
        public StreamWriter writer;

        public Player(int id, NetworkStream stream, TcpClient client)
        {
            this.playerId = id;
            this.stream = stream;
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream); 
            this.tcpClient = client;
            
            //Todo: 유저이름 입력받기(회의 필요)
        }

        public void ReceiveUserInfo(MessageCliToServer msg)
        {
            // 유저정보를 수신함
            switch (msg.key)
            {
                case "k":
                    PlayCard();
                    break;
                case " ":
                    RingBell(msg);
                    break;
            }
        }

        private void PlayCard()
        {
            // Json으로 받은 정보가 카드 내기일 경우
            if (!isTurn)
            {
                return;
            }
            Table.Instance.PlayCard(playerId); // 테이블에서 카드 내기 로직 호출
        }
        private void RingBell(MessageCliToServer msg)
        {
            //Todo: 타임스탬프 밀리초단위 시간차 받아서
            Random rand = new Random();
            int value = rand.Next(1, 6);
            Table.Instance.bell.Ring(msg.playerId, value);
            // Json으로 받은 정보가 종 울리기일 경우
        }

        public T ReceiveJson<T>()
        {
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = new byte[length];
            int readBytes = 0;
            while (readBytes < length)
            {
                int r = stream.Read(data, readBytes, length - readBytes);
                if (r == 0) throw new IOException("Disconnected");
                readBytes += r;
            }

            string json = Encoding.UTF8.GetString(data);

            //테스트
            Console.WriteLine("=== 수신된 원본 JSON ===");
            Console.WriteLine(json);
            Console.WriteLine("========================");

            return JsonSerializer.Deserialize<T>(json);
        }



        
    }

}
