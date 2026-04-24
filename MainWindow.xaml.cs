using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace ML_Log_Analyzer
{
    public partial class MainWindow : FluentWindow
    {
        private string _loadedLogPath = string.Empty;
        private AppSettings _settings = new();

        public MainWindow()
        {
            InitializeComponent();
            _settings = SettingsManager.Load();

            if (!string.IsNullOrEmpty(_settings.ApiKey))
                ApiKeyBox.Password = _settings.ApiKey;
        }

        private void OpenLogButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Log Files (*.log;*.txt)|*.log;*.txt|All Files (*.*)|*.*",
                Title = "Select Log File"
            };

            if (dialog.ShowDialog() == true)
            {
                _loadedLogPath = dialog.FileName;
                var content = File.ReadAllText(_loadedLogPath);
                LogContentBox.Text = content;

                HomePanel.Visibility = Visibility.Collapsed;
                AnalysisPanel.Visibility = Visibility.Visible;
                AnalysisContentBox.Text = string.Empty;
                SaveButton.IsEnabled = false;
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.ApiKey = ApiKeyBox.Password;
            _settings.Model = (ModelComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            SettingsManager.Save(_settings);
            System.Windows.MessageBox.Show("Settings saved successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AnalysisPanel.Visibility = Visibility.Collapsed;
            HomePanel.Visibility = Visibility.Visible;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                System.Windows.MessageBox.Show("Please enter your API key in Settings.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(LogContentBox.Text))
            {
                System.Windows.MessageBox.Show("No log content to analyze.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            StartButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            LoadingText.Visibility = Visibility.Visible;
            AnalysisContentBox.Text = string.Empty;

            try
            {
                var model = (ModelComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? _settings.Model;
                var result = await OpenRouterService.AnalyzeLogAsync(_settings.ApiKey, LogContentBox.Text, model);

                AnalysisContentBox.Text = result;
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                StartButton.IsEnabled = true;
                LoadingText.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Save Analysis Result",
                DefaultExt = ".txt",
                FileName = "analysis_result"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, AnalysisContentBox.Text);
                System.Windows.MessageBox.Show("Analysis saved successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}