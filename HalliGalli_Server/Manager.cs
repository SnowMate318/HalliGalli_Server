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

        bool[] idBox = {false,false,false,false};

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

                TcpClient client = server.AcceptTcpClient();
                if (client != null && CheckUserAvailable())
                {
                    Thread ClientThread = new Thread(new ParameterizedThreadStart(UserTCPStart));
                    lock (threadLock)
                    {
                        currentThreadCount++;
                    }
                    ClientThread.Start(client);

                }
            }
        }
        public void UserTCPStart(object obj) // 유저 추가시 
        {
            TcpClient Client = (TcpClient)obj;
            NetworkStream stream = Client.GetStream();

            int playerid = 0;
            for(int i = 0; i < 4; i++)
            {
                if (!idBox[i])
                {
                    lock (threadLock) {
                        idBox[i] = true;
                    }
                    playerid = i;
                    break;
                }
            }

            Player player = new Player(playerid, stream, Client); 
            MessageCliToServer msg = player.ReceiveJson<MessageCliToServer>();

            if (msg!=null)
            {
                if (Table.Instance.players[msg.playerName] != null)
                {

                    throw new Exception("이름 중복");
                }
                player.username = msg.playerName;
            }

            Table.Instance.AddPlayer(player); // 수정

            try
            {
                while (true)
                {
                    msg = player.ReceiveJson<MessageCliToServer>();
                    if (msg != null) {

                        if(!gamestart && msg.key == "p")
                        {
                            gamestart = true;
                            continue;
                        }

                        if (!gamestart) continue;
                        player.ReceiveUserInfo(msg);// 메세지를 받았을때 로직을 처리
                                                    // 상태 기준 판단
                                                    // 상대가 누른 키를 기준으로 판단


                        // 테스트(쓰레기값 주기)
                        // Broadcaster.Instance.BroadcastToAll(new MessageServerToCli());
                    }


                }
            }
            catch (IOException)
            {
                Console.WriteLine($"{player.playerId} 연결 종료");
                Broadcaster.Instance.BroadcastToAll(new MessageServerToCli(player.playerId, player.username, 5)); // 5 -> 퇴장
            }
            catch (Exception e)
            {
                if(e.Message=="이름 중복")
                {
                    Broadcaster.Instance.BroadcastToAll(new MessageServerToCli(6)); // 6 -> 이름 중복
                }
            }
            finally
            {
                RemoveUser(player);
            }

        }
        public bool CheckUserAvailable()
        {
            lock (threadLock)
            {
                return (!gamestart && currentThreadCount < MAX_THREAD);
            }

        }
        public void RemoveUser(Player player)
        {

            lock (threadLock)
            {
                idBox[player.playerId] = false;
                currentThreadCount--;
            }

            Table.Instance.PlayerDeath(player.username);
            Table.Instance.players.Remove(player.username);
            player.stream.Close();
            player.tcpClient.Close();
        }

        
    }
}
