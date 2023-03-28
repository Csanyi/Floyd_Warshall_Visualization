using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Events
{
    public class EdgeAddedEventArgs : EventArgs
    {
        public EdgeAddedEventArgs(int id, int from, int to, short weight)
        {
            Id = id;
            From = from;
            To = to;
            Weight = weight;
        }

        public int Id { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }
        public short Weight { get; private set; }
    }
}
