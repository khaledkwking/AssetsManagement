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
    
    public partial class violation
    {
        public long violationId { get; set; }
        public Nullable<System.DateTime> violationDate { get; set; }
        public string violationNo { get; set; }
        public Nullable<int> CarID { get; set; }
        public string Note { get; set; }
        public Nullable<int> Contractid { get; set; }
        public Nullable<int> BookNo { get; set; }
        public string ViolationTime { get; set; }
        public Nullable<int> OrgId { get; set; }
    
        public virtual Car Car { get; set; }
    }
}