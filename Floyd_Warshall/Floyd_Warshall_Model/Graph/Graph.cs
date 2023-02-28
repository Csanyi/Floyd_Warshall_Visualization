using System;
using System.Collections.Generic;
using System.Linq;

namespace Floyd_Warshall_Model
{
    public class Graph
    {
        private IDictionary<Vertex, ICollection<Edge>> _adjacenylist;

        private bool _isDirected;
        public bool IsDirected { get { return _isDirected; } }

        public Graph(bool isDirected)
        { 
            _adjacenylist = new Dictionary<Vertex, ICollection<Edge>>();
            _isDirected = isDirected;
        }

        public void AddVertex(Vertex v) 
        {
            Check(() => !_adjacenylist.ContainsKey(v));

            _adjacenylist.Add(v, new List<Edge>());
        }

        public void RemoveVertex(Vertex v)
        {
            Check(() => _adjacenylist.ContainsKey(v));

            foreach(var adjacent in _adjacenylist)
            {
                Edge e = GetEdge(adjacent.Key, v);

                if(e != null)
                {
                    adjacent.Value.Remove(e);
                }
            }

            _adjacenylist.Remove(v);
        }

        public void AddEdge(Vertex from, Vertex to, int weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));
            Check(() => to != from);
            Check(() => GetEdge(from, to) == null);

            _adjacenylist[from].Add(new Edge(from, to, weight));
            if(!_isDirected)
            {
                _adjacenylist[to].Add(new Edge(to, from, weight));
            }
        }

        public void RemoveEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if(e != null)
            {
                _adjacenylist[from].Remove(e);
            }

            if(!_isDirected)
            {
                e = GetEdge(to,from);

                if (e != null)
                {
                    _adjacenylist[to].Remove(e);
                }
            }
        }

        public Edge GetEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));

            return _adjacenylist[from].FirstOrDefault(e => e.To == to);
        }

        public List<Vertex> GetNeighbours(Vertex from)
        {
            return _adjacenylist[from].Select(e => e.To).ToList();
        }

        public List<Vertex> GetVertices()
        {
            return _adjacenylist.Keys.ToList();
        }

        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();

            foreach(var v in _adjacenylist)
            {
                edges.AddRange(v.Value);
            }

            return edges;
        }

        public int GetEdgeCount()
        {
            return _adjacenylist.Sum(x => x.Value.Count());
        }

        public int GetVertexCount()
        {
            return _adjacenylist.Count;
        }

        public Vertex GetVertexById(int id)
        { 
            return _adjacenylist.Keys.FirstOrDefault(v => v.Id == id);
        }

        public int GetWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            return _adjacenylist[from].First(e => e.To == to).Weight;
        }

        public void UpdateWeight(Vertex from, Vertex to, int weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if (e != null)
            {
                e.Weight = weight;
            }

            if (!_isDirected)
            {
                e = GetEdge(to, from);

                if (e != null)
                {
                    e.Weight = weight;
                }
            }
        }

        public void IncrementWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if (e != null)
            {
                ++e.Weight;
            }

            if (!_isDirected)
            {
                e = GetEdge(to, from);

                if (e != null)
                {
                    ++e.Weight;
                }
            }
        }

        public int[,] ToAdjacencyMatrix()
        {
            int size = GetVertexCount();
            List<Vertex> vertices = GetVertices();

            int[,] adjacencyMatrix = new int[size, size];

            for(int i = 0;  i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    Edge e = GetEdge(vertices[i], vertices[j]);

                    adjacencyMatrix[i, j] = (e != null) ? e.Weight : int.MaxValue; 
                }
            }

            return adjacencyMatrix;
        }

        private void Check(Func<bool> cond)
        {
            if(!cond())
            {
                throw new InvalidOperationException();
            }
        }
    }
}
