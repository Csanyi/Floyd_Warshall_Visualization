using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Events
{
    public class VertexAddedEventArgs : EventArgs
    {
        public VertexAddedEventArgs(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}
