using HalliGalli_Server;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server_Test
{


    [TestFixture]
    public class CardTests
    {
        [Test]
        [Description("기본 생성자는 fruitType을 '없음', count를 0으로 초기화해야 한다.")]
        public void 기본생성자_기본값확인()
        {
            var card = new Card();

            ClassicAssert.AreEqual("없음", card.fruitType);
            ClassicAssert.AreEqual(0, card.count);
        }

        [Test]
        [Description("매개변수 생성자는 지정된 과일 이름과 개수로 초기화되어야 한다.")]
        public void 매개변수생성자_정상초기화()
        {
            var card = new Card("apple", 3);

            ClassicAssert.AreEqual("apple", card.fruitType);
            ClassicAssert.AreEqual(3, card.count);
        }

        [TestCase("없음", 0, ExpectedResult = -5, TestName = "getNum_기본생성자는_-5반환")]
        [TestCase("apple", 1, ExpectedResult = 1, TestName = "getNum_사과1개는_1반환")]
        [TestCase("apple", 5, ExpectedResult = 5, TestName = "getNum_사과5개는_5반환")]
        [TestCase("banana", 1, ExpectedResult = 6, TestName = "getNum_바나나1개는_6반환")]
        [TestCase("banana", 5, ExpectedResult = 10, TestName = "getNum_바나나5개는_10반환")]
        [TestCase("grape", 1, ExpectedResult = 11, TestName = "getNum_포도1개는_11반환")]
        [TestCase("grape", 5, ExpectedResult = 15, TestName = "getNum_포도5개는_15반환")]
        [TestCase("watermelon", 1, ExpectedResult = 16, TestName = "getNum_수박1개는_16반환")]
        [TestCase("watermelon", 5, ExpectedResult = 20, TestName = "getNum_수박5개는_20반환")]


        [Description("과일 이름에 따라 점수 계산 로직이 올바른지 테스트")]
        public int getNum_과일별점수계산(string fruitType, int count)
        {
            var card = new Card(fruitType, count);
            return card.getNum();
        }

        [Test]
        [Description("ClearCard()를 호출하면 카드의 fruitType은 '없음', count는 0으로 초기화되어야 한다.")]
        public void 카드초기화_동작확인()
        {
            var card = new Card("grape", 4);
            card.ClearCard();

            ClassicAssert.AreEqual("없음", card.fruitType);
            ClassicAssert.AreEqual(0, card.count);
        }

    }
}
