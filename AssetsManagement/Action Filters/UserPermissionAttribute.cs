using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using System.Web.Routing;


    public class UserPermissionAttribute : FilterAttribute, IAuthorizationFilter
{
    public string AllowFeature { get; set; }
    public string AllowPermission { get; set; }

    private UnitOfWork unitWork = new UnitOfWork();
        public void  OnAuthorization(AuthorizationContext filterContext)
        {
            //Create permission string based on the requested controller name and action name in the format 'controllername-action'
        string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
        var filterAttribute = filterContext.ActionDescriptor.GetFilterAttributes(true)
                    .Where(a => a.GetType() == typeof(UserPermissionAttribute));
        foreach (UserPermissionAttribute attr in filterAttribute)
        {
            AllowFeature = attr.AllowFeature;
            AllowPermission = attr.AllowPermission;
        }
       
         int userId = SesssionUser.GetCurrentUserId();
            //Create an instance of our custom user authorization object passing requesting user's 'Windows Username' into constructor
         tbUsers requestingUser = unitWork.UsersManager.GetById (userId);
        //RBACUser requestingUser = new RBACUser(filterContext.RequestContext.HttpContext.User.Identity.Name);
        //Check if the requesting user has the permission to run the controller's action
        if (requestingUser != null)
        {
            if (!unitWork.UsersManager.HasPermission(AllowFeature, AllowPermission, userId) && requestingUser.RoleID != 1) // is not system admin
            {
                //User doesn't have the required permission and is not a SysAdmin, return our custom “401 Unauthorized” access error
                //Since we are setting filterContext.Result to contain an ActionResult page, the controller's action will not be run.
                //The custom “401 Unauthorized” access error will be returned to the browser in response to the initial request.
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorised" } });
            }
        }
        else 
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "login" }, { "controller", "Home" } });
        }
            //If the user has the permission to run the controller's action, then filterContext.Result will be uninitialized and
            //executing the controller's action is dependant on whether filterContext.Result is uninitialized.
        }
    }

