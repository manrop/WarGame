using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using SqliteManager;

namespace WarGameApi
{
    public class Game
    {
        private string player1, player2;
        private string conSqlite = "Data Source = wargame.db";

        public GameResult StartGame(string p1, string p2)
        {
            player1 = p1;
            player2 = p2;

            GameResult gameResult;

            if (string.IsNullOrWhiteSpace(p1))
                throw new ArgumentException("Player1 name Invalid.");

            if (string.IsNullOrWhiteSpace(p2))
                throw new ArgumentException("Player2 name Invalid.");

            IniDB();

            //Get initial Deck of Cards
            List<Card> cards = InitialCards();

            //Play The Game
            gameResult = PlayGame(cards);

            Console.WriteLine($"The winner is {gameResult.Winner}, after {gameResult.Rounds} rounds.");

            int GameId = SaveGameResult(gameResult);
            if (GameId > 0)
                gameResult.GameId = GameId;
            else
                throw new Exception("Error ocurred saving the Game.");

            return gameResult;

        }

        public List<PlayerStats> PlayerWins()
        {
            List<PlayerStats> playerStats = new List<PlayerStats>();
            int cantWins;

            string qrySelect = @"SELECT Winner, Count(GameId) Cant
                                FROM GameResult
                                GROUP BY Winner
                                ORDER BY Cant DESC, Winner ASC";

            SQLiteManager sqlman = new SQLiteManager(conSqlite);

            var data = sqlman.GetDataTable(qrySelect);
            foreach(System.Data.DataRow row in data.Rows)
            {
                if (!int.TryParse(row["Cant"].ToString(), out cantWins))
                    cantWins = 0;

                playerStats.Add(new PlayerStats()
                {
                    Player = row["Winner"].ToString()
                    ,Wins = cantWins
                });
            }

            return playerStats;
        }

        private GameResult PlayGame(List<Card> cards)
        {
            GameResult result = new GameResult();

            List<Card> p1Cards = new List<Card>();
            List<Card> p2Cards = new List<Card>();

            List<Card> p1Pile = new List<Card>();
            List<Card> p2Pile = new List<Card>();

            //Shuffle card deck
            ShuffleCards(cards);

            //Deal Cards to each Player
            while (cards.Count > 0)
            {
                p1Cards.Add(cards.First());
                cards.Remove(cards.First());

                p2Cards.Add(cards.First());
                cards.Remove(cards.First());
            }

            string movDesc;
            int roundNum = 0;

            while (p1Cards.Count > 0 && p2Cards.Count > 0)
            {
                roundNum++;
                //Console.WriteLine("Round " + roundNum + " -->");

                p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());
                p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Cards.First());

                int roundWinner = p1Pile.Last().Compare(p2Pile.Last());

                if (roundWinner > 0)
                    movDesc = $"{player1} plays {p1Pile.Last()}. {player2} plays {p2Pile.Last()}. {(roundWinner == 1 ? player1 : player2)} wins!";
                else
                    movDesc = $"{player1} plays {p1Pile.Last()}. {player2} plays {p2Pile.Last()}. It's WAR!";
                result.Movements.Add(new GameMovement() { Round = roundNum, Description = movDesc });

                //Same Rank (WAR)
                while (roundWinner == 0)
                {
                    if(p1Cards.Count < 2) // Not enought cards to War
                    {
                        p1Pile.AddRange(p1Cards);
                        p1Cards.Clear();
                        roundWinner = 2;

                        movDesc = $"{player1} doesn't have enough cards to play War. {player2} wins!";
                        result.Movements.Add(new GameMovement() { Round = roundNum, Description = movDesc });
                        break;
                    }
                    if(p2Cards.Count < 2) // Not enought cards to War
                    {
                        p2Pile.AddRange(p2Cards);
                        p2Cards.Clear();
                        roundWinner = 1;
                        movDesc = $"{player2} doesn't have enough cards to play War. {player1} wins!";
                        result.Movements.Add(new GameMovement() { Round = roundNum, Description = movDesc });
                        break;
                    }
                        
                    p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());
                    p1Pile.Add(p1Cards.First()); p1Cards.Remove(p1Cards.First());

                    p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Cards.First());
                    p2Pile.Add(p2Cards.First()); p2Cards.Remove(p2Cards.First());

                    roundWinner = p1Pile.Last().Compare(p2Pile.Last());
                    if (roundWinner > 0)
                        movDesc = $"{player1} plays {p1Pile.Last()}. {player2} plays {p2Pile.Last()}. '{(roundWinner == 1 ? player1 : player2)}' wins the war! Takes {p1Pile.Count + p2Pile.Count} cards.";
                    else
                        movDesc = $"{player1} plays {p1Pile.Last()}. {player2} plays {p2Pile.Last()}. War continues!";
                    result.Movements.Add(new GameMovement() { Round = roundNum, Description = movDesc });
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

            }

            if (p1Cards.Count > 0)
                result.Winner = player1;
            else
                result.Winner = player2;

            result.GameId = 0;
            result.Rounds = roundNum;

            return result;
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

        private int SaveGameResult(GameResult game)
        {
            int gameId = 0;
            Dictionary<string, string> datos = new Dictionary<string, string>();

            datos.Add("Winner", game.Winner);
            datos.Add("Rounds", game.Rounds.ToString());

            SQLiteManager sqlman = new SQLiteManager(conSqlite);
            if(sqlman.Insert("GameResult", datos))
            {
                var res = sqlman.ExecuteScalar("SELECT GameId FROM GameResult ORDER BY GameId desc LIMIT 1");
                gameId = int.Parse(res);
            }

            return gameId;
        }
        

        private void IniDB()
        {
            SQLiteManager sqlman = new SQLiteManager(conSqlite);

            string qryCreate = @"CREATE TABLE IF NOT EXISTS ""GameResult"" (
                                ""GameId""    INTEGER,
	                            ""Winner""    TEXT,
	                            ""Rounds""    INTEGER,
	                            PRIMARY KEY(""GameId"" AUTOINCREMENT))";

            sqlman.ExecuteNonQuery(qryCreate);

            qryCreate = @"CREATE TABLE IF NOT EXISTS ""GameMovement"" (
                                ""GameId""    INTEGER,
	                            ""Round"" INTEGER,
	                            ""Description""   TEXT,
	                            PRIMARY KEY(""GameId"",""Round"")
                            )";
            sqlman.ExecuteNonQuery(qryCreate);
        }

        private void ListCards(List<Card> cards)
        {
            foreach(Card c in cards)
            {
                Console.WriteLine("Card: " + c.Suit + "," + c.Rank);
            }
        }
    }
}
