
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
    
public partial class Floor_tbl
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Floor_tbl()
    {

        this.Room_tbl = new HashSet<Room_tbl>();

    }


    public int Floor_Id { get; set; }

    public int Building_Id { get; set; }

    public string Floor_Name { get; set; }

    public string Floor_NameEn { get; set; }

    public string Floor_Code { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<int> UpdateBy { get; set; }

    public Nullable<int> DeletedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> UpdateOn { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public Nullable<bool> IsDeleted { get; set; }



    public virtual Building_tbl Building_tbl { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Room_tbl> Room_tbl { get; set; }

}

}
