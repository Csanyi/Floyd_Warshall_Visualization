using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Floyd_Warshall.View.UserControls
{
	/// <summary>
	/// Interaction logic for GraphCanvas.xaml
	/// </summary>
	public partial class GraphCanvas : UserControl
    {
        public GraphCanvas()
        {
            InitializeComponent();
        }

        public ICommand CanvasClick
        {
            get { return (ICommand)GetValue(CanvasClickProperty); }
            set { SetValue(CanvasClickProperty, value); }
        }

        public static readonly DependencyProperty CanvasClickProperty =
            DependencyProperty.Register("CanvasClick", typeof(ICommand), typeof(GraphCanvas), new PropertyMetadata(null));



        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(GraphCanvas), new PropertyMetadata(0.0));


        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(GraphCanvas), new PropertyMetadata(0.0));



        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Canvas)
            {
                Point pos = e.GetPosition(sender as Canvas);

                X = pos.X;
                Y = pos.Y;

                if (CanvasClick != null && CanvasClick.CanExecute(null))
                {
                    CanvasClick.Execute(null);
                }
            }
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.Serializable) is Vertex v)
            {
                Point p = e.GetPosition(sender as Canvas);

                v.X = p.X;
                v.Y = p.Y;
            }
        }
    }
}
