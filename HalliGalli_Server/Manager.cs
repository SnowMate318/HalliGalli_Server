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
                    Thread ClientThread = new Thread(new ParameterizedThreadStart(UserTCPStert));
                    lock (threadLock)
                    {
                        currentThreadCount++;
                    }
                    ClientThread.Start(client);

                }
            }
        }
        public void UserTCPStert(object obj) // 유저 추가시 
        {
            TcpClient Client = (TcpClient)obj;
            NetworkStream stream = Client.GetStream();
            Player player = new Player(currentThreadCount, stream, Client); //currentcount로 하는이유 어짜피 나갔다 다시 들어왔을때는 게임진행이 안됨
            Table.Instance.AddPlayer(player);

            try
            {
                while (true)
                {
                    MessageCliToServer msg = player.ReceiveJson<MessageCliToServer>();
                    if (msg != null) {

                        if(!gamestart && msg.key == "p")
                        {
                            gamestart = true;
                            continue;
                        }



                        // todo:
                        // 메세지를 받았을때 로직을 처리
                        // 상태 기준 판단
                        // 상대가 누른 키를 기준으로 판단


                        // 테스트(쓰레기값 주기)
                        Broadcaster.Instance.BroadcastToAll(new Message());
                    }


                }
            }
            catch (IOException)
            {
                Console.WriteLine($"{player.playerId} 연결 종료");
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
                currentThreadCount--;
            }

            Table.Instance.PlayerDeath(player.playerId);
            //Todo: Table.Instance.deletePlayer(player.playerId);
            player.stream.Close();
            player.tcpClient.Close();
        }

        
    }
}
