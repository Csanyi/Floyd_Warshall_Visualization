using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    public class MatrixGridViewModel : ViewModelBase
    {
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private bool _changed;
        public bool Changed
        {
            get { return _changed; }
            set
            {
                _changed = value;
                OnPropertyChanged();
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }

        public ICommand ClickCommand { get; set; } = null!;
    }
}
