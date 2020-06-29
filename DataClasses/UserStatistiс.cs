using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chart.DataClasses
{
    [Serializable]
    public class UserStatistiс : INotifyPropertyChanged
    {
        public string name { get; set; }
        public int midSteps { get; set; }
        public int bestRes { get; set; }
        public int worstRes { get; set; }

        private string _status;
        [XmlIgnoreAttribute]
        [JsonIgnore]
        [Ignore]
        public string Status {
            get { return _status; }
            set { _status = value;
                OnPropertyChanged("Status");
            }
        }
        private UserStatistiс()
        {

        }
        public UserStatistiс(IEnumerable<UserData> userData)
        {
            name = userData.First().User;
            midSteps = userData.Sum(d => d.Steps) / userData.Count();
            bestRes = userData.Max(u => u.Steps);
            worstRes = userData.Min(u => u.Steps);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
