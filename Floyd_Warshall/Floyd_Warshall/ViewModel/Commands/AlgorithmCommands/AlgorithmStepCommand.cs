﻿using Floyd_Warshall_Model;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    public class AlgorithmStepCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;
        private readonly GraphModel _graphModel;

        public AlgorithmStepCommand(AlgorithmViewModel viewModel, GraphModel graphModel)
        {
            _viewModel = viewModel;
            _graphModel = graphModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _graphModel.StepAlgorithm();
        }

        public override bool CanExecute(object? parameter)
        {
            return  _viewModel.IsRunning && _viewModel.IsStopped && _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.IsInitialized) || e.PropertyName == nameof(AlgorithmViewModel.IsStopped)
                 || e.PropertyName == nameof(AlgorithmViewModel.IsRunning))
            {
                OnCanExecuteChanged();
            }
        }
    }
}