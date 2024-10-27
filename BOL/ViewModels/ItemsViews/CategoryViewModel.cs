using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class CategoryViewModel
    {
      
      
        public PagedList.IPagedList<Category_tbl> Categories { get; set; }
        public Category_tbl SelectedCategory { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        
    }
}