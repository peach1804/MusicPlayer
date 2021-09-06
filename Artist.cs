using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMusicPlayer
{
    public class Artist : IComparable<Artist>
    {
        public string name;
        public List<Song> songs = new List<Song>();

        public int CompareTo(Artist other)
        {
            if (other == null) return 1;

            return this.name.ToLower().CompareTo(other.name.ToLower());
        }
    }
}
