using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace BOL
{
    public class EmpRoomsManager : Repository<Emp_rooms>
    {

        public EmpRoomsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(Emp_rooms entity)
        {
            return base.Delete(entity);
        }

        public override Emp_rooms Add(Emp_rooms entity)
        {
            long RoomId = entity.RoomId;
            UnitOfWork UWork = new UnitOfWork();
            Room_tbl objRoom = UWork.RoomsManager.GetById(RoomId);
            List<Emp_rooms> EmpRooms = GetRoomOrEmpId(RoomId, 0);
            int DeptCount = EmpRooms.Count;
            int Quota = 0;
            if (objRoom != null)
            {
                if (objRoom.DeptQuota != null)
                {
                    Quota = objRoom.DeptQuota.GetValueOrDefault();
                }
            }
            if (Quota > 0 && DeptCount < Quota)
            {
                return base.Add(entity);
            }
            else
            {
                return null;
            }
        }
        public bool update(int id)
        {
            Emp_rooms st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Emp_rooms> GetRoomOrEmpId(long RoomId, int EmpId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.EmpId == EmpId || c.RoomId == RoomId && ( c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }

        public List<Emp_rooms> GetRoomByParam(int DeptId, long? RoomId, int? EmpId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.EmpId == EmpId && c.RoomId == RoomId && c.DeptId == DeptId).ToList();
        }
        public List<Emp_rooms> SearchByString(string Search_Data)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetAll().Where(c => c.Room_tbl.Room_Name.ToUpper().Contains(Search_Data.ToUpper())).ToList();

        }
        public List<Emp_rooms> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<Emp_rooms> GetNotDelAll(string RoomId = null)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<Emp_rooms> List;
            if (!String.IsNullOrEmpty(RoomId))
            {
                long CurRoomId = long.Parse(RoomId);
                List = GetNotDelAll().Where(c => c.RoomId == CurRoomId).ToList();
            }
            else
            {
                List = GetNotDelAll().ToList();
            }
            foreach (var Emproom in List)
            {


                vwEmployees CurList = new vwEmployees();
                vwDepartments CurDeptList = new vwDepartments();
                int EmpId = 0, DeptId = 0;
                if (Emproom.EmpId != null)
                {
                    EmpId = int.Parse(Emproom.EmpId.ToString());
                }
                if (Emproom.DeptId != null)
                {
                    DeptId = int.Parse(Emproom.DeptId.ToString());
                }

                CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                Emproom.VmEmployees = CurList;
                Emproom.VmDepartments = CurDeptList;

            }

            //var L=  List.Join( empList,
            //    room => room.EmpId,
            //    emp => emp.Id,
            //    (room, emp) => new { Emp = emp }
            //);
            //(from c in db.Contacts select c).ToList();
            return List;//.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }


    }
}