using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace semestralka_pudilova
{
    class ExportHelper
    {
        private string uri = "ues:UCL-BT:UCL.INF/DEMO_REZERVACE:EBC.MCS.DEMO/MR001/SCHEDULE";
        private string schema = "PLUS4U.EBC.MCS.MeetingRoom_Schedule_1.0";

        [JsonProperty("schema", Order = 0)]
        public string Schema
        {
            get { return schema; }
        }

        [JsonProperty("uri", Order = 1)]
        public string Uri
        {
            get { return uri; }
        }

        [JsonProperty("data", Order = 2)]
        public List<Room> _data;

        public ExportHelper(List<Room> data)
        {
            _data = data.Where(p => p.Reservations.Count > 0).ToList(); //Vybere pouze ty meeting roomy, které mají nějaké meetingy
        }
    }
}
