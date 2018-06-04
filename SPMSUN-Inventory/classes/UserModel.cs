using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMSUN_Inventory.classes
{
    public static class UserModel
    {

        public static string ID { get; set; }

        public static string FirstName { get; set; }

        public static string LastName { get; set; }

        public static string Username { get; set; }

        public static string Password { get; set; }

        public static string UserType { get; set; }

        public static AccountType TypeOfAccount { get; set; }

        public static bool Loginsuccess { get; set; }

    }
}
