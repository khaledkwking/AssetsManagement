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
    
    public partial class qids
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public qids()
        {
            this.qidsDetails = new HashSet<qidsDetails>();
        }
    
        public long qidid { get; set; }
        public string qidcode { get; set; }
        public Nullable<System.DateTime> qidDate { get; set; }
        public string description { get; set; }
        public Nullable<bool> edit { get; set; }
        public Nullable<int> recitetype { get; set; }
        public Nullable<bool> underupdate { get; set; }
        public string name { get; set; }
        public Nullable<long> BrachId { get; set; }
        public Nullable<long> EmpId { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> repeated { get; set; }
        public Nullable<long> BankAccId { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<long> InvId { get; set; }
        public Nullable<long> ReorderInvId { get; set; }
        public Nullable<bool> PaymentType { get; set; }
        public string PaymentTo { get; set; }
        public Nullable<long> QidSerial { get; set; }
        public Nullable<int> Contractid { get; set; }
        public Nullable<int> Carid { get; set; }
        public Nullable<int> Custid { get; set; }
        public Nullable<long> ReorderId { get; set; }
        public Nullable<int> OfficeId { get; set; }
        public Nullable<int> OrgId { get; set; }
    
        public virtual Contracts Contracts { get; set; }
        public virtual Recite Recite { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<qidsDetails> qidsDetails { get; set; }
    }
}
