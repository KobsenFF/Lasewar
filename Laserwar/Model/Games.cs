using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laserwar.Model
{
    class Games
    {
        public Games(string Name, string Id, string Date, int playesrCount)
        {
            this.Name = Name;
            this.Id = Id;
            this.Date = Date;
            this.playesrCount = playesrCount;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Date { get; set; }
        public int playesrCount { get; set; }
    }
}
