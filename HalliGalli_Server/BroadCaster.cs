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

    internal class Message1 {
        public int playerId;
        public string message;
    
    }
    internal class Message2 { 
        public int playerId;
        public bool isTurnActive;
        public Card Card;
        public int userState;
        public Card[] openCards;
    
    }

    public class Broadcaster
    {

        public void SendJson<T>(T obj, NetworkStream stream)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            stream.Write(dataLength, 0, 4);  // 데이터 길이 먼저 전송 (4바이트)
            stream.Write(data, 0, data.Length);  // 실제 데이터 전송
            stream.Flush();
        }


        //public void SendJson(int playerId, string message) { }

        //public void SendJson(int playerId, bool isTurnActive, Card card, int userState, Card[] openedCards) { }
        
        //public T ReceiveJson<T>()
        //{
        //    byte[] lengthBytes = new byte[4];
        //    stream.Read(lengthBytes, 0, 4);  // 길이 읽기
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
        //    return JsonSerializer.Deserialize<T>(json);
        //}

    }
}
