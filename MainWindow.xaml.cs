using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using Chart.Models;

namespace Chart
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
    
}
