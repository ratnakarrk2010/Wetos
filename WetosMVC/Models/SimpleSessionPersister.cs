using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC.Models
{
    public class SimpleSessionPersister
    {
        public static string usernameSessionVar = "username";
        public static string useridSessionVar = "userid";
        private static string userGuidVar = "UserGUID";
        public static string userPhotonPathVar = "photoPath";
        public static string usertypeIdvar = "UserTypeId";
        public static string userRoleId = "UserRoleId";
        public static string userLastLoginVar = "lastlogin";
        public static string countryid = "countryId";
        public static string departmentid = "DepartmentId";
        public static string department = "Department";

        // SimplesessionPersister.Username
        public static string Username
        {
            get
            {
                if (HttpContext.Current == null) return string.Empty;

                var sessionVar =
                    HttpContext.Current.Session[usernameSessionVar];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionVar] = value;
            }
        }

        // SimplesessionPersister.UserId
        public static int UserID
        {
            get
            {
                if (HttpContext.Current == null) return 0;

                var sessionVar =
                    HttpContext.Current.Session[useridSessionVar];
                if (sessionVar != null)
                    return Convert.ToInt32(sessionVar);
                return 0;
            }
            set
            {
                HttpContext.Current.Session[useridSessionVar] = value;
            }
        }

        // SimplesessionPersister.UserId
        public static int CountryId
        {
            get
            {
                if (HttpContext.Current == null) return 0;

                var sessionVar =
                    HttpContext.Current.Session[countryid];
                if (sessionVar != null)
                    return Convert.ToInt32(sessionVar);
                return 0;
            }
            set
            {
                HttpContext.Current.Session[countryid] = value;
            }
        }

        // SimplesessionPersister.UserId
        public static long UserRoleId
        {
            get
            {
                if (HttpContext.Current == null) return 0;

                var sessionVar =
                    HttpContext.Current.Session[userRoleId];
                if (sessionVar != null)
                    return Convert.ToInt32(sessionVar);
                return 0;
            }
            set
            {
                HttpContext.Current.Session[userRoleId] = value;
            }
        }

        //SimplesessionPersister.IsMcsChange
        public static Guid UserGuid
        {
            get
            {
                if (HttpContext.Current == null) return new Guid("00000000000000000000000000000000");

                var sessionVar =
                    HttpContext.Current.Session[userGuidVar];
                if (sessionVar != null)
                    return new Guid(sessionVar.ToString());
                return new Guid("00000000000000000000000000000000");
            }
            set
            {
                HttpContext.Current.Session[userGuidVar] = value;
            }
        }

        // PHOTO PATH
        public static string photoPath
        {
            get
            {
                if (HttpContext.Current == null) return string.Empty;

                var sessionVar =
                    HttpContext.Current.Session[userPhotonPathVar];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[userPhotonPathVar] = value;
            }
        }

        // SimplesessionPersister.UserTypeId
        public static int UserTypeID
        {
            get
            {
                if (HttpContext.Current == null) return 0;

                var sessionVar =
                    HttpContext.Current.Session[usertypeIdvar];
                if (sessionVar != null)
                    return Convert.ToInt32(sessionVar);
                return 0;
            }
            set
            {
                HttpContext.Current.Session[usertypeIdvar] = value;
            }
        }

        // SimplesessionPersister.LastLogin
        public static string LastLogin
        {
            get
            {
                if (HttpContext.Current == null) return string.Empty;

                var sessionVar =
                    HttpContext.Current.Session[userLastLoginVar];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[userLastLoginVar] = value;
            }
        }

        public static int DepartmentId
        {
            get
            {
                if (HttpContext.Current == null) return 0;

                var sessionVar =
                    HttpContext.Current.Session[departmentid];
                if (sessionVar != null)
                    return Convert.ToInt32(sessionVar);
                return 0;
            }
            set
            {
                HttpContext.Current.Session[departmentid] = value;
            }
        }

        public static string Department
        {
            get
            {
                if (HttpContext.Current == null) return string.Empty;

                var sessionVar =
                    HttpContext.Current.Session[department];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session[department] = value;
            }
        }

        //public static string UserRole
        //{
        //    get
        //    {
        //        if (HttpContext.Current == null) return string.Empty;

        //        var sessionVar =
        //            HttpContext.Current.Session[UserRole];
        //        if (sessionVar != null)
        //            return sessionVar as string;
        //        return null;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[UserRole] = value;
        //    }
        //}
    }
}