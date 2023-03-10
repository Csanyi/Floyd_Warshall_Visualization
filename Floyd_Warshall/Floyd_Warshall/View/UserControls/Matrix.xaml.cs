using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Floyd_Warshall.View.UserControls
{
    /// <summary>
    /// Interaction logic for Matrix.xaml
    /// </summary>
    public partial class Matrix : UserControl
    {
        public Matrix()
        {
            InitializeComponent();
        }

        public ICollection<int> MatrixData
        {
            get { return (ICollection<int>)GetValue(MatrixDataProperty); }
            set { SetValue(MatrixDataProperty, value); }
        }

        public static readonly DependencyProperty MatrixDataProperty =
            DependencyProperty.Register("MatrixData", typeof(ICollection<int>), typeof(Matrix), new PropertyMetadata(null));



        public bool UseConverter
        {
            get { return (bool)GetValue(UseConverterProperty); }
            set { SetValue(UseConverterProperty, value); }
        }

        public static readonly DependencyProperty UseConverterProperty =
            DependencyProperty.Register("UseConverter", typeof(bool), typeof(Matrix), new PropertyMetadata(false));
    }
}
