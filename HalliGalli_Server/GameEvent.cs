using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    public enum GameEvent
    {
        WIN = 1,
        LOSE,
        PENALTY,
        ENTER,
        EXIT,
        DUP_NAME,
        GAME_START
    }
}
