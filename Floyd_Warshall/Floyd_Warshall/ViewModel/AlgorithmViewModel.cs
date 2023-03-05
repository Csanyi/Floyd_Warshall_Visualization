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

        public int? K { get { return _graphModel.K; } }

        private bool _isStopped = false;
        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                OnPropertyChanged();
            }
        }

        private bool _isStarted = false;
        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                _isStarted = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning => _graphModel.IsRunning;

        public WpfObservableRangeCollection<int> D { get; set; }
        public WpfObservableRangeCollection<int> Pi { get; set; }

        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand RestartCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }

        public AlgorithmViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            StartCommand = new DelegateCommand(param => Start(), param => { return _graphModel.Graph.GetVertexCount() > 1; });
            StopCommand = new DelegateCommand(param => Stop());

            PauseCommand = new DelegateCommand(param => Pause(), param => { return IsRunning; });
            RestartCommand = new DelegateCommand(param => Restart(), param => { return IsRunning; });

            StepCommand = new DelegateCommand(param => Step(), param => { return IsRunning && IsStopped; });

            _timer = new DispatcherTimer();
            TimerInterval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);

            _graphModel.AlgorithmStarted += new EventHandler<Tuple<int[,], int[,]>>(Model_AlgorithmStarted);
            _graphModel.AlgorithmStepped += new EventHandler<Tuple<int[,], int[,]>>(Model_AlgorithmStepped);
            _graphModel.AlgorithmEnded += new EventHandler(Model_AlgorithmEnded);

            D = new WpfObservableRangeCollection<int>();
            Pi = new WpfObservableRangeCollection<int>();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        private void Start()
        {
            Size = _graphModel.Graph.GetVertexCount();
            _graphModel.StartAlgorithm();
            _timer.Start();

            IsStarted = true;
        }

        private void Stop()
        {
            _timer.Stop();
            _graphModel.StopAlgorithm();

            IsStarted = false;
            IsStopped = false;

            D.Clear();
            Pi.Clear();
        }

        private void Pause()
        {
            _timer.Stop();
            IsStopped = true;
        }

        private void Restart()
        {
            _timer.Start();
            IsStopped = false;
        }

        private void Step()
        {
            _graphModel.StepAlgorithm();
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

            RestartCommand.RaiseCanExecuteChanged();
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
