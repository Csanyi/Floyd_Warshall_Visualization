using Floyd_Warshall_Model;
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

        private int _timerInterval;
        public int TimerInterval
        {
            get { return _timerInterval; }
            set
            {
                _timerInterval = value;
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
            _graphModel.AlgorithmStarted += new EventHandler<Tuple<int[,], int[,]>>(Model_AlgorithmStarted);
            _graphModel.AlgorithmStepped += new EventHandler<Tuple<int[,], int[,]>>(Model_AlgorithmStepped);
            _graphModel.AlgorithmEnded += new EventHandler(Model_AlgorithmEnded);

            D = new WpfObservableRangeCollection<int>();
            Pi = new WpfObservableRangeCollection<int>();

            IsStopped = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        private void Init()
        {
            Size = _graphModel.GetVertexCount();
            _graphModel.StartAlgorithm();

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

        private void Model_VertexCntChanged(object sender, EventArgs e)
        {
            InitCommand.RaiseCanExecuteChanged();
        }

        private async void Model_AlgorithmStarted(object sender, Tuple<int[,], int[,]> e)
        {
            OnPropertyChanged(nameof(K));

            var res = await Task.Run(() =>
            {
                List<int> res = new List<int>();

                for (int i = 0; i < e.Item1.GetLength(0); ++i)
                {
                    for (int j = 0; j < e.Item2.GetLength(1); ++j)
                    {
                        res.Add(e.Item1[i, j]);
                    }
                }

                return res;
            });

            D.Clear();
            D.AddRange(res);

            res = await Task.Run(() =>
            {
                List<int> res = new List<int>();

                for (int i = 0; i < e.Item1.GetLength(0); ++i)
                {
                    for (int j = 0; j < e.Item2.GetLength(1); ++j)
                    {
                        res.Add(e.Item2[i, j]);
                    }
                }

                return res;
            });

            Pi.Clear();
            Pi.AddRange(res);
        }
        
        private void Model_AlgorithmEnded(object sender, EventArgs e)
        {
            _timer.Stop();

            StartCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StepCommand.RaiseCanExecuteChanged();
        }

        private async void Model_AlgorithmStepped(object sender, Tuple<int[,], int[,]> e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(IsRunning));

            var res = await Task.Run(() =>
            {
                List<int> res = new List<int>();

                for (int i = 0; i < e.Item1.GetLength(0); ++i)
                {
                    for (int j = 0; j < e.Item2.GetLength(1); ++j)
                    {
                        res.Add(e.Item1[i, j]);
                    }
                }

                return res;
            });

            D.Clear();
            D.AddRange(res);

            res = await Task.Run(() =>
            {
                List<int> res = new List<int>();

                for (int i = 0; i < e.Item1.GetLength(0); ++i)
                {
                    for (int j = 0; j < e.Item2.GetLength(1); ++j)
                    {
                        res.Add(e.Item2[i, j]);
                    }
                }

                return res;
            });

            Pi.Clear();
            Pi.AddRange(res);
        }
    }
}
