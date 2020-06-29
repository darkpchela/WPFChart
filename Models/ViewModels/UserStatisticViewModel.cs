using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Chart.DataClasses;
using CsvHelper.Configuration.Attributes;

namespace Chart.Models.ViewModels
{
    public class UserStatisticViewModel:INotifyPropertyChanged
    {
        public string name { get; set; }
        public int midSteps { get; set; }
        public int bestRes { get; set; }
        public int worstRes { get; set; }
        private string _status;
        public string Status {
            get{ return _status; }
            set {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        public UserStatisticViewModel(UserStatistiс userStatistiс, string status="normal")
        {
            this.name = userStatistiс.name;
            this.midSteps = userStatistiс.midSteps;
            this.bestRes = userStatistiс.bestRes;
            this.worstRes = userStatistiс.worstRes;
            this.Status = status;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
