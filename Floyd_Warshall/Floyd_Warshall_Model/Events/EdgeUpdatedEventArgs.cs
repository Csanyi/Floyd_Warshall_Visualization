using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Events
{
    public class EdgeUpdatedEventArgs : EventArgs
    {
        public EdgeUpdatedEventArgs(int from, int to)
        {
            From = from;
            To = to;
        }

        public int From { get; private set; }
        public int To { get; private set; }
    }
}
