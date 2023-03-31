namespace Floyd_Warshall_Model.Model.Events
{
    public class RouteEventArgs : EventArgs
    {
        public RouteEventArgs(List<int> route)
        {
            Route = route;
        }

        public List<int> Route { get; }
    }
}
