﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Floyd_Warshall.View.UserControls
{
	/// <summary>
	/// Interaction logic for Vertex.xaml
	/// </summary>
	public partial class Vertex : UserControl
    {
        public Vertex()
        {
            InitializeComponent();
        }

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(Vertex), new PropertyMetadata(0.0));


        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(Vertex), new PropertyMetadata(0.0));



        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, new DataObject(DataFormats.Serializable, this), DragDropEffects.Move);
            }
        }
    }
}
