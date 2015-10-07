using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmackBrosMatchmakingServer
{
    public struct StoredPlayer
    {
        public string name;
        public string playerIP;
        public int mmrTolerance;
        public int mmr;
        public bool searchedThisIteration;
        public int TimeAddedtoQueue;
    }
}
