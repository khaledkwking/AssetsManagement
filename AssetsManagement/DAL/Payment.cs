//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AssetsManagement.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public string ItemType { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string BillNo { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string PaymentSourceName { get; set; }
        public string Tel { get; set; }
        public string Notes { get; set; }
        public Nullable<int> OrgId { get; set; }
    }
}
