using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class VendorContractsViewModel
    {
      
      
        public PagedList.IPagedList<Vendor_Contracts> VendorContracts { get; set; }
        public Vendor_Contracts SelectedContracts { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        //public SelectList VendorsList { get; set; }
        public IEnumerable<SelectListItem> VendorsList { get; set; }
        public string CurVendorId { get; set; }

    }
}