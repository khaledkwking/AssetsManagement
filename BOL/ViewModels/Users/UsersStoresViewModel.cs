using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class UsersStoresViewModel
    {
        public UsersStoresViewModel()
        {

            ItemsStoresCheckList = new List<CheckBoxListStoresPermissionItem>();
        }
        //public PagedList.IPagedList<tbUsersPages> UsersPages { get; set; }
        public List<tbUsersStores> UsersStores { get; set; }
        public tbUsersStores SelectedUsersStores { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public int? CurUserId { get; set; }
        public IEnumerable<SelectListItem> UsersPageslist { get; set; }
        public List<CheckBoxListStoresPermissionItem> ItemsStoresCheckList { get; set; }
       
    }
    public class CheckBoxListStoresPermissionItem
    {
        public CheckBoxListStoresPermissionItem()
        {
            Name = "";
            IsAccessSelected = false;
            IsAddSelected = false;
            IsUpdateSelected = false;
            IsDeleteSelected = false;
        }
        public long StoreId { get; set; }
        public string Name { get; set; }
        public bool IsAccessSelected { get; set; }
        public bool IsAddSelected { get; set; }
        public bool IsUpdateSelected { get; set; }
        public bool IsDeleteSelected { get; set; }
    }
}