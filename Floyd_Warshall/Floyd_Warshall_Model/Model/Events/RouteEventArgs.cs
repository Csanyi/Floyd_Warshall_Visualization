namespace Floyd_Warshall_Model.Model.Events
{
    /// <summary>
    /// Type of the route event argument
    /// </summary>
    public class RouteEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the route event argument
        /// </summary>
        /// <param name="route">List of the route vertex ids</param>
        public RouteEventArgs(List<int> route)
        {
            Route = route;
        }

        /// <summary>
        /// Gets the route
        /// </summary>
        public List<int> Route { get; }
    }
}
