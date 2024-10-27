using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class RequestOutOrdersManager : Repository<RequestOutOrders>
    {

        public RequestOutOrdersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(RequestOutOrders entity)
        {
            return base.Delete(entity);
        }

        public RequestOutOrders GetByOrderId(long OrderId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            RequestOutOrders Item = GetById(OrderId);
          

            if (Item != null)
            {
                if (Item.EmpId != null)
                {
                    int EmpId = int.Parse(Item.EmpId.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    Item.VmEmployees = CurList;
                }
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                    Item.VmDepartments = CurDeptList;
                }
            }
            return Item;
        }

        public bool update(int id)
        {
            RequestOutOrders st = GetById(id);

            return Update(st);
        }
        public List<RequestOutOrders> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Notes.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<RequestOutOrders> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<RequestOutOrders> List;
            
            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            

            foreach (var Item in List)
            {
               
                if (Item.EmpId != null)
                {
                    int EmpId = int.Parse(Item.EmpId.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    Item.VmEmployees = CurList;
                }
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                    Item.VmDepartments = CurDeptList;
                }
            }
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<RequestOutOrders> GetNotDelAllByUserId(int userId, int? ReqStatusId, int? SecondReqStatusId, string UnitSearch=null)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            List<RequestOutOrders> List;

            List<vwDepartments> Depts = UWork.DepartmentManager.GetUserDepts(userId).ToList();
            var listOfDepts = Depts.Select(r => r.Id).ToList();


            List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && (c.ReqStatusId == ReqStatusId|| c.ReqStatusId == SecondReqStatusId || ReqStatusId == null)).ToList();

            List<RequestOutOrders> NewList = List.Where(c => listOfDepts.Contains(c.DeptId.GetValueOrDefault())).ToList();

            if (UnitSearch !=null)
            {
                NewList= NewList.Where(c => c.Notes.ToUpper().Contains(UnitSearch.ToUpper()) || c.VmDepartments.Name.ToUpper().Contains(UnitSearch.ToUpper())
                || c.ReqOrderId.ToString().ToUpper().Contains(UnitSearch.ToUpper())).ToList();
            }


            foreach (var Item in NewList)
            {

                //if (Item.EmpId != null)
                //{
                //    int EmpId = int.Parse(Item.EmpId.ToString());
                //    vwEmployees CurList = new vwEmployees();
                //    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                //    Item.VmEmployees = CurList;
                //}
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                    Item.VmDepartments = CurDeptList;
                }
            }
            return NewList;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}