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

            _viewModel.SwitchLanguage += ViewModel_SwitchLanguage;

			_view = new MainWindow
            {
                DataContext = _viewModel
            };

			ViewModel_SwitchLanguage(this, new LanguageEventArgs("en"));

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
            string? graph = Resources["StrGraph"] as string;
			string? load = Resources["StrLoadGraph"] as string;
			string? fail = Resources["StrLoadError"] as string;
			string? error =Resources["StrError"] as string;
			string? title = Resources["StrTitle"] as string;

			try
			{
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = load,
                    Filter = $"{graph}|*.gph"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    await _graphModel.LoadAsync(openFileDialog.FileName);
                }
            }
            catch (GraphDataException)
            {
                MessageBox.Show(fail, $"{title} - {error}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private async void ViewModel_SaveGraph(object? sender, GraphLocationEventArgs e)
        {
			string? graph = Resources["StrGraph"] as string;
			string? save = Resources["StrSaveGraph"] as string;
			string? fail = Resources["StrSaveError"] as string;
			string? error = Resources["StrError"] as string;
			string? title = Resources["StrTitle"] as string;
            string? pathError = Resources["StrPathError"] as string;

			try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = save,
                    Filter = $"{graph}|*.gph"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _graphModel.SaveAsync(saveFileDialog.FileName, e.VertexLocations);
                    }
                    catch (GraphDataException)
                    {
                        MessageBox.Show(fail + Environment.NewLine + pathError, $"{title} - {error}",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
				MessageBox.Show(fail, $"{title} - {error}", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

        private void ViewModel_Exit(object? sender, EventArgs e)
        {
            _view.Close();
        }

		private void ViewModel_SwitchLanguage(object? sender, LanguageEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();

			switch (e.LangCode)
            {
                case "en":
                    dict.Source = new Uri("..\\Resources\\AppText.xaml", UriKind.Relative);
                    break;
				case "hu":
                    dict.Source = new Uri("..\\Resources\\AppText.hu-HU.xaml", UriKind.Relative);
					break;
				default:
                    dict.Source = new Uri("..\\Resources\\AppText.xaml", UriKind.Relative);
					break;
			}

            Resources.MergedDictionaries.Add(dict);
        }

		#endregion
	}
}
