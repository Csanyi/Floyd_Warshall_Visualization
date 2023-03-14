using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Events
{
    public class RouteEventArgs : EventArgs
    {
        public RouteEventArgs(List<int> route)
        { 
            Route = route;
        }

        public List<int> Route { get; private set; }
    }
}
