using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            
          
        }
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
               
        public string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public int? PageNumber { get; set; }
        public int? PageCount { get; set; }


        [DefaultValue(1)]
        public int PostFlag { get; set; }
        public int searchType { get; set; }

        //[DefaultValue(1)]
        public int ReaderType { get; set; }
        //[DefaultValue(1)]
        public bool RFIDFlag { get; set; }

        public string EditMode { get; set; }

        public IEnumerable<SelectListItem> Rooms { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Inventories { get; set; }
      
        public IEnumerable<SelectListItem> Suppliers { get; set; }


        public IEnumerable<SelectListItem> Items { get; set; }
        public IEnumerable<SelectListItem> CatMain { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; }
        public IEnumerable<SelectListItem> SubCategory { get; set; }

        public IEnumerable<SelectListItem> HandOverReasons { get; set; }

        public IEnumerable<SelectListItem> InventoriesTo { get; set; }
        public IEnumerable<SelectListItem> Buildings { get; set; }
        public IEnumerable<SelectListItem> Floors { get; set; }

        public IEnumerable<SelectListItem> RoomTypes { get; set; }

        public int [] members { get; set; }
        //public SelectList memberList { get; set; } = Load_Members()
        public long[] Inventors { get; set; }
        public IEnumerable<SelectListItem> vWVendors { get; set; }
        public IEnumerable<SelectListItem> VwContracts { get; set; }

        public IEnumerable<SelectListItem> PrinterNames { get; set; }
        public int PrinterId { get; set; }
        public string PrinterName { get; set; }
        public int NofPapers  { get; set; }
        public string OrderDate { get; set; }
        public string OrderNo { get; set; }
        public string ItemName { get; set; }
        public string ItemBarcode { get; set; }
        public string ItemPrice { get; set; }
        public string ZplCode { get; set; }
        public string ZplQRCode { get; set; }
        public long? RoomId { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public long? StoreId { get; set; }

        public long? ItemId { get; set; }
        public long? Cat_Id { get; set; }
        public long? CatSub_Id { get; set; }
        public int? MainCatId { get; set; }

        public int? DeptId { get; set; }
        public int? EmpId { get; set; }

        public int? supplierId { get; set; }
        
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        
        public int ? ReasonId { get; set; }
        public long? StoreToId { get; set; }
        public string Qty { get; set; }


        public long? VendorId { get; set; }
        public int? ContractId { get; set; }
        public int? RoomTypeId { get; set; }
    }
}