using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;

namespace SmackBrosMatchmakingServer
{
    public class TheQueue
    {
        int total_size;
        List<StoredPlayer> sortedRatingList = new List<StoredPlayer>();
        SortedDictionary<int, Queue> storage;
        public TheQueue()
        {
            this.storage = new SortedDictionary<int, Queue>();
            this.total_size = 0;
        }

        public bool IsEmpty()
        {
            return (total_size == 0);
        }

        public object Dequeue()
        {
            if (IsEmpty())
            {
                throw new Exception("No Players in the Queue");
            }
            else
                foreach (Queue q in storage.Values)
                {
                    // we use a sorted dictionary
                    if (q.Count > 0)
                    {
                        total_size--;
                        return q.Dequeue();
                    }
                }

            throw new Exception("Error pulling from Queue");
        }

        // same as above, except for peek.

        public object Peek()
        {
            if (IsEmpty())
                throw new Exception("No Players in the Queue");
            else
                foreach (Queue q in storage.Values)
                {
                    if (q.Count > 0)
                        return q.Peek();
                }
            throw new Exception("Error peeking into Queue");
        }

        public object Dequeue(int prio)
        {
            total_size--;
            return storage[prio].Dequeue();
        }
        public void Enqueue(object item, int prio)
        {
            if (!storage.ContainsKey(prio))
            {
                storage.Add(prio, new Queue());
            }
            storage[prio].Enqueue(item);
            total_size++;
        }
        private List<StoredPlayer> SortByMMR(List<StoredPlayer> toSort)
        {
            StoredPlayer[] playerList = toSort.ToArray();
            playerList = SortMerge(playerList, 0, playerList.Count());
            return playerList.ToList();
        }
        private StoredPlayer[] Merge(StoredPlayer[] players, int left, int middle, int right)
        {
            //find the lengths of the two halves
            int lengthLeft = middle - left;
            int lengthRight = right - middle;
            //set the final element of both arrays to infinity
            StoredPlayer[] leftArray = new StoredPlayer[lengthLeft + 2];
            StoredPlayer temp = new StoredPlayer();
            {
                temp.mmr = 10000;
            }
            leftArray[lengthLeft + 1] = temp;
            StoredPlayer[] rightArray = new StoredPlayer[lengthRight + 1];
            rightArray[lengthRight] = temp;
            //create the two arrays that will be used to 
            for (int i = 0; i <= lengthLeft; i++)
            {
                leftArray[i] = players[left + i];
            }
            for (int j = 0; j < lengthRight; j++)
            {
                rightArray[j] = players[middle + j + 1];
            }
            int iIndex = 0;
            int jIndex = 0;
            //take the lower element of the two arrays and add it to the new sorted array
            for (int k = left; k <= right; k++)
            {
                if (leftArray[iIndex].mmr <= rightArray[jIndex].mmr)
                {
                    players[k] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    players[k] = rightArray[jIndex];
                    jIndex++;
                }
            }
            return players;
        }
        public StoredPlayer[] SortMerge(StoredPlayer[] players, int left, int right)
        {
            //Mergesort is the "serious" sorting algorithm
            int mid;
            if (right > left)
            {
                //find a midpoint
                mid = (right + left) / 2;
                //recursively call this function for each half of the current array
                players = SortMerge(players, left, mid);
                players = SortMerge(players, mid + 1, right);
                //once that is done, merge it
                players = Merge(players, left, mid, right);
            }
            return players;
        }
        public static object BinarySearch(List<StoredPlayer> input, int key, int min, int max)
        {
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (Math.Abs(key - input[mid].mmr) < input[mid].mmrTolerance)
                {
                    return mid;
                }
                else if (key < input[mid].mmr)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return "Item Not Found";
        }
    }
}
