using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 종 클래스
    public class Bell
    {
        public bool isActive = false;
        public Dictionary<int, double> buffer = new();

        public void Ring(int playerId) { }
        public void Activate() { isActive = true; }
        public void Deactivate() { isActive = false; }
        public int DecideWinner() { return -1; }
    }
}
