
using SKHSS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models.PageModels
{
    public class RegisterPageModel
    {
        //public Int32 UserID { get; set; }
        public Parent Parent { get; set; }
        public Boolean Medical { get; set; }
        public Int32 Volunteer { get; set; }
        public List<RegisterPageChild> RegChildren { get; set; }
        public RegisterPageModel()
        {
            //UserID = 0;
            Parent = new Parent();
            Medical = false;
            Volunteer = -1;
            RegChildren = new List<RegisterPageChild>();
        }
        public RegisterPageModel(Parent Parent)
        {
            this.Parent = Parent;
            //UserId = this.Parent.UserLogin.USR_UserID;
            Medical = false;
            Order cur = Data.GetCurrentOrder(Parent);
            if (cur != null)
            {
                Volunteer = cur.ORD_Volunteer;
            }
            RegChildren = new List<RegisterPageChild>();
            foreach (Child c in Parent.Children.OrderBy(m => m.CLD_Birthdate).ToList()){
                RegChildren.Add(new RegisterPageChild(c));
            }
        }
    }
}