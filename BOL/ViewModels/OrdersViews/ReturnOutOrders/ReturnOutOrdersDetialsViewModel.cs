using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class ReturnOutOrdersDetialsViewModel
    {
   
        public PagedList.IPagedList<ReturnOutOrdersDetails> ReturnOutOrdersDetails { get; set; }
        public ReturnOutOrdersDetails SelectedItem { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
          
    }
}