using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class Editable
    {
        public String Key { get; set; }
        public String Class { get; set; }
        public String Value { get; set; }
        public String DisplayValue { get; set; }
        public Editable()
        {
            this.Key = "";
            this.Class = "";
            this.Value = "";
            this.DisplayValue = "";
        }
        public Editable(String Key, String Value)
        {
            this.Key = Key;
            this.Class = Key;
            this.Value = Value;
            this.DisplayValue = Value;
        }
        public Editable(String Key, String Value, String DisplayValue)
        {
            this.Key = Key;
            this.Class = Key;
            this.Value = Value;
            this.DisplayValue = DisplayValue;
        }
        public Editable(String Key, String Value, String DisplayValue, String Class)
        {
            this.Key = Key;
            this.Class = Class;
            this.Value = Value;
            this.DisplayValue = DisplayValue;
        }
    }
}