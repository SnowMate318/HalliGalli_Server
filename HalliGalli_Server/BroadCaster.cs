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

        public void SendJson<T>(T obj, StreamWriter writer, string name)
        {
            string json = JsonSerializer.Serialize(obj);
            try
            {
                writer.WriteLine(json);
                writer.Flush();
                Console.WriteLine(name + "에게 보낸 JSON: " + json);
                Thread.Sleep(30);
            }
            catch (Exception e)
            {
                Console.WriteLine("전송 실패: " + e.Message);
            }
        }

        // 수정된 BroadcastToAll 메서드
        public void BroadcastToAll(MessageServerToCli message, List<Player> players)
        {

            foreach (Player player in players)
            {
                SendJson(message, player.streamManager.writer, player.Name);
            }
        }
        public void BroadcastWinner(string winnerName, Table table)
        {
            foreach (Player player in table.players.Values.ToList())
            {
                GameEvent gameEvnet = GameEvent.WIN; // 승리자일 경우
                if (!player.Name.Equals(winnerName))
                {
                    gameEvnet = GameEvent.LOSE; // 패배자일 경우
                }

               
                MessageServerToCli message = table.MakeMessageServerToCli(
                    player.Id, 
                    player.Name,
                    (player.Id==table.currentTurnPlayerId),
                    gameEvnet
                );

                SendJson(message, player.streamManager.writer, player.Name);
            }
        }

        // 처음 입장했을 때 이미 참여한 사람들 정보를 뿌림
        public void BroadcastEnternece(Player target, Table table)
        {
            Console.WriteLine("타겟 플레이어: "+target.Name);
            foreach (Player player in table.GetPlayers())
            {

                MessageServerToCli message = table.MakeMessageServerToCli(
                        player.Id,
                        player.Name,
                        GameEvent.ENTER
                    );
                
                SendJson(message, target.streamManager.writer, target.Name);
                Thread.Sleep(500);
            }
            Console.WriteLine("---끝---");
        }


        public void BroadcastCurrentTurn(Table table)
        {
            foreach (Player player in table.GetPlayers())
            {
                
                MessageServerToCli msg = table.MakeMessageServerToCli(

                        player.Id,
                        player.Name,
                        (player.Id == table.currentTurnPlayerId),
                        GameEvent.None

                    );

                SendJson(msg, player.streamManager.writer, player.Name);
            }
        }



    }
}

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