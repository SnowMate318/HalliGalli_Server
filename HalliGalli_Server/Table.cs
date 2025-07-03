using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 테이블 클래스
    public class Table
    {
        // 1) 유일한 인스턴스를 저장할 정적 필드
        private static readonly Table _instance = new Table();

        public List<Player> players;
        public List<Card> openedCards;
        public Dictionary<string, int> fruitCardCount;
        public Queue<Card> tableDeck;
        public int currentTurnPlayerId;
        public Bell bell;
        public bool gameStart = false; // 인원을 더 받을지 트리거로 사용

        private Table()
        {

            fruitCardCount = new Dictionary<string, int>();//사과 바나나 포도 수박 과일 별 카드 수 딕셔너리 생성

            fruitCardCount["사과"] = 0;
            fruitCardCount["바나나"] = 0;
            fruitCardCount["포도"] = 0;
            fruitCardCount["수박"] = 0;

            tableDeck = new Queue<Card>();
            bell = new Bell();
            players = new List<Player>();
            currentTurnPlayerId = 0;
            
        }

        public Table getInstance()
        {
            return _instance;
        }

        public void StartGame()
        {
            //Todo:  56장의 카드를 N명에게 나눠줌, 나머지 정보들 초기화
        }
        public void PlayCard(int playerId)
        {
            // Todo: 플레이어의 카드 덱에서 카드를 한장 뽑아서 테이블의 카드 덱에 넣고,
            // 펼쳐진 카드와 과일 별 카드 갯수를 업데이트함
        }
        public Card[] ShowCurrentResult()
        {
            // Todo: 테이블에 나와있는 N(플레이어 수)개의 카드 배열을 반환

            return null;
        }
        public void MergeDeck(int playerId)
        {
            // Todo: 해당하는 플레이어의 카드덱에 테이블의 카드덱을 합침(이겼을 때 호출) 

        }
        public void MoveTurn()
        {
            // Todo: 카드를 내거나 플레이어가 5초이상 카드를 보여주지 않았을 때 호출
			// 턴 진행중인 유저 id 정보를 업데이트
			// 브로드캐스트 (턴 이동 정보 주기)

        }
        public void ApplyPenalty(int playerId)
        {
            // Todo: 플레이어 카드덱에서 카드를 한장씩 다른 플레이어에게 줌

        }
        public void PlayerDeath(int playerId)
        {
            //Todo: 해당 플레이어를 죽은상태로 바꿈

        }
        public int CheckWinner()
        {
            //Todo: // 살아있는 대상이 둘일 때, 패널티없이 종을 친 사람이 우승
            // 우승자 브로드캐스팅 후 서버 종료 메소드 호출

            return -1;
        }
    }
}
