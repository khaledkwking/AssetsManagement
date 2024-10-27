using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using System.IO;
using System.Configuration;

namespace AssetsManagement.Controllers
{
    //[Authorize]
    public class ItemsController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        // Start module Unit of Items -----------------------------------------------------------------------------------------------------


        [UserPermissionAttribute(AllowFeature = "Units", AllowPermission = "Accessing")]
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            UnitsViewModel model = new UnitsViewModel();
            List<Unit_tbl> UnitList = unitWork.UnitsManager.GetNotDelAll().OrderBy(m => m.Unit_Id).ToList();
            model.SelectedUnit = null;
            //List<Unit_tbl> UnitList = unitWork.UnitsManager.GetAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Unit_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Unit_Id" ? "Unit_Name" : "Unit_NameEn";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Units select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Units.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Units.Find()
                UnitList = unitWork.UnitsManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "Unit_Id":
                    UnitList = UnitList.OrderBy(stu => stu.Unit_Id ).ToList();
                    break;
                case "Unit_Name":
                    UnitList = UnitList.OrderBy(stu => stu.Unit_Name ).ToList();
                    break;
                case "Unit_NameEn":
                    UnitList = UnitList.OrderBy(stu => stu.Unit_NameEn).ToList();
                    break;
                default:
                    UnitList = UnitList.OrderBy(stu => stu.Unit_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Units = UnitList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UnitList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
               
        //[HttpPost]
        public ActionResult New()
        {

            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).ToPagedList(No_Of_Page, Size_Of_Page);

            model.SelectedUnit = null;
            model.DisplayMode = "WriteOnly";
            return PartialView("EditUnitModal", model.SelectedUnit);
            // return View("Index", model);

        }
        [HttpPost]
        public ActionResult Insert(Unit_tbl obj)
        {
            UnitsViewModel model = new UnitsViewModel();
            unitWork.UnitsManager.Add(obj);
            unitWork.UnitsManager.Update(obj);

            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.SelectedUnit = unitWork.UnitsManager.GetById(obj.Unit_Id);  //officeList..GetById((obj.Unit_Id);
            model.DisplayMode = "ReadOnly";
            return View("Index", model);

        }
        [HttpPost]
        public ActionResult Select(string id)
        {

            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            int Unit_Id = int.Parse(id);
            model.SelectedUnit = unitWork.UnitsManager.GetById(Unit_Id);
            model.DisplayMode = "ReadOnly";
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult Edit(string id)
        {
            int Unit_Id = int.Parse(id);
            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.SelectedUnit = unitWork.UnitsManager.GetById(Unit_Id);
            model.DisplayMode = "ReadWrite";
            return View("Index", model);

        }
        [HttpPost]
        public ActionResult Update(Unit_tbl obj)
        {

            Unit_tbl existing = unitWork.UnitsManager.GetById(obj.Unit_Id);
            existing.Unit_Id = obj.Unit_Id;
            existing.Unit_Name  = obj.Unit_Name;
            existing.Unit_NameEn  = obj.Unit_NameEn;
           
            unitWork.UnitsManager.Update(existing);

            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);

            model.SelectedUnit = existing;
            model.DisplayMode = "ReadOnly";
            return View("Index", model);

        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            int Unit_Id = int.Parse(id);
            Unit_tbl existing = unitWork.UnitsManager.GetById(Unit_Id);
            if (existing != null)
            {
                unitWork.UnitsManager.Delete(existing);
                //unitWork.UnitsManager.Update(existing);

                UnitsViewModel model = new UnitsViewModel();
                model.Units = unitWork.UnitsManager.GetNotDelAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedUnit = null;
                model.DisplayMode = "";
                return View("Index", model);
            }
            else
            {
                return RedirectToAction("Index");
              
            }
        }
        [HttpPost]
        public ActionResult Cancel(string id)
        {
            int Unit_Id = int.Parse(id);
            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;
            model.SelectedUnit = unitWork.UnitsManager.GetById(Unit_Id);
            model.DisplayMode = "ReadOnly";
            return View("Index", model);
        }

