using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models.PageModels
{
    public class RegistrationsPageModel
    {
        public Parent Parent { get; set; }
        public List<Child> Children { get; set; }
        public RegistrationsPageModel()
        {
            Parent = new Parent();
            Children = new List<Child>();
        }
    }
}