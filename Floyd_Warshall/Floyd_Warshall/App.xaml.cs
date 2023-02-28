using Floyd_Warshall.View;
using Floyd_Warshall.ViewModel;
using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Persistence;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Floyd_Warshall
{
    public partial class App : Application
    {
        private GraphModel _graphModel;
        private MainWindow _view;
        private MainViewModel _viewModel;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _graphModel = new GraphModel(new GrapfFileDataAccess());
            _viewModel = new MainViewModel(_graphModel);

            _viewModel.NewGraph += new EventHandler<bool>(ViewModel_NewGraph);
            _viewModel.LoadGraph += new EventHandler(ViewModel_LoadGraph);
            _viewModel.SaveGraph += new EventHandler<IEnumerable<Tuple<Vertex, double, double>>>(ViewModel_SaveGraph);
            _viewModel.Exit += new EventHandler(ViewModel_Exit);

            _view = new MainWindow
            {
                DataContext = _viewModel
            };

            _view.Show();
        }


        #region ViewModel event handlers

        private void ViewModel_NewGraph(object sender, bool e) => _graphModel.NewGraph(e);
     

        private async void ViewModel_LoadGraph(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Load graph";
                openFileDialog.Filter = "Graph|*.gph";
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

        private async void ViewModel_SaveGraph(object sender, IEnumerable<Tuple<Vertex, double, double>> e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save graph";
                saveFileDialog.Filter = "Graph|*.gph";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _graphModel.SaveAsync(saveFileDialog.FileName, e);
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

        private void ViewModel_Exit(object sender, EventArgs e) => _view.Close();


        #endregion
    }
}
