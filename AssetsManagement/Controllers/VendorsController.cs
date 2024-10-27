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
    public class VendorsController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

     
        // Start of VendorsList module
        [UserPermissionAttribute(AllowFeature = "Vendors", AllowPermission = "Accessing")]
        public ActionResult VendorsList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            VendorsViewModel model = new VendorsViewModel();
            List<Vendors> VendorsList = unitWork.VendorsManager.GetNotDelAll().OrderBy(m => m.VendorId).ToList();
            model.SelectedVendor  = null;
        
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "CatMain_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "VendorId" ? "VendorName" : "VendorNameEn";

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

                VendorsList = unitWork.VendorsManager.GetCastByUnitName(Search_Data);
              
            }
            switch (Sorting_Order)
            {
                case "VendorId":
                    VendorsList = VendorsList.OrderBy(stu => stu.VendorId ).ToList();
                    break;
                case "VendorName":
                    VendorsList = VendorsList.OrderBy(stu => stu.VendorName).ToList();
                    break;
                case "VendorNameEn":
                    VendorsList = VendorsList.OrderBy(stu => stu.VendorNameEn).ToList();
                    break;
                default:
                    VendorsList = VendorsList.OrderBy(stu => stu.VendorId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Vendors = VendorsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (VendorsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteVendor")]
        public ActionResult DeleteVendor(string id)
        {
            int Id = int.Parse(id);
            Vendors existing = unitWork.VendorsManager.GetById(Id);
            if (existing != null)
            {
                unitWork.VendorsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                VendorsViewModel model = new VendorsViewModel();
                model.Vendors = unitWork.VendorsManager.GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedVendor = null;
                model.DisplayMode = "";

                return RedirectToAction("VendorsList", new { BuildingId = existing.VendorId });
            }
            else
            {
                return RedirectToAction("VendorsList");
            }
        }

        [HttpGet]
        [ActionName("AddEditVendor")]
        public ActionResult AddEditVendor(string id = null)
        {

            VendorsViewModel model = new VendorsViewModel();

            if (String.IsNullOrEmpty(id))
            {
                Vendors VendorRecord = new Vendors();
                model.SelectedVendor = VendorRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddVendorModal", model.SelectedVendor);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Vendors  = unitWork.VendorsManager.GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedVendor = unitWork.VendorsManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedVendor == null) { return View("_error"); }
                // ...
                return PartialView("EditAddVendorModal", model.SelectedVendor);
            }

        }

        [Route("SaveVendor")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveVendor(Vendors Vendor, HttpPostedFileBase postedFile)
        {
            Vendors existing = Vendor;
            try
            {
                string NewFilename = "";
                if (ModelState.IsValid)
                {
                    
                    if (postedFile != null)
                    {
                        string CurfileName = Path.GetFileName(postedFile.FileName);
                        string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                        string FolderName = "~" + ConfigurationManager.AppSettings["vendorsPath"].ToString();
                        string path = Server.MapPath(FolderName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                        postedFile.SaveAs(path + NewFilename);
                        if (NewFilename != "")
                        {

                            existing.PictureName = NewFilename;
                            
                        }

                    }


                    if (Vendor.VendorId == 0)
                    {
                        // insert new record

                        Vendors Vendorobj = unitWork.VendorsManager.Add(existing);
                        if (Vendorobj != null)
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
                        existing = unitWork.VendorsManager.GetById(Vendor.VendorId);

                        existing.VendorName = Vendor.VendorName;
                        existing.VendorNameEn = Vendor.VendorNameEn;
                        existing.Tel  = Vendor.Tel ;
                        existing.Address = Vendor.Address;
                        existing.Email  = Vendor.Email ;
                        existing.Remarks = Vendor.Remarks;
                        if (NewFilename != "")
                        {
                            existing.PictureName = NewFilename;
                        }
                        bool ret = unitWork.VendorsManager.Update(existing);
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

                VendorsViewModel model = new VendorsViewModel();
                model.Vendors = unitWork.VendorsManager.GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedVendor  = existing;
                model.DisplayMode = "ReadOnly";
                //return View("FloorsList", model,   );
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("VendorsList");
        

        }
        // End of Main Category Module------------------------------------------------------------------------------


        // Start of Vendor Contracts module
        [UserPermissionAttribute(AllowFeature = "Vendors", AllowPermission = "Accessing")]
        public ActionResult VendorContractsList(string VendorId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string ContractId)
        {

            //List<Car> carList = null;

            VendorContractsViewModel model = new VendorContractsViewModel();
            List<Vendor_Contracts> VendorsContractsList;
            if (!String.IsNullOrEmpty(VendorId))
            {
                ViewBag.VendorId = VendorId.ToString();
                long Id = int.Parse(VendorId);
                Vendors ObjVendor = unitWork.VendorsManager.GetById(Id);
                if (ObjVendor != null)
                {
                    if (ObjVendor != null)
                        ViewBag.VendorName = ObjVendor.VendorName;
                }
                VendorsContractsList = unitWork.VendorContractsManager.GetNotDelAllByVendorId(Id).OrderByDescending(m => m.VendorId).ToList();
                model.CurVendorId= VendorId;
                @ViewBag.VendorId = VendorId;
            }
            else if(!String.IsNullOrEmpty(ContractId))
            {
                int CurContractId = int.Parse(ContractId);
                VendorsContractsList = unitWork.VendorContractsManager.GetNotDelAllByContractIdId(CurContractId).OrderByDescending(m => m.VendorId).ToList();
              
                
            }
            else
            {
                @ViewBag.VendorId = "0";
                VendorsContractsList = unitWork.VendorContractsManager.GetNotDelAll().OrderByDescending(m => m.VendorId).ToList();
            }
            model.SelectedContracts = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "ContractId" : "";
            ViewBag.SortingModel = Sorting_Order == "ContractId" ? "ContractName" : "ContractNameEn";

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

                VendorsContractsList = unitWork.VendorContractsManager.GetCastByUnitName(Search_Data);

            }
            switch (Sorting_Order)
            {
                case "ContractId":
                    VendorsContractsList = VendorsContractsList.OrderBy(stu => stu.ContractId).ToList();
                    break;
                case "ContractName":
                    VendorsContractsList = VendorsContractsList.OrderBy(stu => stu.ContractName).ToList();
                    break;
                case "ContractNameEn":
                    VendorsContractsList = VendorsContractsList.OrderBy(stu => stu.ContractNameEn).ToList();
                    break;
                default:
                    VendorsContractsList = VendorsContractsList.OrderBy(stu => stu.VendorId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.Page_No = No_Of_Page;
            model.VendorContracts = VendorsContractsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (VendorsContractsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteContract")]
        public ActionResult DeleteContract(string id)
        {
            int Id = int.Parse(id);
            Vendor_Contracts existing = unitWork.VendorContractsManager.GetById(Id);
            if (existing != null)
            {
                unitWork.VendorContractsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                VendorContractsViewModel model = new VendorContractsViewModel();
                model.VendorContracts = unitWork.VendorContractsManager .GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedContracts = null;
                model.DisplayMode = "";

                return RedirectToAction("VendorContractsList", new { VendorId = existing.VendorId });
            }
            else
            {
                return RedirectToAction("VendorContractsList");
            }
        }

        [HttpGet]
        [ActionName("AddEditContract")]
        public ActionResult AddEditContract(string id = null, string VendorId = null)
        {

            VendorContractsViewModel model = new VendorContractsViewModel();
            model.VendorsList = unitWork.VendorsManager.GetNotDelAll().Select(option => new SelectListItem
            {
                Text = option.VendorName ,
                Value = option.VendorId.ToString()
            });

            if (String.IsNullOrEmpty(id))
            {
                Vendor_Contracts VendorRecord = new Vendor_Contracts();
                if (VendorId != null)
                {
                    VendorRecord.VendorId = int.Parse(VendorId);
                }
                model.SelectedContracts = VendorRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddContractModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.VendorContracts = unitWork.VendorContractsManager.GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedContracts = unitWork.VendorContractsManager.GetById(ModelId);
                model.CurVendorId = VendorId;
                ViewBag.VendorId= VendorId;  
                model.DisplayMode = "ReadWrite";
                if (model.SelectedContracts == null) { return View("_error"); }
                // ...
                return PartialView("EditAddContractModal", model);
            }

        }

        [Route("SaveContract")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveContract(VendorContractsViewModel Contract, HttpPostedFileBase postedFile)
        {
            Vendor_Contracts existing = Contract.SelectedContracts;
            try
            {
                string NewFilename = "";
                if (ModelState.IsValid)
                {
                    if (postedFile != null)
                    {
                        string CurfileName = Path.GetFileName(postedFile.FileName);
                        string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                        string FolderName = "~" + ConfigurationManager.AppSettings["vendorsPath"].ToString();
                        string path = Server.MapPath(FolderName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                        postedFile.SaveAs(path + NewFilename);
                        if (NewFilename != "")
                        {

                            existing.PictureName = NewFilename;

                        }

                    }
                    if (Contract.SelectedContracts.ContractId == 0)
                    {
                        // insert new record
                        Vendor_Contracts Vendorobj = unitWork.VendorContractsManager.Add(existing);
                        if (Vendorobj != null)
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
                        bool ret = false;
                        existing = unitWork.VendorContractsManager.GetById(Contract.SelectedContracts.ContractId);
                        if (existing != null)
                        {
                            existing.VendorId  = Contract.SelectedContracts.VendorId;
                            existing.ContractName = Contract.SelectedContracts.ContractName;
                            existing.ContractNameEn = Contract.SelectedContracts.ContractNameEn;
                            existing.ContractNo = Contract.SelectedContracts.ContractNo;
                            existing.FromDate = Contract.SelectedContracts.FromDate;
                            existing.ToDate = Contract.SelectedContracts.ToDate;
                            existing.Notes = Contract.SelectedContracts.Notes;
                            if (NewFilename != "")
                            {
                                existing.PictureName = NewFilename;
                            }
                            ret = unitWork.VendorContractsManager.Update(existing);
                        }
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

                VendorContractsViewModel model = new VendorContractsViewModel();
                model.VendorContracts = unitWork.VendorContractsManager.GetNotDelAll().OrderBy(m => m.VendorId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedContracts = existing;
                model.DisplayMode = "ReadOnly";
                //return View("FloorsList", model,   );
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            if (Contract.CurVendorId == "0" || Contract.CurVendorId==null)
            {
                return RedirectToAction("VendorContractsList");
            }
            else
            {
                return RedirectToAction("VendorContractsList", new { VendorId = existing.VendorId });
            }
        }
        // End of Main Category Module------------------------------------------------------------------------------

       


    }
}