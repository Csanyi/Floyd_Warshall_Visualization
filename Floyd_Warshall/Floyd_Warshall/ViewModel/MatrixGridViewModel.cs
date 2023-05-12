using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
	/// <summary>
	/// Type of the matrix grid viewmodel
	/// </summary>
	public class MatrixGridViewModel : ViewModelBase
    {
        private int _value;
        private bool _changed;

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the changed field
        /// </summary>
        public bool Changed
        {
            get { return _changed; }
            set
            {
                _changed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the horizontal coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the vertical coordinate
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the click command
        /// </summary>
        public ICommand ClickCommand { get; set; } = null!;
    }
}
