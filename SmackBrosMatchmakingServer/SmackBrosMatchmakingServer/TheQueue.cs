using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Linq;

namespace SmackBrosMatchmakingServer
{
    public class TheQueue
    {
        int total_size;
        List<StoredPlayer> sortedRatingList = new List<StoredPlayer>();
        List<StoredPlayer> sortedPriorityList = new List<StoredPlayer>();
        public TheQueue()
        {
            this.total_size = 0;
        }

        public bool IsEmpty()
        {
            return (total_size == 0);
        }
        public void Enqueue(StoredPlayer item)
        {
            sortedPriorityList.Add(item);
            sortedRatingList.Add(item);
        }
        public void Match()
        {
            StoredPlayer matchWith = sortedPriorityList.Last();
            sortedPriorityList.Remove(matchWith);
            sortedRatingList.Remove(matchWith);
            SortByMMR();
            var foundMatch = MatchSearchByMMR(sortedRatingList, ref matchWith, 0, sortedRatingList.Count);
            if(foundMatch.internalID != matchWith.internalID)
            {
                sortedRatingList.Remove(foundMatch);
                sortedPriorityList.Remove(foundMatch);
            }
        }
        private void SortByMMR()
        {
            var playerList = sortedRatingList.ToArray();
            sortedRatingList = Sort_Merge_Rating(playerList, 0, playerList.Count()).ToList();
        }
        private StoredPlayer[] Merge_By_Rating(StoredPlayer[] players, int left, int middle, int right)
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
        private StoredPlayer[] Sort_Merge_Rating(StoredPlayer[] players, int left, int right)
        {
            //Mergesort is the "serious" sorting algorithm
            int mid;
            if (right > left)
            {
                //find a midpoint
                mid = (right + left) / 2;
                //recursively call this function for each half of the current array
                players = Sort_Merge_Rating(players, left, mid);
                players = Sort_Merge_Rating(players, mid + 1, right);
                //once that is done, merge it
                players = Merge_By_Rating(players, left, mid, right);
            }
            return players;
        }
        public static StoredPlayer MatchSearchByMMR(List<StoredPlayer> input, ref StoredPlayer match, int min, int max)
        {
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (Math.Abs(match.mmr - input[mid].mmr) < input[mid].mmrTolerance && match.internalID != input[mid].internalID)
                {
                    return input[mid];
                }
                else if (match.mmr < input[mid].mmr)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return match;
        }
        private void SortByPriority()
        {
            var playerList = sortedRatingList.ToArray();
            sortedRatingList = Sort_Merge_Priority(playerList, 0, playerList.Count()).ToList();
        }
        private StoredPlayer[] Merge_By_Priority(StoredPlayer[] players, int left, int middle, int right)
        {
            //find the lengths of the two halves
            int lengthLeft = middle - left;
            int lengthRight = right - middle;
            //set the final element of both arrays to infinity
            StoredPlayer[] leftArray = new StoredPlayer[lengthLeft + 2];
            StoredPlayer temp = new StoredPlayer();
            {
                temp.priority = 10000000;
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
                if (leftArray[iIndex].priority <= rightArray[jIndex].priority)
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
        private StoredPlayer[] Sort_Merge_Priority(StoredPlayer[] players, int left, int right)
        {
            //Mergesort is the "serious" sorting algorithm
            int mid;
            if (right > left)
            {
                //find a midpoint
                mid = (right + left) / 2;
                //recursively call this function for each half of the current array
                players = Sort_Merge_Priority(players, left, mid);
                players = Sort_Merge_Priority(players, mid + 1, right);
                //once that is done, merge it
                players = Merge_By_Priority(players, left, mid, right);
            }
            return players;
        }
    }
}
