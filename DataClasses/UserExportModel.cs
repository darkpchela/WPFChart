using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chart.DataClasses
{
    [Serializable]
    class UserExportModel
    {
        UserStatistiс userStatistic;
        UserData[] userDatas;
        public UserExportModel(UserStatistiс userStatistic, IEnumerable<UserData> userDatas)
        {
            this.userStatistic = userStatistic;
            this.userDatas = userDatas.ToArray();
        }
    }
}
