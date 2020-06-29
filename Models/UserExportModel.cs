using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chart.Models
{
    [Serializable]
    class UserExportModel
    {
        UserStatisticModel userStatistic;
        UserData[] userDatas;
        public UserExportModel(UserStatisticModel userStatistic, IEnumerable<UserData> userDatas)
        {
            this.userStatistic = userStatistic;
            this.userDatas = userDatas.ToArray();
        }
    }
}
