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

namespace Chart.Models
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public List<string> ExportFormats { get; } =new List<string> {"json", "xml", "csv" };
        private DatasModel datasModel = new DatasModel();
        private ChartModel _chartModel = new ChartModel();
        private List<UserStatistiс> _userStatistics = new List<UserStatistiс>();
        private SeriesCollection _series;
        private UserStatistiс _SelectedStatistic;


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
        //public List<string> ExportFormats
        //{
        //    get { return _ExportFormats; }
        //}
        private string _TextBoxPath;
        public string TextBoxPath
        {
            get { return _TextBoxPath??datasModel.CurrentFolderPath; }
            set  
            {
                _TextBoxPath = value;
                OnPropertyChanged("TextBoxPath");
            }
        }

        public UserStatistiс SelectedStatistic
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
                    SendDataToChartModel();
                    Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Values = _chartModel.Points,
                            Fill = Brushes.Transparent
                        } 
                    };
                    CompareStatics();
                }
                OnPropertyChanged("SelectedStatistic");
            }
        }
        public List<UserStatistiс> userStatistics
        {
            get
            {
                return datasModel.AllStatistics;
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
                            OnPropertyChanged("userStatistics");
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
            //DataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        }
        private void SendDataToChartModel()
        {
            var currentUserDatas = datasModel.AllDatas.Where(d => d.User == SelectedStatistic.name);
            List<int> stepsAsPoints = new List<int>(currentUserDatas.Select(d => d.Steps));
            List<int> daysAsPoints = new List<int>();
            for (int i = 1; i < currentUserDatas.Count(); i++)
            {
                daysAsPoints.Add(i);
            }
            _chartModel.SetData(daysAsPoints, stepsAsPoints);
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
