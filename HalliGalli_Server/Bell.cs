using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{

    public class PlayerId_TimeDIf
    {
        public int PlayerId { get; set; }
        public int TimeDif { get; set; }

        public PlayerId_TimeDIf()
        {
            this.PlayerId = 0;
            this.TimeDif = -1;
        }

        public PlayerId_TimeDIf(int userId, int timeDif)
        {
            this.PlayerId = userId;
            this.TimeDif = timeDif;
        }
    }

    // 종 클래스
    public class Bell
    {
        public bool isActive = false; // 과일 5개가 모였을 때 활성화
                                      // 활성화가 되지 않으면 패널티
                                      // 활성화가 되어있으면 버퍼에 정보저장 후 승리자 판별

        public bool isDeciding = false;

        public List<PlayerId_TimeDIf> TimeDifList;

        private readonly object lockObj = new();

        public Bell()
        {
            this.TimeDifList = new List<PlayerId_TimeDIf>();
        }

        public void Ring(int playerId, int timeDif)
        {
            if (!isActive)
            {
                Table.Instance.ApplyPenalty(playerId);
                return;
            }
            if (!isDeciding)
            {
                isDeciding = true;
                new Thread(new ThreadStart(StartDecision)).Start();
            }
            TimeDifList.Add(new PlayerId_TimeDIf(playerId, timeDif));
        }

        public void StartDecision()
        {
            Thread.Sleep(2000);
            int winner = DecideWinner();
            if (winner == -1)
            {
                // Todo: 브로드캐스팅 (우승자 없음)
            }
            // Todo: 브로드캐스팅 (...번 우승)
            Table.Instance.MergeDeck(winner);

            // 쓰레드 종료
            TimeDifList.Clear();
            isDeciding = false;
            isActive = false;
            return;
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }

        public int DecideWinner()
        {
            int MinDif = int.MaxValue;
            int CurWinner = -1;

            foreach(PlayerId_TimeDIf pt in TimeDifList)
            {
                if(pt.TimeDif < MinDif)
                {
                    MinDif = pt.TimeDif;
                    CurWinner = pt.PlayerId;
                }
            }


            return CurWinner;
        }
    }
}
