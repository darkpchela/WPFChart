using Chart.DataClasses;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chart.Models
{
    public class DatasModel
    {
        public string CurrentFolderPath { get; private set; }
        public string ErrorMessage { get; private set; }
        public string LastExportFolder { get; private set; }
        public List<UserStatistiс> AllStatistics { get; set; }
        public List<UserData> AllDatas { get; set; }

        public bool TryLoadDatasFromFolder(string path)
        {
            try
            {
                var fileList = Directory.GetFiles(path, "*.json");
                int dayNum = 1;
                List<UserData> newDatas = new List<UserData>();
                foreach (var file in fileList)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        UserData[] datas = JsonSerializer.Deserialize<UserData[]>(sr.ReadToEnd());
                        datas.ToList().ForEach(d => {
                            if (d.dayNum == null)
                            {
                                d.dayNum = dayNum;
                            }
                        });
                        newDatas.AddRange(datas);
                    }
                    dayNum++;
                }
                AllDatas = newDatas;
                CurrentFolderPath = path;
                LoadStatistics();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка десериализации, воз";
                return false;
            }

        }
        public bool TryExportData(string userName, string mode = "json")
        {
            try
            {
                UserStatistiс userStatistiс = AllStatistics.Find(s=>s.name==userName);
                if (userStatistiс==null)
                    throw new Exception("Пользователь с таким именем не найден!");

                string ExportPath = Path.Combine(Directory.GetCurrentDirectory(), "Export", userStatistiс.name);
                string datasFileName = Path.Combine(ExportPath, "Данные за все дни.");
                string statisticFileName = Path.Combine(ExportPath, "Общая статистика.");
                UserData[] userDatas = AllDatas.Select(u => u).Where(u => u.User == userStatistiс.name).ToArray();

                if (!Directory.Exists(ExportPath))
                    Directory.CreateDirectory(ExportPath);

                switch (mode)
                {
                    case "json":
                        datasFileName += "json";
                        statisticFileName += "json";

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };

                        string datas = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(userDatas, options));
                        datas = Regex.Replace(datas, @"\\u([0-9A-Fa-f]{4})", m => "" + (char)Convert.ToInt32(m.Groups[1].Value, 16));
                        string statisticData = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(userStatistiс, options));
                        statisticData = Regex.Replace(statisticData, @"\\u([0-9A-Fa-f]{4})", m => "" + (char)Convert.ToInt32(m.Groups[1].Value, 16));
                        using (StreamWriter sw = new StreamWriter(datasFileName, false, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(datas);
                        }
                        using (StreamWriter sw = new StreamWriter(statisticFileName, false, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(statisticData);
                        }
                        break;
                    case "xml":
                        datasFileName += "xml";
                        statisticFileName += "xml";
                        XmlSerializer formatter = new XmlSerializer(typeof(UserData[]));
                        using (StreamWriter sw = new StreamWriter(datasFileName, false, Encoding.UTF8))
                        {
                            formatter.Serialize(sw, userDatas.ToArray());
                        }
                        formatter = new XmlSerializer(typeof(UserStatistiс));
                        using (StreamWriter sw = new StreamWriter(statisticFileName, false, Encoding.UTF8))
                        {
                            formatter.Serialize(sw, userStatistiс);
                        }
                        break;
                    case "csv":
                        datasFileName += "csv";
                        statisticFileName += "csv";
                        using (StreamWriter sw = new StreamWriter(datasFileName, false, Encoding.UTF8))
                        {
                            using (CsvWriter csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
                            {
                                csvWriter.Configuration.Delimiter = ",";
                                csvWriter.WriteRecords(userDatas.ToArray());
                            }
                        }
                        using (StreamWriter sw = new StreamWriter(statisticFileName, false, Encoding.UTF8))
                        {
                            using (CsvWriter csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
                            {
                                csvWriter.Configuration.Delimiter = ",";
                                csvWriter.WriteRecords(new List<UserStatistiс> { userStatistiс });
                            }
                        }
                        break;
                    default:
                        break;
                }
                LastExportFolder = ExportPath;
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
        private void LoadStatistics()
        {
            List<UserStatistiс> newUserStatistics = new List<UserStatistiс>();
            if (AllDatas.Any())
            {
                var groupedDatas = AllDatas.GroupBy(ud => ud.User);
                foreach (var g in groupedDatas)
                {
                    var datas = g.Select(d => d).ToList();
                    newUserStatistics.Add(new UserStatistiс(datas));
                }
            }
            AllStatistics = newUserStatistics;
        }
    }
}
