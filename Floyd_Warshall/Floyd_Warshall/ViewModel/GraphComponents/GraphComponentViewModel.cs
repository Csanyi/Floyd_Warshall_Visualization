using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    public abstract class GraphComponentViewModel : ViewModelBase
    {
        public virtual int Id { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public virtual double CanvasX { get; set; }
        public virtual double CanvasY { get; set; }

        public ICommand LeftClickCommand { get; set; }
        public ICommand RightClickCommand { get; set; }

        public Type Type => this.GetType();

        public Type BaseType => this.GetType().BaseType;

    }
}
