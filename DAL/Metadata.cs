using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EntityFramework.DynamicFilters;
using System.ComponentModel;
using System.Net.Http;

namespace DAL
{

    public class ClassRequestOutOrdersAttrib
    {
 
        [Required]
        public int DeptId { get; set; }

        [Required]
        public int LocationId { get; set; }
    }

    public partial class tbUsersDepts
    {
        // leave it empty.
        public virtual vwDepartments VmDepartments { get; set; }

    }


    [MetadataType(typeof(ClassRequestOutOrdersAttrib))]
    public partial class RequestOutOrders
    {
        // leave it empty.
        public virtual vwEmployees VmEmployees { get; set; }
        public virtual vwDepartments VmDepartments { get; set; }


    }

    public partial class Room_tbl
    {
        public Boolean busyRoom { get; set; }
        public String EmpNames { get; set; }
    }
        // partials classes scope
        [MetadataType(typeof(StockAttrib))]
    public partial class tbl_ItemsStock
    {
        // leave it empty.
        public string ItemName
        {
            get { return Item_tbl.Item_Name; }
        }
        public string RoomNameAr
        {
            get { return Room_tbl.Room_Name; }
        }
        public string EmpNameAr
        {
            get {
                return VmEmployees != null ? VmEmployees.FULL_NAME_AR : "";
            }
        }
        public string DeptNameAr
        {
            get { return VmDepartments !=null? VmDepartments.Name: ""; }
        }
        public virtual vwEmployees VmEmployees { get; set; }
        public virtual vwDepartments VmDepartments { get; set; }
        [DefaultValue(false)]
        public Boolean ItemSelected { get; set; }
    }
    
   [MetadataType(typeof(ClassOutOrderAttrib))]
    public partial class OutOrders
    {
        // leave it empty.
        public virtual vwEmployees VmEmployees { get; set; }
        public virtual vwDepartments VmDepartments { get; set; }
      

    }

    [MetadataType(typeof(HandoverOrdersAttrib))]
    public partial class HandoverOrders
    {
        // leave it empty.
        //public virtual vwEmployees VmEmployees { get; set; }
        //public virtual vwDepartments VmDepartments { get; set; }
        public virtual Room_tbl Room_tbl { get; set; }
        //public virtual Room_tbl Room_tbl1 { get; set; }

    }

    [MetadataType(typeof(ChangeQuantityOrdersAttrib))]
    public partial class ChangeQuantityOrders
    {
        // leave it empty.
        public virtual Room_tbl FromStores { get; set; }
       // public virtual Room_tbl ToStore { get; set; }


    }

    [MetadataType(typeof(ClassInOrderAttrib))]
    public partial class InOrders
    {
        // leave it empty.
        public virtual Room_tbl FromStore { get; set; }
        public virtual Room_tbl ToStore { get; set; }

    }
    [MetadataType(typeof(ClassTransferOrdersAttrib))]
    public partial class TransferOrders
    {
        // leave it empty.
        public virtual Room_tbl FromStore { get; set; }
        public virtual Room_tbl ToStore { get; set; }

    }

    [MetadataType(typeof(ClassInOrdersDetailsAttrib))]
    public partial class InOrdersDetails
    {
        public virtual string ItemName { get; set; }
        public virtual int ? ItemInStoreQty { get; set; }
    }
    public partial class OutOrdersDetails
    {
        public virtual int? ItemInStoreQty { get; set; }
    }
    //[MetadataType(typeof(ClassInOrdersDetailsAttrib))]
    public partial class HandOverOrdersDetails
    {
        public virtual vwEmployees VmEmployees { get; set; }
        public virtual vwDepartments VmDepartments { get; set; }
        public virtual Room_tbl Room_tbl { get; set; }
             
    }
    public partial class vwOutOrderDetails
    {
        public string EmpName { get; set; }

        public string DeptName { get; set; }

    }
    public partial class vwAllOrders
    {
        public string EmpName { get; set; }

        public string DeptName { get; set; }

    }
    
    public partial class vwHandOverOrdersDetails
    {
        public string EmpName { get; set; }
        public string DeptName { get; set; }

    }
    public partial class vwDestroyOrdersDetails
    {
        public string EmpName { get; set; }
        public string DeptName { get; set; }

    }
    [MetadataType(typeof(ClassDestroyAttrib))]
    public partial class DestroyOrders
    {
        // leave it empty.
        public virtual Room_tbl FromStore { get; set; }
        public virtual Room_tbl ToStore { get; set; }

    }
    public partial class vwItemsStock
    {
        public string EmpName { get; set; }

        public string DeptName { get; set; }
        public string EmpTitle { get; set; }

    }
    public partial class vwEmpRooms
    {
          public string DeptName { get; set; }
     
    }
    public partial class TransferAssets
    {
        public string RoomNameFrom { get; set; }
        public string RoomNameTo { get; set; }
        public string DeptNameFrom { get; set; }
        public string DeptNameTo { get; set; }
        public string EmpNameFrom { get; set; }
        public string EmpNameTo { get; set; }
        public string userName { get; set; }
    }
    public partial class vwTransferAssests
    {
        public string EmpFromName { get; set; }
        public string DeptFromName { get; set; }
        public string EmpToName { get; set; }
        public string DeptToName { get; set; }

    }
    //----------------------------------

