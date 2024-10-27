using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

[Serializable]
public class SesssionUser
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        public string LoginName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        private Boolean IsAuth { get; set; }
        public string Department
        {
            get
            {
                return GetDepartment();
            }
        }

     

        private string GetDepartment()
        {
            if (!IsAuth)
            { return null; }
            else
            { 
                return "Dept"; 
            }
            //Gets the department.
        }

        private bool Authenticate(string userName, string password, string LDAPPath)
        {
            bool authentic = false;
            //try
            //{
            //    DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain,
            //    userName, password);
            //    object nativeObject = entry.NativeObject;
            //    authentic = true;
            //}
            //catch (DirectoryServicesCOMException) { }
            //return authentic;

            try
            {
                userName = userName + "@gpf.gov.kw";

                LdapConnection connection = new LdapConnection(LDAPPath);
                NetworkCredential credential = new NetworkCredential(userName, password);
                connection.Credential = credential;
                connection.Bind();
                authentic = true;
                
            }
            catch (LdapException lexc)
            {
                string error = lexc.Message;
               
            }
            catch (Exception exc)
            {
                string error = exc.Message;
                
            }
            return authentic;


        }

        /// <summary>
        /// Validates the user in the AD
        /// </summary>
        /// <returns>true if the credentials are correct else false</returns>
        public Boolean ValidateUser()
        {
            IsAuth = Authenticate(LoginName, Password, BOL.Properties.Settings.Default .LDAPPath);
            return IsAuth;
        }

    public static int GetCurrentUserId()
    {
        int UserId = 0;
        string userData = null;
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            if (FormsAuthentication.CookiesSupported == true)

            {
                //Look for an existing authorization cookie when challenged via [Authorize]
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null || authCookie.Value != "")
                {

                    FormsAuthenticationTicket authTicket = null;

                    //Reading from the ticket
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);


                    //FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;
                    userData = authTicket.UserData;
                    if (!String.IsNullOrEmpty(userData))
                    {
                        UserId = int.Parse(userData);
                    }
                    //separate the parts in userData and create an object according to your liking
                    //SesssionUser ud = new SesssionUser(userData);

                }

            }
            //if (HttpContext.Current.Session["UserProfile"] != null)
            //{
            //    var profileData = HttpContext.Current.Session["UserProfile"] as SesssionUser;
            //    UserId = profileData.UserId;
            //}


        }
        else
        {
           
        }
        return UserId;
    }
    }

