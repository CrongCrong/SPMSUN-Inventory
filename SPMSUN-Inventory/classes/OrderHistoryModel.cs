using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMSUN_Inventory.classes
{
    public class OrderHistoryModel
    {

        public string ID { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string DRNumber { get; set; }

        public string Total { get; set; }

        public string ifPaid { get; set; }

        public string ifCancelled { get; set; }

        public string DRDate { get; set; }
    }
}
