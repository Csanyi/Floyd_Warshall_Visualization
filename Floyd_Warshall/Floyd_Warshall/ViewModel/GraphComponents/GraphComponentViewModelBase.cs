using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    public abstract class GraphComponentViewModelBase : ViewModelBase
    {
        protected GraphComponentViewModelBase(int id)
        {
            _id = id;
        }

        private readonly int _id;
        public int Id { get { return _id; } }

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

        public ICommand LeftClickCommand { get; set; } = null!;
        public ICommand RightClickCommand { get; set; } = null!;

        public Type Type { get { return this.GetType(); } }
    }
}
