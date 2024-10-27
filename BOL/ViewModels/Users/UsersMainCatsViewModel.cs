using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class UsersMainCatsViewModel
    {
        public UsersMainCatsViewModel()
        {

            ItemsMainCatsCheckList = new List<CheckBoxListMainCatsPermissionItem>();
        }
        //public PagedList.IPagedList<tbUsersPages> UsersPages { get; set; }
        public List<tbUsersMainCats> UsersMainCats { get; set; }
        public tbUsersMainCats SelectedUsersMainCats { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public int? CurUserId { get; set; }
        public IEnumerable<SelectListItem> UsersPageslist { get; set; }
        public List<CheckBoxListMainCatsPermissionItem> ItemsMainCatsCheckList { get; set; }
       
    }
    public class CheckBoxListMainCatsPermissionItem
    {
        public CheckBoxListMainCatsPermissionItem()
        {
            Name = "";
            IsAccessSelected = false;
            IsAddSelected = false;
            IsUpdateSelected = false;
            IsDeleteSelected = false;
        }
        public int CatMain_Id { get; set; }
        public string Name { get; set; }
        public bool IsAccessSelected { get; set; }
        public bool IsAddSelected { get; set; }
        public bool IsUpdateSelected { get; set; }
        public bool IsDeleteSelected { get; set; }
    }
}