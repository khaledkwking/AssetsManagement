using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace ProjectsManagements.Utilities
{
    public class BaseClass2 : System.Web.UI.Page
    {
        private void Page_PreInit(object sender, System.EventArgs e)
        {
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
            {
                Session["SiteLanguage"] = "ArabicTheme";
                Page.Theme = "ArabicTheme";
                Page.Title = "جهاز متابعة الاداء الحكومي | نظام إدارة العهد.";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
                
            }
            else if (Session["SiteLanguage"].ToString() == "ArabicTheme")
            {
                Page.Theme = "ArabicTheme";
                Page.Title = "جهاز متابعة الاداء الحكومي | نظام إدارة العهد.";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
            }
            else if (Session["SiteLanguage"].ToString() == "EnglishTheme")
            {
                Page.Theme = "EnglishTheme";
                Page.Title = "GPFA | Projects Managements.";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                Page.Theme = "ArabicTheme";
                Page.Title = "جهاز متابعة الاداء الحكومي | نظام إدارة العهد.";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
            }
        }


    }
}