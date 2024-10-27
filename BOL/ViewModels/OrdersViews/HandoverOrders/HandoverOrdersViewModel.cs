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
    public class HandoverOrdersViewModel
    {
        public HandoverOrdersViewModel()
        {
            ItemsScanCheckList = new List<CheckBoxListItem>();
            HandOverOrdersDetails = new List <HandOverOrdersDetails>();
        }
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        public PagedList.IPagedList<HandoverOrders> HandoverOrders { get; set; }
        public List <HandOverOrdersDetails> HandOverOrdersDetails { get; set; }
        public List <tbl_ItemsStock> ScanItems { get; set; }
       
        public HandoverOrders SelectedItem { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }

       
        [DefaultValue(1)]
        public int PostFlag { get; set; }
        public int searchType { get; set; }

        [DefaultValue(1)]
        public int ReaderType { get; set; }
        public string Barcode { get; set; }
       

        //public IEnumerable<SelectListItem> FromRooms { get; set; }
        //public IEnumerable<SelectListItem> FromEmployees { get; set; }
        //public IEnumerable<SelectListItem> FromDepartments { get; set; }
        public IEnumerable<SelectListItem> Inventories { get; set; }
        public IEnumerable<SelectListItem> HandOverReasons { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; }
        public List<CheckBoxListItem> ItemsScanCheckList { get; set; }
        public long? FromRoomId { get; set; }
        public int FromDeptId { get; set; }
        public int? FromEmpId { get; set; }
        public string IPAddress { get; set; }
        public bool TcpFlag { get; set; }
       
    }
}