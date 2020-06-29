using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Wpf;
using LiveCharts;
using System.IO;
using System.Text.Json;
using LiveCharts.Defaults;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows;

namespace Chart.Models
{
    public class Chart
    {
        public IEnumerable<int> xPoints { get; private set; }
        public IEnumerable<int> yPoints { get; private set; }
        public List<Point> Points { get; private set; }

        public void SetData(IEnumerable<int> xPoints, IEnumerable<int> yPoints)
        {
            this.xPoints = xPoints;
            this.yPoints = yPoints;
            List<Point> newPoints = new List<Point>();
            for (int i = 0; i < Math.Min(xPoints.Count(), yPoints.Count()); ++i)
            {
                newPoints.Add(new Point(xPoints.ElementAt(i), yPoints.ElementAt(i)));
            }
            this.Points = newPoints;
        }
    }
}
