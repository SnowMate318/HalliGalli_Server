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

    public class Broadcaster
    {
        public static Broadcaster Instance { get; } = new Broadcaster();

        public void SendJson<T>(T obj, NetworkStream stream)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true, //  필드 포함
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

        // 수정된 BroadcastToAll 메서드
        public void BroadcastToAll(Message message)
        {
            foreach (Player player in Table.Instance.players)
            {
                SendJson(message, player.tcpClient.GetStream());
            }
        }

    }
}
