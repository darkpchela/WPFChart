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
using Chart.Services;

namespace Chart.Models
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private Chart chart = new Chart();
        private DataContainer dataContainer;
        private List<UserStatisticModel> _userStatistics = new List<UserStatisticModel>();
        private SeriesCollection _series;
        private UserStatisticModel _SelectedStatistic;
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
        public List<string> _ExportFormats =new List<string> {"json", "xml", "csv" };
        public List<string> ExportFormats
        {
            get { return _ExportFormats; }
        }
        private string _TextBoxPath;
        public string TextBoxPath
        {
            get { return _TextBoxPath??DataFolder; }
            set  
            {
                _TextBoxPath = value;
                OnPropertyChanged("TextBoxPath");
            }
        }
        private string _DataFolder;
        public string DataFolder
        {
            get { return _DataFolder ?? Path.Combine(Directory.GetCurrentDirectory(), "Data"); }
            set
            {
                dataContainer = new DataContainer(value);
                if (dataContainer.AllDatas.Any())
                { 
                    _DataFolder = value;
                    LoadStatistics();
                    OnPropertyChanged("DataFolder");
                }
                else
                {
                    TextBoxPath = _DataFolder;
                }
            }
        }

        public UserStatisticModel SelectedStatistic
        {
            get
            {
                return _SelectedStatistic;
            }
            set
            {
                _SelectedStatistic = value;
                if (SelectedStatistic != null)
                {
                    Plot();
                    CompareStatics();
                }
                OnPropertyChanged("SelectedStatistic");
            }
        }
        public List<UserStatisticModel> userStatistics
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public RelayCommand Export
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Exporter.Export(dataContainer, SelectedStatistic, SelectedFormat);
                    MessageBox.Show($"Экспорт завершен! \n Путь: {Exporter.LastExportFolder}");
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
                        DataFolder = TextBoxPath;
                });
                    }
        }
        public MainWindowViewModel()
        {
            DataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        }
        private void LoadStatistics()
        {
            var groupedDatas = dataContainer.AllDatas.GroupBy(ud => ud.User);
            List<UserStatisticModel> newUserStatistics = new List<UserStatisticModel>();
            foreach (var g in groupedDatas)
            {
                var datas = g.Select(d => d).ToList();
               newUserStatistics.Add(new UserStatisticModel(datas));
            }
            userStatistics = newUserStatistics;
           SelectedStatistic = userStatistics.First();
        }
        private void Plot()
        {
            var currentUserDatas = dataContainer.AllDatas.Where(d => d.User == SelectedStatistic.name);
            List<int> stepsAsPoints = new List<int>(currentUserDatas.Select(d => d.Steps));
            List<int> daysAsPoints = new List<int>();
            for (int i = 1; i < currentUserDatas.Count(); i++)
            {
                daysAsPoints.Add(i);
            }

            chart.SetData(daysAsPoints, stepsAsPoints);

            ChartValues<ObservablePoint> points = new ChartValues<ObservablePoint>();
            foreach (var point in chart.Points)
            {
                points.Add(new ObservablePoint(point.X, point.Y));
            }

            Series = new SeriesCollection {
                new LineSeries
                {
                    Values = points,
                    Fill = Brushes.Transparent
                } };
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
    }
}
