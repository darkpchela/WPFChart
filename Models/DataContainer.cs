using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Chart.Models
{
    public class DataContainer
    {
        public string ErrorMessage;
        public string FolderPath { get; set; }
        public List<UserData> AllDatas { get; set; } = new List<UserData>();

        public DataContainer(string path)
        {
            this.FolderPath = path;
            try
            {
                var fileList = Directory.GetFiles(FolderPath);
                int dayNum = 1;
                foreach (var file in fileList)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        UserData[] datas = JsonSerializer.Deserialize<UserData[]>(sr.ReadToEnd());
                        datas.ToList().ForEach(d => {
                            if (d.dayNum==null)
                            {
                                d.dayNum = dayNum;
                            }
                        });
                        AllDatas.AddRange(datas);
                    }
                    dayNum++;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}
