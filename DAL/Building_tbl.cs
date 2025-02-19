
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
    
public partial class Building_tbl
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Building_tbl()
    {

        this.Floor_tbl = new HashSet<Floor_tbl>();

        this.RequestOutOrders = new HashSet<RequestOutOrders>();

    }


    public int Building_Id { get; set; }

    public string Building_Name { get; set; }

    public string Building_NameEn { get; set; }

    public string Building_Code { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<int> UpdateBy { get; set; }

    public Nullable<int> DeletedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> UpdateOn { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public Nullable<bool> IsDeleted { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Floor_tbl> Floor_tbl { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<RequestOutOrders> RequestOutOrders { get; set; }

}

}
