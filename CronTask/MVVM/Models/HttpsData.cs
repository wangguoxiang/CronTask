using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronTask.MVVM.Models
{
    public struct HttpsData
    {
        public int ID { get; set; }
        public string DNS { get; set; }
        public string HaveTime { get; set; }
        public string IsChecker { get; set; }
        public string Alerte { get; set; }
    }
}
