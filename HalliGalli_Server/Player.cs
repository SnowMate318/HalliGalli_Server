using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int Id;
        
        public CardDeck cardDeck;
        private Table table;

        public string Name = "없음";
        public bool isAlive = true;
        public bool isTurn = false;


        public StreamManager streamManager; // 네트워크 관리


        public Player(int id, NetworkStream stream, TcpClient client)
        {
            this.Id = id;
            this.streamManager = new StreamManager(stream,client);
            this.table = new Table();
            this.cardDeck = new CardDeck();

            //Todo: 유저이름 입력받기(회의 필요)
        }

        public void ReceiveUserInfo(MessageCliToServer msg)
        {
            // 유저정보를 수신함
            switch (msg.key)
            {
                case 1: // 
                    PlayCard(msg);
                    break;
                case 2:
                    RingBell(msg);
                    break;
            }
        }

        private void PlayCard(MessageCliToServer msg)
        {
            table.PlayCard(msg.name); // 테이블에서 카드 내기 로직 호출
        }
        private void RingBell(MessageCliToServer msg)
        {

            if (msg.penalty == true)
            {
                table.ApplyPenalty(msg.name);
                return;
            }
            if (msg.time_dif == null) return;

            //Todo: 타임스탬프 밀리초단위 시간차 받아서
            int value = (int)msg.time_dif;

            table.bell.Ring(msg.name, value);
            // Json으로 받은 정보가 종 울리기일 경우
        }

        
    }

}

//public T ReceiveJson<T>()
//{
//    byte[] lengthBytes = new byte[4];
//    stream.Read(lengthBytes, 0, 4);
//    int length = BitConverter.ToInt32(lengthBytes, 0);

//    byte[] data = new byte[length];
//    int readBytes = 0;
//    while (readBytes < length)
//    {
//        int r = stream.Read(data, readBytes, length - readBytes);
//        if (r == 0) throw new IOException("Disconnected");
//        readBytes += r;
//    }

//    string json = Encoding.UTF8.GetString(data);

//    //테스트
//    Console.WriteLine("=== 수신된 원본 JSON ===");
//    Console.WriteLine(json);
//    Console.WriteLine("========================");

//    return JsonSerializer.Deserialize<T>(json);
//}