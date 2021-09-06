using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMusicPlayer
{
    class BinarySearch<T> where T : IComparable<T>
    {
        public static T binarySearch(List<T> list, T search)
        {
            int lowBound = 0;
            int highBound = list.Count() - 1;

            while (lowBound <= highBound)
            {
                int mid = (lowBound + highBound) / 2;

                int compare = list[mid].CompareTo(search);

                if (compare == 0)
                {
                    search = list[mid];
                    return search;
                }
                else if (compare > 0)
                {
                    highBound = mid - 1;
                }
                else
                {
                    lowBound = mid + 1;
                }
            }

            return default(T);
        }
    }
}
