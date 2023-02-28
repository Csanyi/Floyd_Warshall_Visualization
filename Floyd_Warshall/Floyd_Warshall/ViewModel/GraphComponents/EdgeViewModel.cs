using Floyd_Warshall_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class EdgeViewModel : EdgeViewModelBase
    {
        public EdgeViewModel(int id, Graph graph) : base(id, graph) { }

        public override double X1 { get { return From.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y1 { get { return From.CanvasY - CanvasY + VertexViewModel.Size / 2; } }

        public override double X2 { get { return To.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y2 { get { return To.CanvasY - CanvasY + VertexViewModel.Size / 2; } }
    }
}
