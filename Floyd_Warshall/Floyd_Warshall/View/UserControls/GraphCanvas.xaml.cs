using Floyd_Warshall.ViewModel.GraphComponents;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Floyd_Warshall.View.UserControls
{
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

                CanvasClick?.Execute(null);
            }
        }

        private static readonly Regex _allowedRegex = new Regex("[0-9\\-]");
        private static bool IsTextAllowed(string text)
        {
            return _allowedRegex.IsMatch(text);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            Vertex v = e.Data.GetData(DataFormats.Serializable) as Vertex;

            Point p = e.GetPosition(sender as Canvas);

            v.X = p.X - VertexViewModel.Size / 2;
            v.Y = p.Y - VertexViewModel.Size / 2;
        }
    }
}
