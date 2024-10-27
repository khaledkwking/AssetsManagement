using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class CatSubViewModel
    {
      
      
        public PagedList.IPagedList<CatSub_tbl> CatSub { get; set; }
        public CatSub_tbl SelectedCatSub { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        
    }
}