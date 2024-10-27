using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class UnitsViewModel
    {
      
      
        public PagedList.IPagedList<Unit_tbl> Units { get; set; }
        public Unit_tbl SelectedUnit { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        
    }
}