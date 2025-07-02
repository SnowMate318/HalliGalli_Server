using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 카드 클래스
    public class Card
    {
        public string fruitType;
        public int count;

        public Card(string fruitType, int count)
        {
            this.fruitType = fruitType;
            this.count = count;
        }
    }
}
