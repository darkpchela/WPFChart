using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Chart.Models;
using System.Xml.Serialization;
using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Chart.Services
{
    public static class Exporter
    {
        public static string LastExportFolder;
        public static void Export(DataContainer dataContainer, UserStatisticModel userStatistic, string mode="json")
        {
            try
            {
                string ExportPath = Path.Combine(Directory.GetCurrentDirectory(), "Export", userStatistic.name);
                string datasFileName = Path.Combine(ExportPath, "Данные за все дни.");
                string statisticFileName = Path.Combine(ExportPath, "Общая статистика.");
                UserData[] userDatas = dataContainer.AllDatas.Select(u => u).Where(u => u.User == userStatistic.name).ToArray();

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
                        string statisticData = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(userStatistic, options));
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
                        formatter = new XmlSerializer(typeof(UserStatisticModel));
                        using (StreamWriter sw = new StreamWriter(statisticFileName, false, Encoding.UTF8))
                        {
                            formatter.Serialize(sw, userStatistic);
                        }
                        break;
                    case "csv":
                        datasFileName += "csv";
                        statisticFileName += "csv";
                        using (StreamWriter sw = new StreamWriter(datasFileName,false ,Encoding.UTF8))
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
                                csvWriter.WriteRecords(new List<UserStatisticModel> { userStatistic });
                            }
                        }
                        break;
                    default:
                        break;
                }
                LastExportFolder = ExportPath;
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
