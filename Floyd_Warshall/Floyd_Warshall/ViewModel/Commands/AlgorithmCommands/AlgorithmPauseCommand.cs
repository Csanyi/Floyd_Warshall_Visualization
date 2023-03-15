﻿using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    public class AlgorithmPauseCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;

        public AlgorithmPauseCommand(AlgorithmViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _viewModel.Timer.Stop();
            _viewModel.IsStopped = true;
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsRunning && !_viewModel.IsStopped && _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.IsRunning) || e.PropertyName == nameof(AlgorithmViewModel.IsStopped)
                 || e.PropertyName == nameof(AlgorithmViewModel.IsInitialized))
            {
                OnCanExecuteChanged();
            }
        }
    }
}