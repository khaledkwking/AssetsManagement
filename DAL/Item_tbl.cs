
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
    
public partial class Item_tbl
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Item_tbl()
    {

        this.RequestOrdersDetails = new HashSet<RequestOrdersDetails>();

        this.tbl_ItemsStock = new HashSet<tbl_ItemsStock>();

    }


    public long Item_Id { get; set; }

    public string Item_Name { get; set; }

    public string Item_NameEn { get; set; }

    public Nullable<int> Unit_Id { get; set; }

    public Nullable<int> CatSub_Id { get; set; }

    public string Item_Dec { get; set; }

    public Nullable<int> Item_Count { get; set; }

    public Nullable<double> Item_Price { get; set; }

    public string Item_Remarks { get; set; }

    public string Item_BarCode { get; set; }

    public string Item_RFID { get; set; }

    public Nullable<long> Room_Id { get; set; }

    public Nullable<int> Emp_Id { get; set; }

    public Nullable<int> Depart_Id { get; set; }

    public Nullable<System.DateTime> Item_AssDate { get; set; }

    public Nullable<int> Item_LowQty { get; set; }

    public Nullable<System.DateTime> Item_Expire { get; set; }

    public bool CountableFlag { get; set; }

    public bool OrganizedFlag { get; set; }

    public string PictureName { get; set; }

    public Nullable<int> Item_StateId { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<int> UpdateBy { get; set; }

    public Nullable<int> DeletedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> UpdateOn { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<int> Merge_Item_Id { get; set; }

    public Nullable<int> Age { get; set; }

    public bool OutFlag { get; set; }



    public virtual CatSub_tbl CatSub_tbl { get; set; }

    public virtual Unit_tbl Unit_tbl { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<RequestOrdersDetails> RequestOrdersDetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tbl_ItemsStock> tbl_ItemsStock { get; set; }

}

}