    //------------------------------------------------
    [MetadataType(typeof(EmpRoomsAttrib))]
    public partial class Emp_rooms
    {
        public virtual vwEmployees VmEmployees { get; set; }
        public virtual vwDepartments VmDepartments { get; set; }

    }

    [MetadataType(typeof(BuildingsAttrib))]
    public partial class Building_tbl
    {
        // leave it empty.
    }


    [MetadataType(typeof(FloorsAttrib))]
    public partial class Floor_tbl
    {
        // leave it empty.
    }
    [MetadataType(typeof(ClassCatMainAttrib))]
    public partial class CatMain_tbl
    {

    }
    [MetadataType(typeof(ClassCatSubAttrib))]
    public partial class CatSub_tbl
    {

    }
    [MetadataType(typeof(ClassCategoryAttrib))]
    public partial class Category_tbl
    {

    }

    [MetadataType(typeof(ClassItemAttrib))]
    public partial class Item_tbl
    {

    }
   
    [MetadataType(typeof(ClassUserAttrib))]
    public partial class tbUsers
    {
        public virtual vwEmployees VmEmployees { get; set; }
        //public virtual vwDepartments VmDepartments { get; set; }
       
    }
    // end of partial Class Scope


    // start of Attributes scope
  
    public class ClassUserAttrib
    {
        [Required]
        public string Password { get; set; }

    }
    public class ClassCatMainAttrib
    {
        [Required]
           public string CatMain_Name { get; set; }
    }
    public class ClassCatSubAttrib
    {
        [Required]
        public string CatSub_Name { get; set; }
    }
    public class ClassCategoryAttrib
    {
        [Required]
        public string Cat_Name { get; set; }
    }

    public class ClassItemAttrib
    {
        [Required]
        public string Item_Name { get; set; }
    }
    
    public class ClassInOrderAttrib
    {
        //[Range(1, 99, ErrorMessage = "Author field is required.")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        //[ValidateDateRange]
        public DateTime InOrderDate { get; set; }

        [Required]
        public virtual int? SupplierId_From { get; set; }
        [Required]
        public long StoreId_To { get; set; }

        [DataType(DataType.MultilineText)]
       
        public string Remarks { get; set; }

    }
    public class ClassTransferOrdersAttrib
    {
        //[Range(1, 99, ErrorMessage = "Author field is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        public DateTime TransferOrderDate { get; set; }
               
        [Required]
        public long StoreId_To { get; set; }
        [Required]
        public long StoreId_From { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }


    }
    public class ClassInOrdersDetailsAttrib
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExpiredDate { get; set; }
       
    }
    public class ClassOutOrderAttrib
    {
        static string StartDate = DateTime.Now.AddDays(-3).ToShortDateString();
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
       
        public DateTime OutOrderDate { get; set; }

        [Required]
        public int StoreId { get; set; }
    }
    public class HandoverOrdersAttrib
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OverOrderDate { get; set; }
    }
    public class ChangeQuantityOrdersAttrib
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ChangeOrderDate { get; set; }
    }
    public class ClassDestroyAttrib
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        public DateTime DestroyOrderDate { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public int StoreId_From { get; set; }
    }
   
    public class BuildingsAttrib
    {
        [Display(Name = "BuildingName")]
        [Required]
        public string Building_Name { get; set; }
        // and other properties you want...
    }
    public class FloorsAttrib
    {
        [Display(Name = "FloorName")]
        [Required]
        public string Floor_Name { get; set; }
        // and other properties you want...
    }

    public class StockAttrib
    {
        //[Display(Name = "Item_Expire")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string Item_Expire { get; set; }
        //// and other properties you want...
        //[Display(Name = "ItemId")]
        //[Required]
        //public string Item_Id { get; set; }

        //[Display(Name = "ItemQty")]
        //[Required]
        //[DefaultValue(1)]
        //public string ItemQty { get; set; }
        // and other properties you want...

    }
    public class EmpRoomsAttrib
    {
        //[Display(Name = "Room_Name")]
        //[Required]
        //public string Room_Name { get; set; }
        // and other properties you want...
    }
    public class RoomAttrib
    {
        [Display(Name = "DeptId")]
        [Required]
        public int DeptId { get; set; }
        // and other properties you want...
    }
    public partial class GPFAssetsEntities
    {
        // leave it empty.
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Building_tbl>().Map(m => m.Requires("IsDeleted").HasValue(false));
        //    //modelBuilder.Filter("IsDeleted", (ISoftDelete d) => d.IsDeleted, false);
        //   // modelBuilder.Entity<Building_tbl>().HasQueryFilter(p => !p.IsDeleted);

        //}
    }
    public class ValidateDateRange : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // your validation logic
            if (Convert.ToDateTime(value.ToString()) >= DateTime.Today)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Date is not in given range.");
            }
        }
    }
}
