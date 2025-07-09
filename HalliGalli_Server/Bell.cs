using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{

    public class PlayerName_TimeDIf
    {
        public string PlayerName { get; set; }
        public int TimeDif { get; set; }

        public PlayerName_TimeDIf()
        {
            this.PlayerName = "없음";
            this.TimeDif = -1;
        }

        public PlayerName_TimeDIf(string playerName, int timeDif)
        {
            this.PlayerName = playerName;
            this.TimeDif = timeDif;
        }
    }

    // 종 클래스
    public class Bell
    {
        //public bool isActive = false; // 과일 5개가 모였을 때 활성화
                                      // 활성화가 되지 않으면 패널티
                                      // 활성화가 되어있으면 버퍼에 정보저장 후 승리자 판별

        private bool isDeciding = false;
        private List<PlayerName_TimeDIf> TimeDifList;
        private string? winner;
        private Table table;
        //private readonly object lockObj = new();

        public Bell(Table table)
        {
            this.TimeDifList = new List<PlayerName_TimeDIf>();
            this.table = table;
        }

        public void Ring(string playerName, int timeDif)
        {

            if (!isDeciding)
            {
                isDeciding = true;
                new Thread(new ThreadStart(StartDecision)).Start();
            }
            TimeDifList.Add(new PlayerName_TimeDIf(playerName, timeDif));
        }

        public void StartDecision()
        {
            Thread.Sleep(3500);
            string? winner = DecideWinner();
            if (winner == null)
            {
                Broadcaster.Instance.BroadcastToAll(
                    table.MakeMessageServerToCli(GameEvent.GAME_LOSE), table.GetPlayers()); // 9 -> 우승자없음
                return;
            }

            // 쓰레드 종료
            TimeDifList.Clear();
            isDeciding = false;


            //Todo: 테이블 지움
            Broadcaster.Instance.BroadcastWinner(winner, table);

            table.MergeDeck(winner);
            table.CheckWinner(winner);

            return;
        }

        public string? DecideWinner()
        {
            int MinDif = int.MaxValue;
            string? CurWinner = null;

            foreach(PlayerName_TimeDIf pt in TimeDifList)
            {
                if(pt.TimeDif < MinDif)
                {
                    MinDif = pt.TimeDif;
                    CurWinner = pt.PlayerName;
                }
            }


            return CurWinner;
        }
    }
}
