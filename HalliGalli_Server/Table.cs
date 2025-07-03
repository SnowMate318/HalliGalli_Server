using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 테이블 클래스
    // 테이블 클래스
    public class Table
    {

        public static Table Instance { get; } = new Table(); // 1) 유일한 인스턴스를 저장할 정적 필드

        public List<Player> players;
        public List<Card> openedCards;
        public Dictionary<string, int> fruitCardCount;
        public Queue<Card> tableDeck;
        public int currentTurnPlayerId;
        //public Bell bell;
        public bool gameStart = false; // 인원을 더 받을지 트리거로 사용

        private Table()
        {
            fruitCardCount = new Dictionary<string, int>();//사과 바나나 포도 수박 과일 별 카드 수 딕셔너리 생성

            fruitCardCount["사과"] = 0;
            fruitCardCount["바나나"] = 0;
            fruitCardCount["포도"] = 0;
            fruitCardCount["수박"] = 0;

            tableDeck = new Queue<Card>();
            //bell = new Bell();
            players = new List<Player>();
            openedCards = new List<Card>();
            currentTurnPlayerId = 0;
        }
        public void StartGame()
        {
            //Todo:  56장의 카드를 N명에게 나눠줌, 나머지 정보들 초기화
            List<Card> allCards = new List<Card>();
            string[] fruits = { "apple", "banana", "grape", "watermelon" };
            int[] cardCounts = { 5, 4, 3, 1, 1 };

            /*
             5-1개 4-1개 3-3개 2-4개 1-5개  각 과일 별로 구성 
             */
            foreach (string fruit in fruits)
            {
                int number = 1;
                foreach (int count in cardCounts)
                {
                    for (int i = 0; i < count; i++)
                    {
                        allCards.Add(new Card(fruit, number));
                    }
                    number++;
                }
            }

            //카드 섞기 
            Random rand = new Random();
            allCards = allCards.OrderBy(x => rand.Next()).ToList();

            // 플레이어에게 카드 나누기
            int playerCount = players.Count;
            int cardsPerPlayer = allCards.Count / playerCount;
            int extraCards = allCards.Count % playerCount;
            for (int i = 0; i < playerCount; i++)
            {
                players[i].cardDeck.deck.Clear(); // 카드덱의 큐 초기화
                int takeCount = cardsPerPlayer + (i < extraCards ? 1 : 0);
                foreach (var card in allCards.Take(takeCount))
                    players[i].cardDeck.deck.Enqueue(card);
                allCards.RemoveRange(0, takeCount);
            }


            // 게임 상태 초기화
            openedCards.Clear();
            foreach (string fruit in fruits)
            {
                fruitCardCount[fruit] = 0;
            }
            tableDeck.Clear();
            currentTurnPlayerId = 0;
            gameStart = true;

        }
        public void AddPlayer(Player player)
        {
            if (gameStart)
                throw new InvalidOperationException("게임이 이미 시작되어 플레이어를 추가할 수 없습니다.");
            if (players.Any(p => p.playerId == player.playerId))
                throw new InvalidOperationException("이미 등록된 플레이어입니다.");
            if (players.Count >= 4) // 최대 인원 제한, 필요 없다면 생략
                throw new InvalidOperationException("최대 인원 초과입니다.");
            this.players.Add(player);

        }

        public void PlayCard(int playerId)
        {
            // Todo: 플레이어의 카드 덱에서 카드를 한장 뽑아서 테이블의 카드 덱에 넣고,
            // 펼쳐진 카드와 과일 별 카드 갯수를 업데이트함
            // 1. 플레이어 찾기
            Player player = players.FirstOrDefault(p => p.playerId == playerId);
            if (player == null)
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");

            // 2. 카드 한 장 뽑기
            Card card = player.cardDeck.DrawCard(); // CardDeck 클래스 사용 기준
            if (card == null)
                throw new InvalidOperationException("플레이어의 카드가 모두 소진되었습니다.");

            // 3. 테이블 덱에 카드 올리기
            tableDeck.Enqueue(card);

            // 4. 펼쳐진 카드 목록에 추가
            openedCards.Add(card);

            // 과일별 카드 개수 업데이트
            if (fruitCardCount.ContainsKey(card.fruitType))
                fruitCardCount[card.fruitType] += card.count;
            else
                fruitCardCount[card.fruitType] = card.count; ;

        }
        public Card[] ShowCurrentResult()
        {
            // Todo: 테이블에 나와있는 N(플레이어 수)개의 카드 배열을 반환
            int PlayerCount = players.Count;
            //아직 모든 플레이어가 카드를 내지 않았을때. 낸사람만 표시
            if (openedCards.Count < players.Count) return openedCards.ToArray();
            //마지막 N장(플레이어 수만큼 반환)
            return openedCards.Skip(openedCards.Count - PlayerCount).ToArray();
        }
        public void MergeDeck(int playerId)
        {
            // Todo: 해당하는 플레이어의 카드덱에 테이블의 카드덱을 합침(이겼을 때 호출) 
            // 1. 플레이어 찾기
            Player player = players.FirstOrDefault(p => p.playerId == playerId);
            if (player == null)
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");

            // 2. 테이블 덱의 모든 카드를 플레이어 카드덱에 추가
            while (tableDeck.Count > 0)
            {
                Card card = tableDeck.Dequeue();
                player.cardDeck.AddCard(card); // CardDeck의 AddCard 사용
            }

            // 3. 펼쳐진 카드 목록 초기화
            openedCards.Clear();

            // 4. 과일별 카드 개수 초기화
            foreach (var key in fruitCardCount.Keys.ToList())
            {
                fruitCardCount[key] = 0;
            }
        }
        public void MoveTurn()
        {
            // Todo: 카드를 내거나 플레이어가 5초이상 카드를 보여주지 않았을 때 호출
            // 턴 진행중인 유저 id 정보를 업데이트


            int playerCount = players.Count;
            if (playerCount == 0)
                return; // 플레이어가 없으면 아무것도 하지 않음

            int nextTurn = currentTurnPlayerId;

            // 다음 턴을 가진 플레이어를 찾음 (탈락한 플레이어는 건너뜀)
            for (int i = 1; i <= playerCount; i++)
            {
                int candidate = (currentTurnPlayerId + i) % playerCount;
                Player candidatePlayer = players[candidate];

                // 플레이어가 살아있고(예: 카드가 남아있음) 게임에 참여 중이라면
                if (candidatePlayer.isAlive)
                {
                    nextTurn = candidate;
                    break;
                }
            }

            currentTurnPlayerId = nextTurn;

            //태현 to do 
            // 브로드캐스트: 턴이 바뀌었음을 모든 클라이언트에 알림(턴 이동 정보 주기)
            // 예: BroadcastTurnChanged(currentTurnPlayerId);

        }
        public void ApplyPenalty(int playerId)
        {
            // Todo: 플레이어 카드덱에서 카드를 한장씩 다른 플레이어에게 줌
            Player penaltyPlayer = players.FirstOrDefault(p => p.playerId == playerId);
            if (penaltyPlayer == null)
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");
            foreach (Player other in players)
            {
                if (other.playerId == playerId) continue; // 자기 자신은 건너뛰기
                Card card = penaltyPlayer.cardDeck.DrawCard();
                if (card != null) other.cardDeck.AddCard(card); // 다른 플레이어에게 카드 추가
                else
                {
                    PlayerDeath(playerId); // 카드가 없으면 플레이어 죽음 처리
                    break; // 더이상 줄 카드가 없으면 종료
                }
            }
        }
        public void PlayerDeath(int playerId)
        {
            //Todo: 해당 플레이어를 죽은상태로 바꿈
            Player player = players.FirstOrDefault(p => p.playerId == playerId);
            if (player == null)
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");

            player.isAlive = false;
        }
        //public int CheckWinner()
        //{
        //    //Todo: // 살아있는 대상이 둘일 때, 패널티없이 종을 친 사람이 우승
        //    // 우승자 브로드캐스팅 후 서버 종료 메소드 호출

        //    return -1;
        //}
        public int CheckWinner(int? bellPlayerId = null)
        {
            // 1. 살아있는 플레이어 찾기
            var alivePlayers = players.Where(p => p.isAlive).ToList();

            // 2. 1명 남았으면 그 사람이 우승
            if (alivePlayers.Count == 1)
            {
                int winnerId = alivePlayers[0].playerId;
                //BroadcastWinner(winnerId); // (선택) 우승자 알림
                //EndGame(); // (선택) 서버 종료 등
                return winnerId;
            }

            // 3. 2명 남았고, bellPlayerId(종 친 사람)가 있으면 그 사람이 우승
            if (alivePlayers.Count == 2 && bellPlayerId.HasValue)
            {
                int winnerId = bellPlayerId.Value;
                //BroadcastWinner(winnerId); // (선택)
                //EndGame(); // (선택)
                return winnerId;
            }

            // 4. 아직 우승자 없음
            return -1;
        }
    }
}
