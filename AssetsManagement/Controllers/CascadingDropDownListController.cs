using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
namespace AssetsManagement.Controllers
{
    public class CascadingDropDownListController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        // GET: CascadingDropDownList
        public ActionResult ItemsPopulateList(int? defaultCatMainId = 1)
        {
         
            ItemsStructureViewModel model = new ItemsStructureViewModel();
            model.ItemsPopulateList(defaultCatMainId);
            //get all Main Categories
            //var allCatMainList = unitWork.CatMainManager.GetNotDelAll().OrderByDescending(m => m.CatMain_Id).ToList();
            ////get all Categories according to defaultCountryId
            //var allCategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id == defaultCatMainId).ToList();

            //var defaultCategoryId = allCategoryList.Select(m => m.Cat_Id).FirstOrDefault();

            ////get all Categories according to defaultCategoryId
            //var allSubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCategoryId).ToList();

            //var defaultSubCategoryId = allSubCategoryList.Select(m => m.CatSub_Id).FirstOrDefault();

            ////Get all Items according to defaultSubCategoryId
            //var allItemslist = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCategoryId).ToList();
            ////set defaultCityId 
            //var defaultItemId = allItemslist.Select(m => m.Item_Id).FirstOrDefault();

            //model.CatMain_tbl  = new SelectList(allCatMainList, "CatMain_Id", "CatMain_Name", defaultCatMainId);
            //model.Category_tbl  = new SelectList(allCategoryList, "Cat_Id", "Cat_Name", defaultCategoryId);
            //model.CatSub_tbl  = new SelectList(allSubCategoryList, "CatSub_Id", "CatSub_Name", defaultSubCategoryId);
            //model.Item_tbl = new SelectList(allItemslist, "Item_Id", "Item_Name", defaultItemId);

            //return PartialView(model, model.SelectedFloor);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            ItemsStructureViewModel model = new ItemsStructureViewModel();
            model.setDropDrownList(type, value);
            //switch (type)
            //{
            //    case "CatMain_Id":
            //        var CategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id == value).ToList();
            //        model.Category_tbl  = new SelectList(CategoryList, "Cat_Id", "Cat_Name");
            //        var defaultCatId = CategoryList.Select(m => m.Cat_Id).FirstOrDefault();

            //        var SubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCatId).ToList();
            //        model.CatSub_tbl = new SelectList(SubCategoryList, "CatSub_Id", "CatSub_Name");
            //        var defaultSubCatId = SubCategoryList.Select(m => m.CatSub_Id).FirstOrDefault();

            //        var ItemsList = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId).ToList();
            //        model.Item_tbl = new SelectList(ItemsList, "Item_Id", "Item_Name");
            //        var defaultItemId = ItemsList.Select(m => m.Item_Id).FirstOrDefault();

            //        break;
            //    case "Cat_Id":
            //        var SubCategoryList1 = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == value).ToList();
            //        model.CatSub_tbl = new SelectList(SubCategoryList1, "CatSub_Id", "CatSub_Name");
            //        var defaultSubCatId1 = SubCategoryList1.Select(m => m.CatSub_Id).FirstOrDefault();

            //        var ItemsList1 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId1).ToList();
            //        model.Item_tbl = new SelectList(ItemsList1, "Item_Id", "Item_Name");
            //        var defaultItemId1 = ItemsList1.Select(m => m.Item_Id).FirstOrDefault();

            //        break;
            //    case "CatSub_Id":

            //        var ItemsList2 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == value).ToList();
            //        model.Item_tbl = new SelectList(ItemsList2, "Item_Id", "Item_Name");
            //        //var defaultItemId1 = ItemsList1.Select(m => m.Item_Id).FirstOrDefault();

            //        break;
            //}
            return Json(model);
        }
        //[HttpPost]
        //public ActionResult ItemsPopulateList(ItemsStructureViewModel model)
        //{
        //    model.ItemsPopulateList(model);
        //    return View(model); 
        //}

        public ActionResult ItemsListModal(string id)
        {

            ItemsStructureViewModel model = new ItemsStructureViewModel();
            //model.ItemsPopulateList(defaultCatMainId);

            var allItemslist = unitWork.ItemsManager.GetNotDelAll().ToList();
            //set defaultCityId 
            var defaultItemId = id;// allItemslist.Select(m => m.Item_Id).FirstOrDefault();
            model.Item_tbl = new SelectList(allItemslist, "Item_Id", "Item_Name", defaultItemId);

            int ItemId = int.Parse (defaultItemId.ToString());
            Item_tbl ItemsList = unitWork.ItemsManager.GetById(ItemId);
            model.CatSub_Name = ItemsList.CatSub_tbl.CatSub_Name;

            model.CatMain_Name = ItemsList.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Name ;
            model.Cat_Name = ItemsList.CatSub_tbl.Category_tbl.Cat_Name ;

             return PartialView(model);
        }

        [HttpPost]
        public JsonResult SetItemCats(string type, int value)
        {
            ItemsStructureViewModel model = new ItemsStructureViewModel();
            switch (type)
            { 
             case "ddlItems":
                    int ItemId = value;
                    Item_tbl ItemsList = unitWork.ItemsManager.GetById (ItemId);
                    model.CatSub_Name = ItemsList.CatSub_tbl.CatSub_Name;

                    model.CatMain_Name = ItemsList.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Name;
                    model.Cat_Name = ItemsList.CatSub_tbl.Category_tbl.Cat_Name;

                    break;
            }
            //model.setDropDrownList(type, value);
     
            return Json(model);
        }
       
    }
}