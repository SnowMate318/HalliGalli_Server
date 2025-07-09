using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 테이블 클래스
    public class Table
    {

        //public static Table Instance { get; } = new Table(); // 1) 유일한 인스턴스를 저장할 정적 필드

        public Dictionary<string, Player> players; //string은 플레이어 이름, Player는 플레이어 객체

        public Dictionary<string, int> fruitCardCount;
        public Queue<Card> tableDeck;
        
        public int currentTurnPlayerId;
        public Bell bell;


        public bool gameStart = false; // 인원을 더 받을지 트리거로 사용
        public List<string> playerOrder; // 플레이어 순서 저장용
        public Card[] openedCards;


        public Table()
        {
            fruitCardCount = new Dictionary<string, int>();//사과 바나나 포도 수박 과일 별 카드 수 딕셔너리 생성

            fruitCardCount["사과"] = 0;
            fruitCardCount["바나나"] = 0;
            fruitCardCount["포도"] = 0;
            fruitCardCount["수박"] = 0;

            tableDeck = new Queue<Card>();
            players = new Dictionary<string, Player>();
            playerOrder = new List<string>();
            openedCards = new Card[4];

            bell = new Bell(this);
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
                string playerName = playerOrder[i];
                players[playerName].cardDeck.deck.Clear(); // 카드덱의 큐 초기화
                int takeCount = cardsPerPlayer + (i < extraCards ? 1 : 0);
                foreach (var card in allCards.Take(takeCount))
                    players[playerName].cardDeck.deck.Enqueue(card);
                allCards.RemoveRange(0, takeCount);
            }


            foreach (string fruit in fruits)
            {
                fruitCardCount[fruit] = 0;
            }
            tableDeck.Clear();
            currentTurnPlayerId = 0;
            string currentPlayerName = playerOrder[currentTurnPlayerId];
            Player currentPlayer = players[currentPlayerName];
            gameStart = true;
            PlayCard(currentPlayer.Name);

        }

        public void AddPlayer(Player player)
        {
            string playerName = player.Name;
            players.Add(playerName, player);
            playerOrder.Add(playerName); // 플레이어 순서 리스트에도 추가!

        }


        public void PlayCard(string playerName)
        {
            // 1. 플레이어 찾기 (이름으로 바로 접근)
            Player? player = GetPlayer(playerName);

            if (player == null) // 플레이어가 죽거나 나갔을 경우
            {
                MoveTurn();
                ShowCurrentResult(); // 현재 결과를 클라에 뿌려주기
                return;
            }

            if (player.cardDeck.Size() == 0)
            {
                PlayerDeath(playerName); // 죽음 처리
                MoveTurn();
                ShowCurrentResult(); // 현재 결과를 클라에 뿌려주기
                return ;
            }

            // 2. 카드 한 장 뽑기
            Card card = player.cardDeck.DrawCard();

            // 3. 테이블 덱에 카드 올리기
            // 4. 펼쳐진 카드 목록에 추가
            // 5. 과일별 카드 개수 업데이트 
            UpdateCardTable(player, card);
            MoveTurn(); // 턴 바꾸고 그 결과를 클라이언트에 뿌려줌?
            ShowCurrentResult(); // 현재 결과를 클라에 뿌려주기
            CheckWinner();

        }

        public Player? GetPlayer(string playerName)
        {
            
            if (!players.ContainsKey(playerName))
            {
                // 해당 플레이어 존재하지 않음
                Debug.WriteLine("플레이어 존재하지 않음: 턴 이동");
                return null;
            }

            Player player = players[playerName];
            if (!player.isAlive)
            {
                //이미 탈락한 플레이어
                Debug.WriteLine("플레이어 죽음: 턴 이동");
                return null;
            }

            return player;
        }

        // 테이블에있는 카드정보 업데이트
        public void UpdateCardTable(Player player, Card card)
        {
            
            tableDeck.Enqueue(card);
            
            openedCards[player.Id] = card;

            
            if (fruitCardCount.ContainsKey(card.fruitType))
                fruitCardCount[card.fruitType] += card.count;
            else
                fruitCardCount[card.fruitType] = card.count;
        }

        public void ShowCurrentResult()
        {

            // 브로드캐스팅int playerId, string playerName, Card card, int userState, Card[] openCards
            MessageCard[] messageCards = getMessageCards();
            Broadcaster.Instance.BroadcastCurrentTurn(this);

        }

        public MessageCard[] getMessageCards()
        {
            MessageCard[] messageCards = new MessageCard[players.Count];
            foreach (Player player in GetPlayers())
            {
                messageCards[player.Id] = new MessageCard(player.Id, openedCards[player.Id].getNum());
            }
            return messageCards;
        }


        public MessageCardCount[] GetAllPlayerCardCounts()
        {
            int size = players.Count;
            MessageCardCount[] res = new MessageCardCount[size]; 

            int index = 0;
            foreach (var player in players.Values)
            {
                res[index++] = new MessageCardCount(player.Id, player.cardDeck.deck.Count);
            }

            return res;
        }


        public void MergeDeck(string playerName)
        {
            // 1. 플레이어 찾기
            if (!players.ContainsKey(playerName))
            {
                Debug.WriteLine("카드를 합칠 사용자가 없습니다.");
                return;
            }
                

            Player player = players[playerName];

            // 2. 테이블 덱의 모든 카드를 플레이어 카드덱에 추가
            while (tableDeck.Count > 0)
            {
                Card card = tableDeck.Dequeue();
                player.cardDeck.AddCard(card); // CardDeck의 AddCard 사용
            }

            // 3. 펼쳐진 카드 목록 초기화
            foreach(Card card in openedCards)
            {
                card.ClearCard();
            }

            // 4. 과일별 카드 개수 초기화
            foreach (var key in fruitCardCount.Keys.ToList())
            {
                fruitCardCount[key] = 0;
            }
        }

        public void MoveTurn()
        {

            int playerCount = players.Count;
            if (playerCount == 0)
                return; // 플레이어가 없으면 아무것도 하지 않음

            int nextTurn = currentTurnPlayerId;

            // 다음 턴을 가진 플레이어를 찾음 (탈락한 플레이어는 건너뜀)
            for (int i = 1; i <= playerCount; i++)
            {
                int candidate = (currentTurnPlayerId + i) % playerCount;
                string candidateName = playerOrder[candidate];
                Player candidatePlayer = players[candidateName];

                if (candidatePlayer.isAlive)
                {
                    nextTurn = candidate;
                    break;
                }
            }


            currentTurnPlayerId = nextTurn;

        }
        public void ApplyPenalty(string playerName)
        {
            Player player = players[playerName];

            // 1. 패널티를 받을 플레이어 찾기
            if (!players.ContainsKey(playerName))
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");

            Player penaltyPlayer = players[playerName];

            // 2. 다른 모든 플레이어에게 한 장씩 카드를 줌
            foreach (var otherEntry in players)
            {
                if (otherEntry.Key == playerName) continue; // 자기 자신은 건너뛰기
                if (!otherEntry.Value.isAlive) continue; // 죽으면 카드 주지말기

                Player other = otherEntry.Value;

                Card card = penaltyPlayer.cardDeck.DrawCard();
                if (card != null)
                    other.cardDeck.AddCard(card); // 다른 플레이어에게 카드 추가
                else
                {
                    PlayerDeath(playerName); // 카드가 없으면 플레이어 죽음 처리
                    break;
                }
            }

            // 브로드캐스트 패널티유저
            Broadcaster.Instance.BroadcastToAll(
                           MakeMessageServerToCli(player.Id, player.Name, GameEvent.PENALTY), GetPlayers()); //패널티
            CheckWinner();

        }

        public void PlayerDeath(string playerName)
        {
            Player player = players[playerName];

            // 해당 플레이어를 죽은 상태로 바꿈
            player.isAlive = false;


            if (!players.ContainsKey(playerName))
                throw new InvalidOperationException("해당 플레이어가 존재하지 않습니다.");

            players[playerName].isAlive = false;

            // 브로드캐스트 유저사망
            Broadcaster.Instance.BroadcastToAll(
                           MakeMessageServerToCli(player.Id, player.Name, GameEvent.GAME_LOSE), GetPlayers()); //우승
        }


        public void  CheckWinner(string bellPlayerName = null)
        {
            // 살아있는 플레이어만 추출
            var alivePlayers = players.Values.Where(p => p.isAlive).ToList();
            Player? winner = null;
            // 1명 남았으면 그 사람이 우승

            if (alivePlayers.Count == 1)
            {
                winner = alivePlayers[0];
            }

            // 2명 남았고, bellPlayerName(종 친 사람)이 있으면 그 사람이 우승
            if (alivePlayers.Count == 2 && !string.IsNullOrEmpty(bellPlayerName))
            {
                if (players.ContainsKey(bellPlayerName) && players[bellPlayerName].isAlive)
                {
                    winner = players[bellPlayerName];
                   
                }
            }

            if (winner != null) {
                Broadcaster.Instance.BroadcastToAll(
                           MakeMessageServerToCli(winner.Id, winner.Name, GameEvent.GAME_WIN), GetPlayers()); //8 -> 우승
                gameStart = false;

            }

        }
        public MessageServerToCli MakeMessageServerToCli(int Id, string name, GameEvent gameEvent)
        {
            return new MessageServerToCli(

                    Id,
                    name,
                    (int)gameEvent,
                    getMessageCards(),
                    GetAllPlayerCardCounts()

                );
        }
        public MessageServerToCli MakeMessageServerToCli(int Id, string name, bool turn, GameEvent gameEvent)
        {
            return new MessageServerToCli(

                    Id,
                    name,
                    turn,
                    (int)gameEvent,
                    getMessageCards(),
                    GetAllPlayerCardCounts()

                );
        }
        public MessageServerToCli MakeMessageServerToCli(GameEvent gameEvent)
        {
            return new MessageServerToCli(

                    (int)gameEvent,
                    getMessageCards(),
                    GetAllPlayerCardCounts()

                );
        }

        public List<Player> GetPlayers()
        {
            return players.Values.ToList();
        }
    }

}
