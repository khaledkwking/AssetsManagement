//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Debits
    {
        public long Id { get; set; }
        public Nullable<int> CusId { get; set; }
        public Nullable<int> CarId { get; set; }
        public Nullable<System.DateTime> EnterDate { get; set; }
        public Nullable<decimal> RValue { get; set; }
        public string Notes { get; set; }
        public Nullable<int> OrgId { get; set; }
    }
}