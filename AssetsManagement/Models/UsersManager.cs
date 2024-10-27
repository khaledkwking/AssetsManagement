using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;

    public class UsersManager: Repository <emp> 
    {
     
        public UsersManager(CARACCOUNTWebEntities ctx):base (ctx)
        {
          
        }

        public override bool Delete(emp entity)
        {
            return base.Delete(entity);
        }
        public emp GetEmpByPassord(string UserName, string Password)
        {
            //a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault()

            return GetAll().Where(c => c.username.Equals(UserName) &&  c.pw.Equals(Password)).FirstOrDefault();

        }
    public bool update(int id, string Name, string address, int genderId)
        {
            emp st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
    }
