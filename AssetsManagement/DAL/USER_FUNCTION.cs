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
    
    public partial class USER_FUNCTION
    {
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public int FUNCTION_ID { get; set; }
        public Nullable<bool> FOpen { get; set; }
        public Nullable<bool> FUpdate { get; set; }
        public Nullable<bool> FSave { get; set; }
        public Nullable<bool> FDel { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<bool> FCommit { get; set; }
    }
}
