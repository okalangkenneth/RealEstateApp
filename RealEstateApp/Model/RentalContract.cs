using System;

namespace RealEstateApp.Model
{
    public class RentalContract
    {
        public int ContractID { get; set; }
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public DateTime StartData { get; set; }
        public DateTime EndDate { get; set; }
        public int RentAmount { get; set; }
        public int DepositAmount { get; set; }


    }
}
