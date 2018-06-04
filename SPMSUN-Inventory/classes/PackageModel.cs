using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMSUN_Inventory.classes
{
    public class PackageModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NonMemberPrice { get; set; }

        public string MemberPrice { get; set; }

        public string HomePrice { get; set; }

        public string MegaPrice { get; set; }

        public string DepotPrice { get; set; }

        public string EmployeePrice { get; set; }

        public string Total { get; set; }

        public string Qty { get; set; }

        public string Unilevel { get; set; }

        public string LB { get; set; }

        public string MFG { get; set; }

        public List<ProductModel> ProductList { get; set; }

    }
}
