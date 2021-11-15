using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarGameApi
{
    public class GameResult
    {
        public int GameId { get; set; }
        public string Winner { get; set; }
        public int Rounds { get; set; }
        public List<GameMovement> Movements {get; set;}

        public GameResult()
        {
            GameId = 0;
            Winner = "";
            Rounds = 0;
            Movements = new List<GameMovement>();
        }
    }

}
