using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HalliGalli_Server
{
    // 브로드캐스터

    public class Broadcaster
    {
        public static Broadcaster Instance { get; } = new Broadcaster();

        //public void SendJson<T>(T obj, NetworkStream stream)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        IncludeFields = true, //  필드 포함
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // JSON 네이밍 정책
        //        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        //        WriteIndented = true
        //    };

        //    string json = JsonSerializer.Serialize(obj, options);
        //    byte[] data = Encoding.UTF8.GetBytes(json);
        //    byte[] dataLength = BitConverter.GetBytes(data.Length);

        //    stream.Write(dataLength, 0, 4);
        //    stream.Write(data, 0, data.Length);
        //    stream.Flush();

        //    Console.WriteLine("보낸 JSON:\n" + json);
        //}

        public void SendJson<T>(T obj, NetworkStream stream)
        {
            string json = JsonSerializer.Serialize(obj);
            Console.WriteLine("보낸 JSON: " + json);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(json);
            writer.Flush();
            writer.Close();
            
        }

        // 수정된 BroadcastToAll 메서드
        public void BroadcastToAll(MessageServerToCli message)
        {

            foreach (var kvp in Table.Instance.players)
            {
                Player player = kvp.Value; // KeyValuePair에서 Player 객체를 가져옴
                SendJson(message, player.tcpClient.GetStream());
            }
        }

        // 처음 입장했을 때 이미 참여한 사람들 정보를 뿌림
        public void BroadcastEnternece(Player target)
        {
            foreach (var kvp in Table.Instance.players)
            {
                Player player = kvp.Value; // KeyValuePair에서 Player 객체를 가져옴

                MessageServerToCli message = new MessageServerToCli(
                        player.playerId,
                        player.username,
                        4
                    );

                SendJson(message, target.tcpClient.GetStream());
            }
        }


        public void BroadcastNextTurn(MessageServerToCli message, int currentTurnPlayerId)
        {
            foreach (var kvp in Table.Instance.players)
            {
                Player player = kvp.Value; // KeyValuePair에서 Player 객체를 가져옴
                   
                if(player.playerId == currentTurnPlayerId)
                {
                    message.IsTurnActive = true;
                }
                else
                {
                    message.IsTurnActive = false;
                }

                SendJson(message, player.tcpClient.GetStream());
            }
        }
    }
}
