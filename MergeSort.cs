using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMusicPlayer
{
    public class MergeSort<T> where T : IComparable<T>
    {
        public static void mergeSort(List<T> list)
        {
            mergeSort(list, list.Count);
        }

        public static void mergeSort(List<T> list, int n)
        {
            if (n > 1)
            {

                int mid = n / 2;
                List<T> left = new List<T>(mid);
                List<T> right = new List<T>(n - mid);

                for (int i = 0; i < mid; i++)
                {
                    left.Add(list[i]);
                }

                for (int i = mid; i < n; i++)
                {
                    right.Add(list[i]);
                }

                mergeSort(left, mid);
                mergeSort(right, n - mid);

                merge(list, left, right);
            }
        }

        public static void merge(List<T> list, List<T> left, List<T> right)
        {
            int m = left.Count;
            int n = right.Count;

            int i = 0, j = 0, k = 0;

            // increment k and the index that is copied
            while (i < m && j < n)
            {
                if (left[i].CompareTo(right[j]) < 0)
                {
                    list[k] = left[i];
                    k++;
                    i++;

                }
                else
                {
                    list[k] = right[j];
                    k++;
                    j++;
                }
            }

            // copy remaining elements of left<> to list<>
            for (; i < m; i++)
            {
                list[k] = left[i];
                k++;
            }

            // copy remaining elements of right<> to list<>
            for (; j < n; j++)
            {
                list[k] = right[j];
                k++;
            }
        }
    }
}