        [Route("UpdateAndAdd")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateAndAdd(Unit_tbl Unit)
        {
            Unit_tbl existing= Unit;
            if (ModelState.IsValid)
            {
               
                if ( Unit.Unit_Id == 0)
                {
                    // insert new record
                    Unit_tbl Unitobj = unitWork.UnitsManager.Add(existing);
                    if (Unitobj != null)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                    ModelState.Clear();
                }
                else
                {
                    existing = unitWork.UnitsManager.GetById(Unit.Unit_Id);
                    existing.Unit_Id = Unit.Unit_Id;
                    existing.Unit_Name = Unit.Unit_Name;
                    existing.Unit_NameEn = Unit.Unit_NameEn;

                    bool ret= unitWork.UnitsManager.Update(existing);
                    // update record
                   
                    if (ret)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                }

            }
           
          
            UnitsViewModel model = new UnitsViewModel();
            model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);

            model.SelectedUnit = existing;
            model.DisplayMode = "ReadOnly";
            return View("Index", model);
            //Car Cars = unitWork.CarManager.GetAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);
           
        }


        [HttpGet]
        [ActionName("AddEditUnit")]
        public ActionResult AddEditUnit(string id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            UnitsViewModel model = new UnitsViewModel();
            if (String.IsNullOrEmpty(id))
            {
                if (this.HasPermission("Units", "Adding"))
                {
                    Unit_tbl UnitRecord = new Unit_tbl();
                    model.SelectedUnit = UnitRecord;
                    model.DisplayMode = "ReadOnly";
                    return PartialView("EditAddModal", model.SelectedUnit);
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }
            }
            else
            {
                if (this.HasPermission("Units", "Updating"))
                {
                    // Edit record (view in Edit mode)
                    int ModelId = int.Parse(id);

                    model.Units = unitWork.UnitsManager.GetAll().OrderBy(m => m.Unit_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                    model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                    model.DisplayMode = "ReadWrite";
                    if (model.SelectedUnit == null) { return View("_error"); }
                    // ...
                    return PartialView("EditAddModal", model.SelectedUnit);
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }
            }

        }
        //-----------------------------------------------------------------------------------------End of units---------------------------------------

        // --------------------------------------- Start of Main Category-------------------------------------------------------------------------------

        // Start of Main Items module
        [UserPermissionAttribute(AllowFeature = "Main Categories", AllowPermission = "Accessing")]
        public ActionResult CatMainList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            CatMainViewModel model = new CatMainViewModel();
            List<CatMain_tbl> CatMainList = unitWork.CatMainManager.GetNotDelAll().OrderBy(m => m.CatMain_Id).ToList();
            model.SelectedCatMain = null;
        
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "CatMain_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "CatMain_Id" ? "CatMain_Name" : "CatMain_NameEn";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
        
            if (!String.IsNullOrEmpty(Search_Data))
            {

                CatMainList = unitWork.CatMainManager.GetCastByUnitName(Search_Data);
              
            }
            switch (Sorting_Order)
            {
                case "ID":
                    CatMainList = CatMainList.OrderBy(stu => stu.CatMain_Id ).ToList();
                    break;
                case "TitleAr":
                    CatMainList = CatMainList.OrderBy(stu => stu.CatMain_Name).ToList();
                    break;
                case "Title":
                    CatMainList = CatMainList.OrderBy(stu => stu.CatMain_NameEn).ToList();
                    break;
                default:
                    CatMainList = CatMainList.OrderBy(stu => stu.CatMain_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.CatMain = CatMainList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (CatMainList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteCatMain")]
        public ActionResult DeleteCatMain(string id)
        {
            int Id = int.Parse(id);
            CatMain_tbl existing = unitWork.CatMainManager.GetById(Id);
            if (existing != null)
            {
                unitWork.CatMainManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                CatMainViewModel model = new CatMainViewModel();
                model.CatMain = unitWork.CatMainManager.GetNotDelAll().OrderBy(m => m.CatMain_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedCatMain = null;
                model.DisplayMode = "";

                return RedirectToAction("CatMainList", new { BuildingId = existing.CatMain_Id });
            }
            else
            {
                return RedirectToAction("CatMainList");
            }
        }


        [HttpGet]
        [ActionName("AddEditCatMain")]
        public ActionResult AddEditCatMain(string id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetNotDelAll();
            //var CurStatusList = unitWork.StatusManager.GetNotDelAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            CatMainViewModel model = new CatMainViewModel();

            if (String.IsNullOrEmpty(id))
            {
                CatMain_tbl FloorRecord = new CatMain_tbl();
                model.SelectedCatMain = FloorRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddCatMainModal", model.SelectedCatMain);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.CatMain = unitWork.CatMainManager.GetNotDelAll().OrderBy(m => m.CatMain_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCatMain = unitWork.CatMainManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedCatMain == null) { return View("_error"); }
                // ...
                return PartialView("EditAddCatMainModal", model.SelectedCatMain);
            }

        }

        [Route("SaveCatMain")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveCatMain(CatMain_tbl CatMain)
        {
            CatMain_tbl existing = CatMain;
            try
            {
                if (ModelState.IsValid)
                {

                    if (CatMain.CatMain_Id == 0)
                    {
                        // insert new record

                        CatMain_tbl CatMainobj = unitWork.CatMainManager.Add(existing);
                        if (CatMainobj != null)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.CatMainManager.GetById(CatMain.CatMain_Id);

                        existing.CatMain_Name = CatMain.CatMain_Name;
                        existing.CatMain_NameEn = CatMain.CatMain_NameEn;
                        bool ret = unitWork.CatMainManager.Update(existing);
                        // update record

                        if (ret)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                    }

                }

                CatMainViewModel model = new CatMainViewModel();
                model.CatMain = unitWork.CatMainManager.GetNotDelAll().OrderBy(m => m.CatMain_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCatMain = existing;
                model.DisplayMode = "ReadOnly";
                //return View("FloorsList", model,   );
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("CatMainList");
        

        }
        // End of Main Category Module------------------------------------------------------------------------------


        // Start of Category module
        [UserPermissionAttribute(AllowFeature = "Sub Categories", AllowPermission = "Accessing")]
        public ActionResult CategoryList(string MainCatId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            if (String.IsNullOrEmpty(MainCatId))
            {
                return View();
            }

            ViewBag.MainCatId = MainCatId.ToString();
            //List<Car> carList = null;
            int Id = int.Parse(MainCatId);
            CatMain_tbl ObjMainCat = unitWork.CatMainManager.GetById(Id);
            if (ObjMainCat != null)
            {
                if (ObjMainCat != null)
                    ViewBag.MainCatName = ObjMainCat.CatMain_Name;
            }

            CategoryViewModel model = new CategoryViewModel();
            List<Category_tbl> CategoryList = unitWork.CategoryManager.GetNotDelAll().OrderBy(m => m.Cat_Id).ToList();

            CategoryList = CategoryList.Where(r => r.CatMain_Id == Id).ToList(); //.FirstOrDefault();
            //var flrs = from f in FloorsList orderby f.Building_Id, f.Floor_Id select f;
            //if (flrs != null)
            //{
            //    if (flrs.Count() >= 0)
            //    {
            //        _floors = flrs.ToList();
            //    }
            //}
            model.Categories = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Cat_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "CatSub_Id" ? "Cat_Name" : "Cat_NameEn";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
                CategoryList = unitWork.CategoryManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ID":
                    CategoryList = CategoryList.OrderBy(stu => stu.Cat_Id).ToList();
                    break;
                case "TitleAr":
                    CategoryList = CategoryList.OrderBy(stu => stu.Cat_Name).ToList();
                    break;
                case "Title":
                    CategoryList = CategoryList.OrderBy(stu => stu.Cat_NameEn).ToList();
                    break;
                default:
                    CategoryList = CategoryList.OrderBy(stu => stu.Cat_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.Page_No= No_Of_Page;
            model.Categories = CategoryList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (CategoryList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteCategory")]
        public ActionResult DeleteCategory(string id)
        {
            int Id = int.Parse(id);
            Category_tbl existing = unitWork.CategoryManager.GetById(Id);
            if (existing != null)
            {
                unitWork.CategoryManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                CategoryViewModel model = new CategoryViewModel();
                model.Categories = unitWork.CategoryManager.GetNotDelAll().OrderBy(m => m.Cat_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedCategory = null;
                model.DisplayMode = "";

                return RedirectToAction("CategoryList", new { MainCatId = existing.CatMain_Id });
            }
            else
            {
                return RedirectToAction("CategoryList");
            }
        }

        
        [HttpGet]
        [ActionName("AddEditCategory")]
        public ActionResult AddEditCategory(string id = null, string MainCatId = null)
        {

            CategoryViewModel model = new CategoryViewModel();

            if (String.IsNullOrEmpty(id))
            {
                Category_tbl CategoryRecord = new Category_tbl();
                if (MainCatId != null)
                {
                    CategoryRecord.CatMain_Id = int.Parse(MainCatId);
                }
                model.SelectedCategory = CategoryRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddCategoryModal", model.SelectedCategory);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Categories = unitWork.CategoryManager.GetNotDelAll().OrderBy(m => m.Cat_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCategory = unitWork.CategoryManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedCategory == null) { return View("_error"); }
                // ...
                return PartialView("EditAddCategoryModal", model.SelectedCategory);
            }

        }

        [Route("SaveCategory")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveCategory(Category_tbl Category)
        {
            Category_tbl existing = Category;
            try
            {
                if (ModelState.IsValid)
                {

                    if (Category.Cat_Id == 0)
                    {
                        // insert new record

                        Category_tbl Categoryobj = unitWork.CategoryManager.Add(existing);
                        if (Categoryobj != null)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.CategoryManager.GetById(Category.Cat_Id);
                        existing.CatMain_Id = Category.CatMain_Id;
                        existing.Cat_Id = Category.Cat_Id;
                        existing.Cat_Name = Category.Cat_Name;
                        existing.Cat_NameEn = Category.Cat_NameEn;

                        bool ret = unitWork.CategoryManager.Update(existing);
                        // update record

                        if (ret)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                    }

                }


                CategoryViewModel model = new CategoryViewModel();
                model.Categories = unitWork.CategoryManager.GetNotDelAll().OrderBy(m => m.Cat_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCategory = existing;
                model.DisplayMode = "ReadOnly";
                //return View("FloorsList", model,   );
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("CategoryList", new { MainCatId = existing.CatMain_Id });


        }
        // End of Sub category Module




        // Start of SubCategory module
        [UserPermissionAttribute(AllowFeature = "Categories", AllowPermission = "Accessing")]
        public ActionResult SubCatList(string MainCatId, string CategoryId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            if (String.IsNullOrEmpty(CategoryId))
            {
                return View();
            }

            ViewBag.CategoryId = CategoryId.ToString();
          
            ViewBag.MainCatId = MainCatId;

            //List<Car> carList = null;
            int Id = int.Parse(CategoryId);
            int CurMainCatId = int.Parse(MainCatId);
            CatMain_tbl ObjCatMain = unitWork.CatMainManager.GetById(CurMainCatId);
            if (ObjCatMain != null)
            {
                if (ObjCatMain != null)
                    ViewBag.CatMainName = ObjCatMain.CatMain_Name;
            }

            Category_tbl ObjCategory = unitWork.CategoryManager.GetById(Id);
            if (ObjCategory != null)
            {
                if (ObjCategory != null)
                    ViewBag.CatName = ObjCategory.Cat_Name;
            }

            CatSubViewModel model = new CatSubViewModel();
            List<CatSub_tbl> CatSubList = unitWork.CatSubManager.GetNotDelAll().OrderBy(m => m.CatSub_Id).ToList();

            CatSubList = CatSubList.Where(r => r.Cat_Id == Id).ToList(); //.FirstOrDefault();
           
            model.CatSub = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "CatSub_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "CatSub_Id" ? "CatSub_Name" : "CatSub_NameEn";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
                CatSubList = unitWork.CatSubManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ID":
                    CatSubList = CatSubList.OrderBy(stu => stu.CatSub_Id ).ToList();
                    break;
                case "TitleAr":
                    CatSubList = CatSubList.OrderBy(stu => stu.CatSub_Name).ToList();
                    break;
                case "Title":
                    CatSubList = CatSubList.OrderBy(stu => stu.CatSub_NameEn).ToList();
                    break;
                default:
                    CatSubList = CatSubList.OrderBy (stu => stu.CatSub_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.CatSub = CatSubList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (CatSubList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteSubCat")]
        public ActionResult DeleteSubCat(string id)
        {
            int Id = int.Parse(id);
            CatSub_tbl existing = unitWork.CatSubManager.GetById(Id);
            if (existing != null)
            {
                unitWork.CatSubManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                CatSubViewModel model = new CatSubViewModel();
                model.CatSub = unitWork.CatSubManager.GetNotDelAll().OrderBy(m => m.CatSub_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedCatSub = null;
                model.DisplayMode = "";

                Category_tbl Categoryobj = unitWork.CategoryManager.GetById(existing.Cat_Id);
                string CatMain_Id="0" ;
                if (Categoryobj != null)
                {
                    CatMain_Id = Categoryobj.CatMain_Id.ToString () ;
                }

                return RedirectToAction("SubCatList", new { MainCatId = CatMain_Id,  CategoryId= existing.Cat_Id });
            }
            else
            {
                return RedirectToAction("SubCatList");
            }
        }


        [HttpGet]
        [ActionName("AddEditSubCat")]
       
        public ActionResult AddEditSubCat(string id = null, string CatId=null, string MainCatId = null)
        {
           
            CatSubViewModel model = new CatSubViewModel();

            if (String.IsNullOrEmpty(id))
            {
                CatSub_tbl SubCatRecord = new CatSub_tbl();
                if (CatId != null)
                {
                    SubCatRecord.Cat_Id = int.Parse(CatId);
                }
                model.SelectedCatSub = SubCatRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddSubCatModal", model.SelectedCatSub);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.CatSub  = unitWork.CatSubManager .GetNotDelAll().OrderBy(m => m.CatSub_Id ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCatSub = unitWork.CatSubManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedCatSub == null) { return View("_error"); }
                // ...
                return PartialView("EditAddSubCatModal", model.SelectedCatSub);
            }

        }

        [Route("SaveSubCat")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveSubCat(CatSub_tbl CatSub)
        {
            CatSub_tbl  existing = CatSub;
            string CatMain_Id = "0";
            try
            {
                if (ModelState.IsValid)
                {

                    if (CatSub.CatSub_Id == 0)
                    {
                        // insert new record

                        CatSub_tbl CatSubobj = unitWork.CatSubManager.Add(existing);
                        if (CatSubobj != null)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.CatSubManager.GetById(CatSub.CatSub_Id);
                        existing.Cat_Id = CatSub.Cat_Id;
                        existing.CatSub_Id = CatSub.CatSub_Id;
                        existing.CatSub_Name = CatSub.CatSub_Name;
                        existing.CatSub_NameEn = CatSub.CatSub_NameEn;
                        existing.CatSub_Notes = CatSub.CatSub_Notes;
                        existing.GenerateBarcodeFlag  = CatSub.GenerateBarcodeFlag;
                        bool ret = unitWork.CatSubManager.Update(existing);
                        // update record

                        if (ret)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                    }

                }

                Category_tbl Categoryobj = unitWork.CategoryManager.GetById(existing.Cat_Id);
               
                if (Categoryobj != null)
                {
                    CatMain_Id = Categoryobj.CatMain_Id.ToString();
                }

                CatSubViewModel model = new CatSubViewModel();
                model.CatSub = unitWork.CatSubManager.GetNotDelAll().OrderBy(m => m.CatSub_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedCatSub = existing;
                model.DisplayMode = "ReadOnly";
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            //return View("FloorsList", model,   );
            return RedirectToAction("SubCatList", new { MainCatId = CatMain_Id, CategoryId = existing.Cat_Id });
         
        }
        // End of Sub category Module



        // Start of SubCategory module
        [UserPermissionAttribute(AllowFeature = "Items", AllowPermission = "Accessing")]
        public ActionResult ItemsList(string MainCatId, string CategoryId,string SubCatId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            if (String.IsNullOrEmpty(SubCatId))
            {
                return View();
            }

            ViewBag.SubCatId = SubCatId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.MainCatId = MainCatId;

            //List<Car> carList = null;
            int Id = int.Parse(SubCatId);
            int CurMainCatId = int.Parse(MainCatId);
            int CurCategoryId= int.Parse(CategoryId);

            CatMain_tbl ObjCatMain = unitWork.CatMainManager.GetById(CurMainCatId);
            if (ObjCatMain != null)
            {
                if (ObjCatMain != null)
                    ViewBag.CatMainName = ObjCatMain.CatMain_Name;
            }

            Category_tbl ObjCategory = unitWork.CategoryManager.GetById(CurCategoryId);
            if (ObjCategory != null)
            {
                if (ObjCategory != null)
                    ViewBag.CatName = ObjCategory.Cat_Name;
            }

            CatSub_tbl ObjSubCat= unitWork.CatSubManager.GetById(Id);
            if (ObjSubCat != null)
            {
                if (ObjSubCat != null)
                    ViewBag.CatSub_Name = ObjSubCat.CatSub_Name;
            }

            ItemsViewModel model = new ItemsViewModel();
            List<Item_tbl> ItemsList = unitWork.ItemsManager.GetNotDelAll().OrderBy(m => m.Item_Id).ToList();

            ItemsList = ItemsList.Where(r => r.CatSub_Id == Id).ToList(); //.FirstOrDefault();

            model.Items = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Item_Name" : "Item_NameEn";
            
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
                ItemsList = unitWork.ItemsManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "Item_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                    break;
                case "Item_Name":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Name ).ToList();
                    break;
                case "Item_NameEn":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_NameEn ).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.PageNumber = No_Of_Page;
            ViewBag.PageNumber= No_Of_Page;

            model.Items = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (ItemsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteItem")]
        public ActionResult DeleteItem(string id)
        {
            int Id = int.Parse(id);
            Item_tbl existing = unitWork.ItemsManager.GetById(Id);
            if (existing != null)
            {
                unitWork.ItemsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                ItemsViewModel model = new ItemsViewModel();
                model.Items = unitWork.ItemsManager.GetNotDelAll().OrderBy(m => m.Item_Id ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedItem = null;
                model.DisplayMode = "";

                CatSub_tbl CatSubobj = unitWork.CatSubManager.GetById(existing.CatSub_Id);
                string CatMain_Id = "0";
                string Cat_Id = "0";
                if (CatSubobj != null)
                {
                    Cat_Id = CatSubobj.Cat_Id.ToString();
                    int CurCat_Id = int.Parse(Cat_Id);
                    Category_tbl Categoryobj = unitWork.CategoryManager.GetById(CurCat_Id);
                    if (Categoryobj != null)
                    {
                        CatMain_Id = Categoryobj.CatMain_Id.ToString ();
                     
                    }
                }
                
                return RedirectToAction("ItemsList", new { MainCatId = CatMain_Id, CategoryId = Cat_Id, SubCatId= existing.CatSub_Id });
            }
            else
            {
                return RedirectToAction("ItemsList");
            }
        }


        [HttpGet]
        [ActionName("AddEditItem")]
        public ActionResult AddEditItem(string id = null, string SubCatId = null, string PageNumber= null)
        {

            ItemsViewModel model = new ItemsViewModel();

            if (PageNumber != null)
            {
                No_Of_Page = int.Parse(PageNumber);
              
            }
            else
            {
                No_Of_Page = 1;
            }
            model.PageNumber = No_Of_Page;
           ViewBag.PageNumber = No_Of_Page;

            model.Units = unitWork.UnitsManager.GetAll().Select(option => new SelectListItem
            {
                Text = option.Unit_Name,
                Value = option.Unit_Id.ToString()
            });

            model.Status  = unitWork.StatusManager.GetAll().Select(option => new SelectListItem
            {
                Text = option.AName ,
                Value = option.Item_StateId.ToString()
            });


            if (String.IsNullOrEmpty(id))
            {
                Item_tbl ItemRecord = new Item_tbl();
                if (SubCatId != null)
                {
                    ItemRecord.CatSub_Id  = int.Parse(SubCatId);
                }
                model.SelectedItem = ItemRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddItemModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);
                
                model.Items = unitWork.ItemsManager.GetNotDelAll().OrderBy(m => m.Item_Id ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.ItemsManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                
                if (model.SelectedItem == null) { return View("_error"); }
                // ...
                return PartialView("EditAddItemModal", model);
            }

        }

        [Route("SaveItem")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveItem(ItemsViewModel ModelItem, HttpPostedFileBase postedFile)
        {
            Item_tbl existing = ModelItem.SelectedItem;
            string CatMain_Id = "0";
            string Cat_Id = "0";
            int? Page_No=1;
            try
            {
                //if (ModelState.IsValid)
                //{
                string Sorting_Order = ModelItem.Sorting_Order;
                string Search_Data = ModelItem.Search_Data;
                string Filter_Value = ModelItem.Filter_Value;
                Page_No = ModelItem.PageNumber;

                string NewFilename = "";
                if (postedFile != null)
                {
                    string CurfileName = Path.GetFileName(postedFile.FileName);
                    string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                    string FolderName = ConfigurationManager.AppSettings["ItemsHTMLPath"].ToString();
                    string path = Server.MapPath(FolderName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                    postedFile.SaveAs(path + NewFilename);
                    ViewBag.ImageUrl = ConfigurationManager.AppSettings["ItemsHTMLPath"].ToString() + NewFilename;

                }

                if (ModelItem.SelectedItem.Item_Id == 0)
                {
                    // insert new record
                    if (NewFilename != "")
                    {
                        existing.PictureName = NewFilename;
                    }
                    Item_tbl Itemobj = unitWork.ItemsManager.Add(existing);
                    if (Itemobj != null)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                    ModelState.Clear();
                }
                else
                {
                    existing = unitWork.ItemsManager.GetById(ModelItem.SelectedItem.Item_Id);
                    existing.CatSub_Id = ModelItem.SelectedItem.CatSub_Id;
                    existing.Item_Id = ModelItem.SelectedItem.Item_Id;
                    existing.Item_Name = ModelItem.SelectedItem.Item_Name;
                    existing.Item_NameEn = ModelItem.SelectedItem.Item_NameEn;
                    existing.Item_LowQty = ModelItem.SelectedItem.Item_LowQty;
                    existing.Item_StateId = ModelItem.SelectedItem.Item_StateId;
                    existing.Item_BarCode = ModelItem.SelectedItem.Item_BarCode;
                    existing.Item_RFID = ModelItem.SelectedItem.Item_RFID;

                    existing.Item_Dec = ModelItem.SelectedItem.Item_Dec;
                    existing.Item_Remarks = ModelItem.SelectedItem.Item_Remarks;
                    existing.Item_Count = ModelItem.SelectedItem.Item_Count;
                    existing.Unit_Id = ModelItem.SelectedItem.Unit_Id;
                    existing.Item_Price = ModelItem.SelectedItem.Item_Price;
                    existing.OrganizedFlag = ModelItem.SelectedItem.OrganizedFlag;
                    existing.CountableFlag = ModelItem.SelectedItem.CountableFlag;
                    existing.Age = ModelItem.SelectedItem.Age;
                    existing.OutFlag = ModelItem.SelectedItem.OutFlag;


                    if (NewFilename != "")
                    {
                        existing.PictureName = NewFilename;
                    }
                    bool ret = unitWork.ItemsManager.Update(existing);
                    // update record

                    if (ret)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                }

                //}

                CatSub_tbl CatSubobj = unitWork.CatSubManager.GetById(existing.CatSub_Id);
               
                if (CatSubobj != null)
                {
                    Cat_Id = CatSubobj.Cat_Id.ToString();
                    int CurCat_Id = int.Parse(Cat_Id);
                    Category_tbl Categoryobj = unitWork.CategoryManager.GetById(CurCat_Id);
                    if (Categoryobj != null)
                    {
                        CatMain_Id = Categoryobj.CatMain_Id.ToString();

                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("ItemsList", new { MainCatId = CatMain_Id, CategoryId = Cat_Id, SubCatId = existing.CatSub_Id, Page_No = Page_No });

        }
        // End of Sub category Module
    }
}