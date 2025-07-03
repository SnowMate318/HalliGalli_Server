using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 관리 클래스 (싱글톤)
    public class Manager
    {
        private const int MAX_THREAD = 4;
        private int currentThreadCount = 0;
        TcpListener server;

        private Manager() {

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
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    if (client != null && CheckUserAvailable())
                    {
                        Thread ClientThread = new Thread(new ParameterizedThreadStart(AddUser));
                        currentThreadCount++;
                        ClientThread.Start(client);

                    }
                }
                catch (IOException ex) {

                    RemoveUser(1); // Todo: 아이디 확인 후 제거 요청 브로드캐스팅 (어케하지..)
                    continue;
                
                }
            }
        }
        public void AddUser(object obj) {
            TcpClient Client = (TcpClient)obj;
            NetworkStream stream = Client.GetStream();
            Player player = new Player(currentThreadCount, stream, Client); //currentcount로 하는이유 어짜피 나갔다 다시 들어왔을때는 게임진행이 안됨
            Table.Instance.AddPlayer(player);
            while (true) {
            
                
            
            }
            
        }
        public bool CheckUserAvailable() {
            return currentThreadCount < MAX_THREAD;
              
        }
        public void RemoveUser(int playerId) {

            currentThreadCount--;
            Table.Instance.PlayerDeath(playerId);

        }

        
    }
}
