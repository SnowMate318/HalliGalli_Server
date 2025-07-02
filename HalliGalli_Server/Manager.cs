using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 관리 클래스 (싱글톤)
    public class Manager
    {
        public static Manager Instance { get; } = new Manager();

        private int maxThread;
        private int currentThread;
        private object server;

        public void AddUser() { }
        public void CheckUserAvailable() { }
        public void RemoveUser(int playerId) { }
    }
}
