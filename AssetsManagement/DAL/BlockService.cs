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
    
    public partial class BlockService
    {
        public int Id { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public Nullable<int> Value { get; set; }
        public string Note { get; set; }
        public Nullable<int> OrgId { get; set; }
    }
}
