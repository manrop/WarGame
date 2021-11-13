using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WarGameApi
{
    public class Game
    {
        private string player1, player2;

        public void StartGame(string p1, string p2)
        {
            player1 = p1;
            player2 = p2;

            List<Card> p1Cards, p2Cards;

            //Get initial Deck of Cards
            List<Card> cards = InitialCards();
            //Debug.WriteLine("Initial Cards:");
            //WriteCards(cards);

            //Shuffle card deck
            ShuffleCards(cards);
            Debug.WriteLine("Shuffled Cards:");
            WriteCards(cards);

            //Deal Cards to each Player
            p1Cards = new List<Card>();
            p2Cards = new List<Card>();
            while (cards.Count > 0)
            {
                p1Cards.Add(cards.First());
                cards.Remove(cards.First());
                p2Cards.Add(cards.First());
                cards.Remove(cards.First());
            }

            Debug.WriteLine("P1Cards:");
            WriteCards(p1Cards);

            Debug.WriteLine("P2Cards:");
            WriteCards(p2Cards);

            //Play The Game
            PlayGame(p1Cards, p2Cards);

        }

        private void PlayGame(List<Card> p1Cards, List<Card> p2Cards)
        {
            List<Card> p1Pile = new List<Card>();
            List<Card> p2Pile = new List<Card>();

            while (p1Cards.Count > 0 && p2Cards.Count > 0)
            {
                int roundWinner = 0;

                p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());
                p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Pile.First());

                roundWinner = p1Pile.Last().Compare(p2Pile.Last());

                //Same Rank (WAR)
                while (roundWinner == 0)
                {
                    if(p1Cards.Count < 2) // Not enought cards to War
                    {
                        p1Pile.AddRange(p1Cards);
                        roundWinner = 2;
                        break;
                    }
                    if(p2Cards.Count < 2) // Not enought cards to War
                    {
                        p2Pile.AddRange(p2Cards);
                        roundWinner = 1;
                        break;
                    }
                        
                    p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());
                    p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Pile.First());

                    p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());
                    p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Pile.First());

                    roundWinner = p1Pile.Last().Compare(p2Pile.Last());
                }

                //p1 Wins
                if (roundWinner == 1)
                {
                    p1Pile.Reverse();
                    p1Cards.AddRange(p1Pile);
                    p2Pile.Reverse();
                    p1Cards.AddRange(p2Pile);
                }
                //p2 Wins
                else if (roundWinner == 2)
                {
                    p2Pile.Reverse();
                    p2Cards.AddRange(p2Pile);
                    p1Pile.Reverse();
                    p2Cards.AddRange(p1Pile);
                }

                p1Pile.Clear();
                p2Pile.Clear();

                Debug.WriteLine("End of round. Winner: " + roundWinner);

            }
        }

        private List<Card> InitialCards()
        {
            List<Card> cards = new List<Card>();

            foreach (Card.SuitType suit in Enum.GetValues(typeof(Card.SuitType)))
            {
                cards.Add(new Card(suit, "ace"));

                for (int i = 2; i <= 10; i++)
                    cards.Add(new Card(suit, i.ToString()));

                cards.Add(new Card(suit, "king"));
                cards.Add(new Card(suit, "queen"));
                cards.Add(new Card(suit, "joker"));
            }

            return cards;
        }

        private void ShuffleCards(List<Card> cards)
        {
            Random rng = new Random();

            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        private void WriteCards(List<Card> cards)
        {
            foreach(Card c in cards)
            {
                Debug.WriteLine("Card: " + c.Suit + "," + c.Rank);
            }
        }
    }
}
