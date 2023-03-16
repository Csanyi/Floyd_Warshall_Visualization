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
            Data = new AlgorithmData(d, pi);
        }

        public AlgorithmData Data { get; private set; }
    }
}
