using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models.PageModels
{
    public class AccountPageModel
    {
        public UserLogin UserLogin { get; set; }
        public Parent Parent { get; set; }
        public List<Child> ChildrenByBirthdate { get; set; }
        public AccountPageModel()
        {
            UserLogin = new UserLogin();
            Parent = new Parent();
            ChildrenByBirthdate = new List<Child>();
        }
        public AccountPageModel(UserLogin UserLogin)
        {
            this.UserLogin = UserLogin;
            Parent = new Parent();
            ChildrenByBirthdate = new List<Child>();
        }
        public AccountPageModel(Parent Parent)
        {
            this.Parent = Parent;
            this.UserLogin = Parent.UserLogin;
            this.ChildrenByBirthdate = Parent.Children.OrderBy(m => m.CLD_Birthdate).ToList();
        }
    }
}