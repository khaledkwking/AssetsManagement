using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class ItemsStructureViewModel
    {

        private UnitOfWork unitWork = new UnitOfWork();

        //public PagedList.IPagedList<Category_tbl> Categories { get; set; }
        public string CatMain_Id { get; set; }
        public string Cat_Id { get; set; }
        public string CatSub_Id { get; set; }
        public string Item_Id { get; set; }
        public SelectList CatMain_tbl { get; set; }
        public SelectList Category_tbl { get; set; }
        public SelectList CatSub_tbl { get; set; }
        public SelectList Item_tbl { get; set; }

        public string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public int? PageNumber { get; set; }
        public int? PageCount { get; set; }

        public string CatMain_Name { get; set; }
        public string Cat_Name { get; set; }
        public string CatSub_Name{ get; set; }
       
        public  void ItemsPopulateList(int? defaultCatMainId )
        {
            //get all Main Categories
            var allCatMainList = unitWork.CatMainManager.GetNotDelAll().OrderByDescending(m => m.CatMain_Id).ToList();
            //get all Categories according to defaultCountryId
            var allCategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id == defaultCatMainId).ToList();

            var defaultCategoryId = allCategoryList.Select(m => m.Cat_Id).FirstOrDefault();

            //get all Categories according to defaultCategoryId
            var allSubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCategoryId).ToList();

            var defaultSubCategoryId = allSubCategoryList.Select(m => m.CatSub_Id).FirstOrDefault();

            //Get all Items according to defaultSubCategoryId
            var allItemslist = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCategoryId).ToList();
            //set defaultCityId 
            var defaultItemId = allItemslist.Select(m => m.Item_Id).FirstOrDefault();

            CatMain_tbl  = new SelectList(allCatMainList, "CatMain_Id", "CatMain_Name", defaultCatMainId);
            Category_tbl  = new SelectList(allCategoryList, "Cat_Id", "Cat_Name", defaultCategoryId);
            CatSub_tbl  = new SelectList(allSubCategoryList, "CatSub_Id", "CatSub_Name", defaultSubCategoryId);
            Item_tbl = new SelectList(allItemslist, "Item_Id", "Item_Name", defaultItemId);

            
        }

        public void setDropDrownList(string type, int value)
        {
            //ItemsStructureViewModel model = new ItemsStructureViewModel();
            switch (type)
            {
                case "CatMain_Id":
                    var CategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id == value).ToList();
                    Category_tbl = new SelectList(CategoryList, "Cat_Id", "Cat_Name");
                    var defaultCatId = CategoryList.Select(m => m.Cat_Id).FirstOrDefault();

                    var SubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCatId).ToList();
                    CatSub_tbl = new SelectList(SubCategoryList, "CatSub_Id", "CatSub_Name");
                    var defaultSubCatId = SubCategoryList.Select(m => m.CatSub_Id).FirstOrDefault();

                    var ItemsList = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId).ToList();
                    Item_tbl = new SelectList(ItemsList, "Item_Id", "Item_Name");
                    var defaultItemId = ItemsList.Select(m => m.Item_Id).FirstOrDefault();

                    break;
                case "Cat_Id":
                    var SubCategoryList1 = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == value).ToList();
                    CatSub_tbl = new SelectList(SubCategoryList1, "CatSub_Id", "CatSub_Name");
                    var defaultSubCatId1 = SubCategoryList1.Select(m => m.CatSub_Id).FirstOrDefault();

                    var ItemsList1 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId1).ToList();
                    Item_tbl = new SelectList(ItemsList1, "Item_Id", "Item_Name");
                    var defaultItemId1 = ItemsList1.Select(m => m.Item_Id).FirstOrDefault();

                    break;
                case "CatSub_Id":

                    var ItemsList2 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == value).ToList();
                    Item_tbl = new SelectList(ItemsList2, "Item_Id", "Item_Name");
                    //var defaultItemId1 = ItemsList1.Select(m => m.Item_Id).FirstOrDefault();

                    break;
            }
           
        }
        //public ItemsStructureViewModel ItemsPopulateList(ItemsStructureViewModel model)
        //{
        //    CatMain_tbl = new SelectList(unitWork.CatMainManager.GetNotDelAll().ToList(), "CountryId", "CountryName", model.CatMain_Id);
        //    Category_tbl = new SelectList(unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id.ToString() == model.CatMain_Id).ToList(), "Cat_Id", "Cat_Name", model.Cat_Id);
        //    CatSub_tbl = new SelectList(unitWork.CategoryManager.GetNotDelAll().Where(m => m.Cat_Id.ToString() == model.Cat_Id).ToList(), "CatSub_Id", "CatSub_Name", model.CatSub_Id);
        //    CatSub_tbl = new SelectList(unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id.ToString() == model.CatSub_Id).ToList(), "Item_Id", "Item_Name", model.Item_Id);
        //    return model;
        //}

    }
    
}