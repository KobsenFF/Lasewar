using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laserwar.Model
{
    class Players
    {
        public Players(int TeamId, int Id, string PlayerName, string GameId, string TeamName, int Rating, string Accurancy, int Shots)
        {
            this.Id = Id;
            this.TeamId = TeamId;
            this.PlayerName = PlayerName;
            this.GameId = GameId;
            this.TeamName = TeamName;
            this.Rating = Rating;
            this.Accurancy = Accurancy+"%";
            this.Shots = Shots;
        }

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerName { get; set; }
        public string GameId { get; set; }
        public string TeamName { get; set; }
        public int Rating { get; set; }
        public string Accurancy { get; set; }
        public int Shots { get; set; }
    }
}
