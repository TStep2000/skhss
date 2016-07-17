using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SKHSS.Models;

namespace SKHSS.Authentication
{
    public class SKHSSUser
    {
        [Required]
        public Guid USR_RecordID { get { return UserLogin.USR_RecordID; } set { UserLogin.USR_RecordID = value; } }
        //public Int32 RoleID { get { return UserLogin.USR_ROL_RoleID; } set { UserLogin.USR_ROL_RoleID = value; } }
        [Required]
        public String Password { get { return UserLogin.USR_Password; } set { UserLogin.USR_Password = value; } }
        [Required]
        public String Email { get { return UserLogin.USR_Email; } set { UserLogin.USR_Email = value; } }
        public String Phone { get { return UserLogin.USR_Phone; } set { UserLogin.USR_Phone = value; } }
        public String Username { get { return UserLogin.USR_Username; } set { UserLogin.USR_Username = value; } }
        //public String RoleName { get { return Role.ROL_RoleName; } set { Role.ROL_RoleName = value; } }
        public UserLogin UserLogin;
        //public Role Role;
        public SKHSSUser()
        {
            this.UserLogin = new UserLogin();
            //this.Role = new Role();
        }
        public SKHSSUser(UserLogin UserLogin)//, Role Role)
        {
            this.UserLogin = UserLogin;
            //this.Role = Role;
        }
    }
}