using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//1. 승리, 2.패배, 3.패널티 4.입장  5.퇴장 6.이름 중복 7.게임시작 




namespace HalliGalli_Server
{
    // 관리 클래스 (싱글톤)
    public class Manager
    {
        private readonly object threadLock = new object();
        private const int MAX_THREAD = 4;
        private int currentThreadCount = 0;
        TcpListener server;
        bool gamestart = false;

        bool[] idBox = { false, false, false, false };

        private Manager()
        {

            server = new TcpListener(IPAddress.Any, 1234);

            Console.WriteLine("서버 시작");
            server.Start();
        }
        public static Manager Instance { get; } = new Manager();


        public void Init()
        {

            Thread thread = new Thread(new ThreadStart(ServerOpen));
            thread.Start();

        }

        public void ServerOpen()
        {
            while (true)
            {
                Table table = new Table(); // 게임 시작에 필요한 테이블

                TcpClient client = server.AcceptTcpClient();
                if (client != null && CheckUserAvailable())
                {
                    Thread ClientThread = new Thread(new ParameterizedThreadStart(UserTCPStart));
                    lock (threadLock)
                    {
                        currentThreadCount++;
                    }
                    ClientThread.Start((client, table));

                }
            }
        }
        public void UserTCPStart(object obj) // 유저 추가시 
        {
            var (client, table) = ((TcpClient, Table))obj;
            NetworkStream stream = client.GetStream();

            int playerId = getPlayerId();

            Player player = new Player(playerId, stream, client);
            MessageCliToServer msg = Receiver.Instance.ReceiveJson<MessageCliToServer>(player.streamManager.reader);

            try
            {
                if (msg == null)
                    throw new Exception("연결 실패");

                if (string.IsNullOrWhiteSpace(msg.name))
                    throw new Exception("이름이 비어 있습니다.");

                if (table.players.ContainsKey(msg.name))
                    throw new Exception("이름 중복");

                player.Name = msg.name;

                table.AddPlayer(player); // 수정

                MessageServerToCli callBack = table.MakeMessageServerToCli(
                        playerId,
                        player.Name,
                        GameEvent.ENTER
                    );

                Console.WriteLine(callBack.ToString());

                Broadcaster.Instance.BroadcastToAll(callBack, table.players.Values.ToList());
                Broadcaster.Instance.BroadcastEnternece(player,table);

                while (true)
                {

                    msg = Receiver.Instance.ReceiveJson<MessageCliToServer>(player.streamManager.reader);
                    if (msg != null)
                    {

                        if (!gamestart && msg.key == 3) // p->3
                        {
                            gamestart = true;
                            table.StartGame();
                            continue;
                        }

                        if (!gamestart) continue;
                        player.ReceiveUserInfo(msg);// 메세지를 받았을때 로직을 처리
                                                    // 상태 기준 판단
                                                    // 상대가 누른 키를 기준으로 판단

                    }
                }
            }
            catch (IOException) // 플레이어와 연결이 끊겼을때
            {
                Console.WriteLine($"{player.Id} 연결 종료");
                RemoveUser(player,table);
                Broadcaster.Instance.BroadcastToAll(table.MakeMessageServerToCli(
                    player.Id, player.Name, GameEvent.GAME_LOSE), table.players.Values.ToList()); // 9 -> 퇴장
            }
            catch (Exception e)
            {
                if (e.Message == "이름 중복" || e.Message == "이름이 비어 있습니다.")
                {
                    Broadcaster.Instance.BroadcastToAll(
                        table.MakeMessageServerToCli(GameEvent.DUP_NAME), table.players.Values.ToList()); // 6 -> 이름 중복
                }
                else
                {
                    Console.WriteLine("유저 연결: 예상치 못한 오류" + e.Message);
                }
            }

        }
        public bool CheckUserAvailable()
        {
            lock (threadLock)
            {
                return (!gamestart && currentThreadCount < MAX_THREAD);
            }

        }
        public void RemoveUser(Player player, Table table)
        {

            lock (threadLock)
            {
                idBox[player.Id] = false;
                currentThreadCount--;
            }

            table.PlayerDeath(player.Name);
            table.players.Remove(player.Name);
            player.streamManager.Dispose();
        }

        private int getPlayerId()
        {
            int playerid = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!idBox[i])
                {
                    lock (threadLock)
                    {
                        idBox[i] = true;
                    }
                    playerid = i;
                    break;
                }
            }
            return playerid;
        }


    }
}
