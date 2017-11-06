using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Laserwar.Model
{
    public class Sounds
    {
        public Sounds(string Name, string Url, string Size)
        {
            this.Name = Name;
            this.Url = Url;
            this.Size = Size;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
    }

    


}


