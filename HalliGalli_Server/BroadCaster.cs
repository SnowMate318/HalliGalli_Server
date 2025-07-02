using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 브로드캐스터
    public class Broadcaster
    {
        public void SendJson(int playerId, string message) { }

        public void SendJson(int playerId, bool isTurnActive, Card card, int userState, Card[] openedCards) { }
    }
}
