using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMusicPlayer
{
    public class Song : IComparable<Song>
    {
        public string name { get; set; }
        public string fileName { get; set; }
        public Artist artist { get; set; }

        public int CompareTo(Song other)
        {
            if (other == null) return 1;

            return this.name.ToLower().CompareTo(other.name.ToLower());
        }
    }
}
