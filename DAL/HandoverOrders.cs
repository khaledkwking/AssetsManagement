
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
    
public partial class HandoverOrders
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public HandoverOrders()
    {

        this.HandOverOrdersDetails = new HashSet<HandOverOrdersDetails>();

    }


    public long OverOrderId { get; set; }

    public Nullable<System.DateTime> OverOrderDate { get; set; }

    public Nullable<long> StoreId { get; set; }

    public Nullable<int> ReasonId { get; set; }

    public Nullable<int> OrganizedFlag { get; set; }

    public string Remarks { get; set; }

    public string PictureName { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<int> UpdateBy { get; set; }

    public Nullable<int> DeletedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> UpdateOn { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<bool> Active { get; set; }

    public Nullable<int> EmpId { get; set; }



    public virtual tbLookups tbLookups { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HandOverOrdersDetails> HandOverOrdersDetails { get; set; }

}

}
