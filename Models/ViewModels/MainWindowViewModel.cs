using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using Chart.Commands;
using System.IO;
using System.Globalization;
using System.Windows.Data;
using Chart.DataClasses;
using Chart.Models.ViewModels;
using LiveCharts.Configurations;

namespace Chart.Models
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public List<string> ExportFormats { get; } = new List<string> { "json", "xml", "csv" };

        private DatasModel datasModel;
        private ChartModel chartModel;

        private List<UserStatisticViewModel> _userStatistics;
        private UserStatisticViewModel _SelectedStatistic;
        private SeriesCollection _series;
        private string _TextBoxPath;
        private string _SelectedFormat;


        public string SelectedFormat
        {
            get { return _SelectedFormat ?? "json"; }
            set
            {
                _SelectedFormat = value;
                OnPropertyChanged("SelectedFormat");
            }
        }
        public string TextBoxPath
        {
            get { return _TextBoxPath ?? datasModel.CurrentFolderPath; }
            set
            {
                _TextBoxPath = value;
                OnPropertyChanged("TextBoxPath");
            }
        }

        public UserStatisticViewModel SelectedStatistic
        {
            get
            {
                return _SelectedStatistic;
            }
            set
            {
                _SelectedStatistic = value;
                SendDataToChartModel();
                Plot();
                CompareStatics();
                OnPropertyChanged("SelectedStatistic");
            }
        }
        public List<UserStatisticViewModel> userStatistics
        {
            get
            {
                return _userStatistics;
            }
            set
            {
                _userStatistics = value;
                OnPropertyChanged("userStatistics");
            }
        }
        public SeriesCollection Series
        {
            get
            {
                return _series ?? new SeriesCollection();
            }
            set
            {
                _series = value;
                OnPropertyChanged("Series");
            }
        }

        public RelayCommand Export
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (SelectedStatistic == null)
                    {
                        MessageBox.Show("Пользователь не выбран!");
                        return;
                    }
                    if (datasModel.TryExportData(SelectedStatistic.name, SelectedFormat))
                        MessageBox.Show($"Экспорт завершен! \n Путь: {datasModel.LastExportFolder}");
                    else
                        MessageBox.Show(datasModel.ErrorMessage);
                });
            }
        }
        public RelayCommand ChangeDataFolder
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Directory.Exists(TextBoxPath))
                    {
                        if (!datasModel.TryLoadDatasFromFolder(TextBoxPath))
                            MessageBox.Show(datasModel.ErrorMessage);
                        else
                        {
                            List<UserStatisticViewModel> newUserStatistics = new List<UserStatisticViewModel>();
                            foreach (var stat in datasModel.AllStatistics)
                            {
                                newUserStatistics.Add(new UserStatisticViewModel(stat));
                            }
                            userStatistics = newUserStatistics;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Указанная папка не существует!");
                    }
                });
            }
        }
        public MainWindowViewModel()
        {
            datasModel = new DatasModel();
            chartModel = new ChartModel();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private void SendDataToChartModel()
        {
            string userName = SelectedStatistic.name;
            var currentUserDatas = datasModel.AllDatas.Where(d => d.User == userName);
            List<int> stepsAsPoints = new List<int>(currentUserDatas.Select(d => d.Steps));
            List<int> daysAsPoints = new List<int>();
            for (int i = 1; i < currentUserDatas.Count(); i++)
            {
                daysAsPoints.Add(i);
            }
            chartModel.SetData(daysAsPoints, stepsAsPoints);
        }

        private void CompareStatics()
        {

            int currentMidRes = SelectedStatistic.midSteps;
            int deltaRes = (int)(currentMidRes * 0.20);
            foreach (var stat in userStatistics)
            {
                if (stat.midSteps < (currentMidRes - deltaRes))
                    stat.Status = "worse";
                else if (stat.midSteps > (currentMidRes + deltaRes))
                    stat.Status = "better";
                else
                    stat.Status = "normal";
            }
        }
        private void Plot()
        {
            int maxPoint = (int)chartModel.Points.OrderBy(p => p.Y).Last().Y;
            int minPoint = (int)chartModel.Points.OrderBy(p => p.Y).First().Y;

            CartesianMapper<ObservablePoint> mapper = Mappers.Xy<ObservablePoint>()
           .Y(item => item.Y)
           .Fill((item) =>
           {
               if (item.Y == maxPoint)
                   return Brushes.Green;
               if (item.Y == minPoint)
                   return Brushes.Red;
               return null;
           })
           .Stroke((item) =>
           {
               if (item.Y == maxPoint)
                   return Brushes.Green;
               if (item.Y == minPoint)
                   return Brushes.Red;
               return null;
           });

            Series = new SeriesCollection
                {
                        new LineSeries
                        {
                            Values = chartModel.Points,
                            Configuration = mapper,
                            Fill = Brushes.Transparent,
                            PointGeometrySize=15
                        }

                };
        }
    }
}
