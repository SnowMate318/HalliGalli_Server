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
        public bool isActive = false;// 과일 5개가 모였을 때 활성화
                                     // 활성화가 되지 않으면 패널티
                                     // 활성화가 되어있으면 버퍼에 정보저장 후 승리자 판별
        public Dictionary<int, double> buffer = new();

        public void Ring(int playerId)
        {
            // Todo: 활성화가 안된 상태일때는 패널티 부여 (다른 유저에게 카드 1장씩 주기)
            // 활성화가 된 상태일때는 버퍼에다 정보 저장
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
            // Todo: 버퍼에 있는 정보 중 시간차가 가장 작은 유저의 id 반환
            // 버퍼 비움
            return -1;
        }
    }
}
