using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class EditableRow
    {
        public List<String> Keys { get; set; }
        public String Content { get; set; }
        public Int32 Number { get; set; }
        public Boolean ClickBtn { get; set; }
        public EditableRow()
        {
            Keys = new List<String>();
            Content = "";
            Number = 1;
            ClickBtn = false;
        }
        public EditableRow(List<String> Keys, String Content, Int32 Number)
        {
            this.Keys = Keys;
            this.Content = Content;
            this.Number = Number;
        }
        public EditableRow(List<String> Keys, String Content, Int32 Number, Boolean ClickBtn)
        {
            this.Keys = Keys;
            this.Content = Content;
            this.Number = Number;
            this.ClickBtn = ClickBtn;
        }
    }
}