using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarGameApi
{
    public class Card
    {
        private SuitType suit;
        private string rank;

        public enum SuitType{
            Hearts,
            Tiles,
            Clovers,
            Pikes
        }

        public SuitType Suit { get => suit; set => suit = value; }
        public string Rank { get => rank; set => rank = value; }

        public Card(SuitType suit, string rank)
        {
            this.suit = suit;
            this.rank = rank;
        }

        public int Compare(Card card)
        {
            int res;

            if (this.CardWeight > card.CardWeight)
                res = 1;
            else if (this.CardWeight < card.CardWeight)
                res = 2;
            else
                res = 0;

            return res;
        }

        private int CardWeight
        {
            get {
                int weight;

                switch (rank)
                {
                    case "ace":
                        weight = 14;
                        break;
                    case "king":
                        weight = 13;
                        break;
                    case "queen":
                        weight = 12;
                        break;
                    case "joker":
                        weight = 11;
                        break;
                    default:
                        if (!int.TryParse(rank, out weight))
                            throw new Exception("Invalid rank of Card.");
                        break;
                }

                return weight;
            }
        }

        public override string ToString()
        {
            return rank + " of " + suit;
        }

    }
}
