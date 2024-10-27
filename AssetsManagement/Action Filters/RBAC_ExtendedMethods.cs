using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
//Get requesting user's roles/permissions from database tables...      
public static class RBAC_ExtendedMethods
{
    
    //public static bool HasRole(this ControllerBase controller, string role)
    //{
    //    bool bFound = false;
    //    try
    //    {
    //        //Check if the requesting user has the specified role...
    //        bFound = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRole(role);            
    //    }
    //    catch { }
    //    return bFound;
    //}

    //public static bool HasRoles(this ControllerBase controller, string roles)
    //{
    //    bool bFound = false;
    //    try
    //    {
    //        //Check if the requesting user has any of the specified roles...
    //        //Make sure you separate the roles using ; (ie "Sales Manager;Sales Operator"
    //        bFound = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRoles(roles);
    //    }
    //    catch { }
    //    return bFound;
    //}

    public static tbUsersPages GetUserPermission(this ControllerBase controller, string AllowFeature)
    {
       tbUsersPages UserPages=null;
        try
        {
             UnitOfWork unitWork = new UnitOfWork();
             int userId = SesssionUser.GetCurrentUserId();
            //Create an instance of our custom user authorization object passing requesting user's 'Windows Username' into constructor

           
            //Check if the requesting user has the permission to run the controller's action
         
            UserPages = unitWork.UsersManager.UserPermission(userId, AllowFeature); // is not system admin
                        

        }
        catch { }
        return UserPages;
    }

    public static bool HasPermission(this ControllerBase controller, string AllowFeature, string AllowPermission)
    {
        bool bFound = false;
        try
        {
            UnitOfWork unitWork = new UnitOfWork();
            int userId = SesssionUser.GetCurrentUserId();
            //Create an instance of our custom user authorization object passing requesting user's 'Windows Username' into constructor
            tbUsers requestingUser = unitWork.UsersManager.GetById(userId);
            //RBACUser requestingUser = new RBACUser(filterContext.RequestContext.HttpContext.User.Identity.Name);
            //Check if the requesting user has the permission to run the controller's action
            if (requestingUser != null)
            {
                if (!unitWork.UsersManager.HasPermission(AllowFeature, AllowPermission, userId) && requestingUser.RoleID != BOL.DataModel.AdminRoleId ) // is not system admin
                {
                    //Check if the requesting user has the specified application permission...
                    bFound = false;
                }
                else
                {
                    bFound = true;
                }
            }
            else
            {

            }

        }
        catch { }
        return bFound;
    }

    //public static bool IsSysAdmin(this ControllerBase controller)
    //{        
    //    bool bIsSysAdmin = false;
    //    try
    //    {
    //        //Check if the requesting user has the System Administrator privilege...
    //        bIsSysAdmin = new RBACUser(controller.ControllerContext.HttpContext.User.Identity.Name).IsSysAdmin;
    //    }
    //    catch { }
    //    return bIsSysAdmin;
    //}
}
