using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Events
{
    public class AlgorithmEventArgs : EventArgs
    {
        public AlgorithmEventArgs(int[,] d, int[,] pi)
        {
            D = d;
            Pi = pi;
        }

        public int[,] D { get; private set; }
        public int[,] Pi { get; private set; }
    }
}
