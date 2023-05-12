using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
	/// <summary>
	/// Type of the graph file data access
	/// </summary>
	public class GraphFileDataAccess : IGraphDataAccess
    {
        /// <summary>
        /// Loads graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The loaded graph data</returns>
        /// <exception cref="GraphDataException"></exception>
        public async Task<GraphData> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader= new StreamReader(path))
                {
                    string? line = await reader.ReadLineAsync();

                    bool isDirected = Convert.ToBoolean(line);

                    GraphBase graph;
                    if (isDirected)
                    {
                        graph = new DirectedGraph();
                    } else
                    {
                        graph = new UndirectedGraph();
                    }

                    List<VertexData> locations = new List<VertexData>();

                    line = await reader.ReadLineAsync();

                    if(line != null && line != "")
                    {
                        string[] values = line.Split(' ');

                        foreach (string value in values)
                        {
                            string[] v = value.Split(';');
                            Vertex vertex = new Vertex(int.Parse(v[0]));
                            graph.AddVertex(vertex);
                            locations.Add(new VertexData(vertex.Id, double.Parse(v[1]), double.Parse(v[2])));
                        }

                        line = await reader.ReadLineAsync();
                        if (line != null && line != "")
                        {
                            values = line.Split(' ');

                            foreach (string value in values)
                            {
                                string[] e = value.Split(';');
                                Vertex? from = graph.GetVertexById(int.Parse(e[0]));
                                Vertex? to = graph.GetVertexById(int.Parse(e[1]));
                                if (from != null && to != null && graph.GetEdge(from, to) == null)
                                {
                                    graph.AddEdge(from, to, short.Parse(e[2]));
                                }
                            }
                        }
                    }

                    return new GraphData(graph, locations);
                }
            } catch
            {
                throw new GraphDataException();
            }
        }

        /// <summary>
        /// Saves graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="graph">Graph to save</param>
        /// <param name="locations">Locations of the verteces</param>
        /// <exception cref="GraphDataException"></exception>
        public async Task SaveAsync(string path, GraphBase graph, IEnumerable<VertexData> locations)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(graph.IsDirected.ToString());

                    string delimiter = "";

                    foreach(var v in locations)
                    {
                        await writer.WriteAsync(delimiter + v.Id + ";" + v.X + ";" + v.Y);
                        delimiter = " ";
                    }
                    await writer.WriteLineAsync();

                    delimiter = "";

                    foreach(Edge e in graph.Edges)
                    {
                        await writer.WriteAsync(delimiter + e.From.Id + ";" + e.To.Id + ";" + e.Weight);
                        delimiter = " ";
                    }
                }
            } catch
            {
                throw new GraphDataException();
            }
        }
    }
}
