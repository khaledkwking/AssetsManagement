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
    public class TransferOrdersViewModel
    {
        public TransferOrdersViewModel()
        {
            ItemsScanCheckList = new List<CheckBoxListItem>();
            TransferOrdersDetails = new List <TransferOrdersDetails>();
        }
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        public PagedList.IPagedList<TransferOrders> TransferOrders { get; set; }
        public List <TransferOrdersDetails> TransferOrdersDetails { get; set; }
        public List <tbl_ItemsStock> ScanItems { get; set; }
       
        public TransferOrders SelectedItem { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }

        public string EditMode { get; set; }

        [DefaultValue(1)]
        public int PostFlag { get; set; }
        public int searchType { get; set; }
        public string OrderType { get; set; }

        [DefaultValue(1)]
        public int ReaderType { get; set; }
        public string Barcode { get; set; }
       

        public IEnumerable<SelectListItem> FromStores { get; set; }
        public IEnumerable<SelectListItem> ToStores { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        public List<CheckBoxListItem> ItemsScanCheckList { get; set; }
        public long? FromStoreId { get; set; }
        public int ToStoreId { get; set; }
        public int? SupplierId { get; set; }
        public string IPAddress { get; set; }
        public bool TcpFlag { get; set; }
     

    }
}