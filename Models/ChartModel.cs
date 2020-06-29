using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chart.Models
{
    class ChartModel: INotifyPropertyChanged
    {
        private ChartValues<ObservablePoint> _points;
        public IEnumerable<int> xPoints { get; private set; }
        public IEnumerable<int> yPoints { get; private set; }
        public ChartValues<ObservablePoint> Points 
        {
            get { return (_points??new ChartValues<ObservablePoint>()); }
            set 
            {
                _points = value;
                OnPropertyChanged("Points");
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public void SetData(IEnumerable<int> xPoints, IEnumerable<int> yPoints)
        {
            this.xPoints = xPoints;
            this.yPoints = yPoints;
            ChartValues<ObservablePoint> newPoints = new ChartValues<ObservablePoint>();
            for (int i = 0; i < Math.Min(xPoints.Count(), yPoints.Count()); ++i)
            {
               newPoints.Add(new ObservablePoint(xPoints.ElementAt(i), yPoints.ElementAt(i)));
            }
            Points = newPoints;
        }
    }
}
