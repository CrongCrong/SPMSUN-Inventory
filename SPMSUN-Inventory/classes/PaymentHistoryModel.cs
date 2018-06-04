using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMSUN_Inventory.classes
{
    public class PaymentHistoryModel
    {

        public string ID { get; set; }

        public string ClientID { get; set; }

        public string OrderID { get; set; }

        public string AmountPaid { get; set; }

        public string Date { get; set; }

        public string Notes { get; set; }

    }
}
