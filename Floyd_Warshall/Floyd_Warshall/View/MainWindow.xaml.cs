using System.Windows;
using System.Windows.Media;

namespace Floyd_Warshall.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            ScaleTransform scale = new ScaleTransform(graphCanvas.canvas.LayoutTransform.Value.M11 * xChange, graphCanvas.canvas.LayoutTransform.Value.M22 * yChange);
            graphCanvas.canvas.LayoutTransform = scale;
            graphCanvas.canvas.UpdateLayout();
        }
	}
}
