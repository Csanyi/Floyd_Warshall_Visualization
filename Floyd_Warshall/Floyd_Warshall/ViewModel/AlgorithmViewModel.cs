using Floyd_Warshall.ViewModel.Commands;
using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Floyd_Warshall.ViewModel
{
    public class AlgorithmViewModel : ViewModelBase
    {
        private readonly GraphModel _graphModel;
        private readonly DispatcherTimer _timer;

        public int TimerInterval
        {
            get { return _timer.Interval.Seconds * 1000 + _timer.Interval.Milliseconds; }
            set
            {
                _timer.Interval = TimeSpan.FromMilliseconds(value);
                OnPropertyChanged();
            }
        }

        private int _size;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        public int? K => _graphModel.K;

        public bool IsInitialized => _graphModel.IsAlgorthmInitialized;

        public bool IsRunning => _graphModel.IsAlgorithmRunning;

        private bool _isStopped;
        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                OnPropertyChanged();
            }
        }

        public WpfObservableRangeCollection<int> D { get; set; }
        public WpfObservableRangeCollection<int> Pi { get; set; }
        public ObservableCollection<int> VertexIds { get; set; }

        public DelegateCommand InitCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }

        public AlgorithmViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            InitCommand = new DelegateCommand(param => Init(), param => { return _graphModel.GetVertexCount() > 1; });
            CancelCommand = new DelegateCommand(param => Cancel());

            PauseCommand = new DelegateCommand(param => Pause(), param => { return IsRunning; });
            StartCommand = new DelegateCommand(param => Start(), param => { return IsRunning; });

            StepCommand = new DelegateCommand(param => Step(), param => { return IsRunning && IsStopped; });

            _timer = new DispatcherTimer();
            TimerInterval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);

            _graphModel.VertexAdded += new EventHandler(Model_VertexCntChanged);
            _graphModel.VertexRemoved += new EventHandler(Model_VertexCntChanged);
            _graphModel.AlgorithmStarted += new EventHandler<AlgorithmEventArgs>(Model_AlgorithmStarted);
            _graphModel.AlgorithmStepped += new EventHandler<AlgorithmEventArgs>(Model_AlgorithmStepped);
            _graphModel.AlgorithmEnded += new EventHandler(Model_AlgorithmEnded);

            D = new WpfObservableRangeCollection<int>();
            Pi = new WpfObservableRangeCollection<int>();
            VertexIds = new ObservableCollection<int>();

            IsStopped = true;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        private void Init()
        {
            _graphModel.StartAlgorithm();

            foreach(int id in _graphModel.GetVertexIds())
            {
                VertexIds.Add(id);
            }

            Size = VertexIds.Count;

            OnPropertyChanged(nameof(IsInitialized));
        }

        private void Cancel()
        {
            _timer.Stop();
            _graphModel.StopAlgorithm();

            OnPropertyChanged(nameof(IsInitialized));
            IsStopped = true;

            D.Clear();
            Pi.Clear();
            VertexIds.Clear();
        }

        private void Pause()
        {
            _timer.Stop();
            IsStopped = true;
        }

        private void Start()
        {
            _timer.Start();
            IsStopped = false;
        }

        private void Step()
        {
            _graphModel.StepAlgorithm();
        }

        private void Model_VertexCntChanged(object? sender, EventArgs e)
        {
            InitCommand.RaiseCanExecuteChanged();
        }

        private async void Model_AlgorithmStarted(object? sender, AlgorithmEventArgs e)
        {
            OnPropertyChanged(nameof(K));

            int[] res = await Task.Run(() =>
            {
                int[] tmp = new int[e.D.GetLength(0) * e.D.GetLength(1)];
                Buffer.BlockCopy(e.D, 0, tmp, 0, tmp.Length * sizeof(int));

                return tmp;
            });

            D.Clear();
            D.AddRange(res);

            res = await Task.Run(() =>
            {
                int[] tmp = new int[e.Pi.GetLength(0) * e.Pi.GetLength(1)];
                Buffer.BlockCopy(e.Pi, 0, tmp, 0, tmp.Length * sizeof(int));

                return tmp;
            });

            Pi.Clear();
            Pi.AddRange(res);
        }
        
        private void Model_AlgorithmEnded(object? sender, EventArgs e)
        {
            _timer.Stop();

            StartCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StepCommand.RaiseCanExecuteChanged();
        }

        private async void Model_AlgorithmStepped(object? sender, AlgorithmEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(IsRunning));

            int[] res = await Task.Run(() =>
            {
                int[] tmp = new int[e.D.GetLength(0) * e.D.GetLength(1)];
                Buffer.BlockCopy(e.D, 0, tmp, 0, tmp.Length * sizeof(int));

                return tmp;
            });

            D.Clear();
            D.AddRange(res);

            res = await Task.Run(() =>
            {
                int[] tmp = new int[e.Pi.GetLength(0) * e.Pi.GetLength(1)];
                Buffer.BlockCopy(e.Pi, 0, tmp, 0, tmp.Length * sizeof(int));

                return tmp;
            });

            Pi.Clear();
            Pi.AddRange(res);
        }
    }
}
