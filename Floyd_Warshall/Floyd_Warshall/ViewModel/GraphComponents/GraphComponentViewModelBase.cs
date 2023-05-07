using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    /// <summary>
    /// Type of the graph compontent viewmodel base class
    /// </summary>
    public abstract class GraphComponentViewModelBase : ViewModelBase, IEquatable<GraphComponentViewModelBase>
    {
        private readonly int _id; // component id
        private bool _isSelected; // component selection

        /// <summary>
        /// Constructor of the graph compontent viewmodel base class
        /// </summary>
        /// <param name="id">The component id</param>
        protected GraphComponentViewModelBase(int id)
        {
            _id = id;
        }

        /// <summary>
        /// Gets the id
        /// </summary>
        public int Id { get { return _id; } }

        /// <summary>
        /// Gets or sets the compontent selection
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the component's x coord on the canvas
        /// </summary>
        public virtual double CanvasX { get; set; }

        /// <summary>
        /// Gets or sets the component's y coord on the canvas
        /// </summary>
        public virtual double CanvasY { get; set; }

        /// <summary>
        /// Gets or sets the left click command
        /// </summary>
        public ICommand LeftClickCommand { get; set; } = null!;

        /// <summary>
        /// Gets or sets the right click command
        /// </summary>
        public ICommand RightClickCommand { get; set; } = null!;

        /// <summary>
        /// Gets the type of the object
        /// </summary>
        public Type Type { get { return this.GetType(); } }

        public bool Equals(GraphComponentViewModelBase? other)
        {
            if ((other == null) || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }
            else
            {
                return this._id == other._id;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                GraphComponentViewModelBase g = (GraphComponentViewModelBase)obj;
                return _id == g._id;
            }
        }
    }
}
