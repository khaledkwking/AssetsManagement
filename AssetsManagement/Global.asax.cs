using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;


namespace AssetsManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            HttpContext.Current.Session["SiteLanguage"] = "ArabicTheme";
            //HttpContext.Current.Session["beingredirected"] = "false";
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
           
            //HttpCookie cookieName = HttpContext.Current.Request.Cookies.Get("Name");
            //HttpCookie cookieUserId = HttpContext.Current.Request.Cookies.Get("UserId");
            ////HttpCookie cookieUserType = HttpContext.Current.Request.Cookies.Get("UserType");

            //if ( (cookieUserId == null || cookieName == null))
            //{
            //    //Response.Redirect("~/Default.aspx");
            //}

            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                //var underconstruction = HttpContext.Current.Session["underconstruction"];
                //if (underconstruction != null)
                //{
                //    string oc = underconstruction.ToString();
                //    if (oc != "false") Response.Redirect("~/Home/UnderConstruction");
                //}

                if (Request.QueryString["Lang"] != null)
                {
                    if (Request.QueryString["Lang"] == "en")
                    {
                        Session["SiteLanguage"] = "EnglishTheme";
                    }
                    else if (Request.QueryString["Lang"] == "ar")
                    {
                        Session["SiteLanguage"] = "ArabicTheme";
                    }
                }


                if (Session["SiteLanguage"] == null)
                //if (String.IsNullOrEmpty(value))
                {
                    Session["SiteLanguage"] = "ArabicTheme";
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");

                }

                else if (Session["SiteLanguage"].ToString() == "ArabicTheme")
                {

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
                }
                else if (Session["SiteLanguage"].ToString() == "EnglishTheme")
                {

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }
                else
                {

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
                }
            }
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            //newCulture.DateTimeFormat.ShortDatePattern = "dd/MMM/yyyy";
            //newCulture.DateTimeFormat.DateSeparator = "/";
            //Thread.CurrentThread.CurrentCulture = newCulture;
            if (!Request.IsLocal)
            {
                switch (Request.Url.Scheme)
                {
                    case "https":
                        Response.AddHeader("Strict-Transport-Security", "max-age=300");
                        break;
                    case "http":
                        var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", path);
                        break;
                }
            }
        }

        protected void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
        {
            //STEP #1 of Authentication/Authorization flow
            //Reference:  http://msdn.microsoft.com/en-us/library/ff649337.aspx
            //==================================================================
            if (FormsAuthentication.CookiesSupported == true)
            {
                //Look for an existing authorization cookie when challenged via [Authorize]
                HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null || authCookie.Value == "")
                {
                    return;
                }
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    //Reading from the ticket
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    //Check the Cookiename (which in this case is UserId).  If it is null, then we have an issue
                    if (authTicket.Name == null || authTicket.Expired)
                    {
                        FormsAuthentication.SignOut();
                        authCookie.Value = null;
                    }

                }
                catch (Exception ex)
                {
                    //Unable to decrypt the auth ticket
                    return;
                }

                //get userId from ticket
                string userId = authTicket.Name;


                Context.User = new GenericPrincipal(new System.Security.Principal.GenericIdentity(userId, "MyCustomAuthTypeName"), authTicket.UserData.Split(','));

                //We are officially 'authenticated' at this point but not neccessarily 'authorized'
            }
            else
            {
                throw new HttpException("Cookieless Forms Authentication is not supported for this application.");

            }
        }

       
    }
}
