using Floyd_Warshall.View;
using Floyd_Warshall.ViewModel;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;
using Floyd_Warshall_Model.Persistence;
using Microsoft.Win32;
using System;
using System.Windows;

namespace Floyd_Warshall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private GraphModel _graphModel = null!;
        private MainViewModel _viewModel = null!;
        private MainWindow _view = null!;

        #endregion

        #region Constructors

        public App()
        {
            Startup += App_Startup;
        }

        #endregion

        #region App event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _graphModel = new GraphModel(new GraphFileDataAccess());
            _viewModel = new MainViewModel(_graphModel);

            _viewModel.NewGraph += ViewModel_NewGraph;
            _viewModel.LoadGraph += ViewModel_LoadGraph;
            _viewModel.SaveGraph += ViewModel_SaveGraph;
            _viewModel.Exit += ViewModel_Exit;

            _view = new MainWindow
            {
                DataContext = _viewModel
            };

            _view.Show();
        }

        #endregion

        #region ViewModel event handlers

        private void ViewModel_NewGraph(object? sender, NewGraphEventArgs e)
        {
            _graphModel.NewGraph(e.IsDirected);
        }
     
        private async void ViewModel_LoadGraph(object? sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Load graph",
                    Filter = "Graph|*.gph"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    await _graphModel.LoadAsync(openFileDialog.FileName);
                }
            }
            catch (GraphDataException)
            {
                MessageBox.Show("Load failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private async void ViewModel_SaveGraph(object? sender, GraphLocationEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Save graph",
                    Filter = "Graph|*.gph"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _graphModel.SaveAsync(saveFileDialog.FileName, e.VertexLocations);
                    }
                    catch (GraphDataException)
                    {
                        MessageBox.Show("Save failed!" + Environment.NewLine + "The path is incorrect or the directory cannot be written.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Save failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_Exit(object? sender, EventArgs e)
        {
            _view.Close();
        }

        #endregion
    }
}
