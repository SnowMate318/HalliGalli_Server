using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    public enum GameEvent
    {
        None = 0,
        WIN,
        LOSE,
        PENALTY,
        ENTER,
        EXIT,
        DUP_NAME,
        GAME_START,
        GAME_WIN, //7
        GAME_LOSE
    }
}
