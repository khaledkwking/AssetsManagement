
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
    
public partial class InOrdersDetails
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public InOrdersDetails()
    {

        this.ReturnInOrdersDetails = new HashSet<ReturnInOrdersDetails>();

    }


    public long InOrderDetId { get; set; }

    public Nullable<long> InOrderId { get; set; }

    public Nullable<long> ItemId { get; set; }

    public string Item_RFID { get; set; }

    public Nullable<long> StockId { get; set; }

    public string Item_BarCode { get; set; }

    public Nullable<int> Qty { get; set; }

    public Nullable<decimal> CostPrice { get; set; }

    public Nullable<int> InStoreQty { get; set; }

    public Nullable<System.DateTime> ExpiredDate { get; set; }

    public Nullable<long> StoreId { get; set; }

    public Nullable<bool> CountableFlag { get; set; }

    public Nullable<int> ReaderTypeId { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<int> UpdateBy { get; set; }

    public Nullable<int> DeletedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<System.DateTime> UpdateOn { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<int> ContractId { get; set; }



    public virtual InOrders InOrders { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ReturnInOrdersDetails> ReturnInOrdersDetails { get; set; }

    public virtual tbl_ItemsStock tbl_ItemsStock { get; set; }

}

}
