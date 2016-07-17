using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKHSS.Helpers.HTMLObjects
{
    public class EditableField
    {
        public EditableField()
        {
        }
        public HtmlString create(String groupid, Int32 id, String fieldName, String value, String type = "text", String displayValue = "", String prefix = "", List<String> AddlData = null)
        {
            if (displayValue == "")
            {
                displayValue = value;
            }
            AddlData = AddlData ?? new List<String>();
            TagBuilder b = new TagBuilder("span");
            if (type == "multiline")
            {
                b = new TagBuilder("div");
            }
            b.Attributes.Add("id", "ef-" + groupid + id);
            b.Attributes.Add("data-status", "closed");
            /*b.AddCssClass(groupid);*/
            b.AddCssClass("editable-field");
            if (prefix != "")
            {
                b.AddCssClass(prefix);
            }
            
            TagBuilder t = new TagBuilder("input");
            t.Attributes.Add("value",fieldName);
            t.Attributes.Add("type", "hidden");
            t.AddCssClass("title");

            TagBuilder d = new TagBuilder("span");
            d.AddCssClass("display");
            if(fieldName=="TeamID"){
                d.InnerHtml = "<a href=\"/Members/Team/"+value+"\">"+displayValue+"</a>";
            }
            else{
                d.InnerHtml = displayValue;
            }

            TagBuilder e = new TagBuilder("input");
            e.Attributes.Add("type",type);
            e.Attributes.Add("value", value);
            e.Attributes.Add("data-autosize-input", "{ \"space\": 3 }");
            e.Attributes.Add("id", groupid + fieldName);
            e.AddCssClass("edit");
            e.AddCssClass("hide");
            e.AddCssClass(groupid + fieldName);

            TagBuilder m = new TagBuilder("textarea");
            m.InnerHtml = value;
            m.Attributes.Add("data-autosize-input", "{ \"space\": 3 }");
            m.Attributes.Add("id", groupid + fieldName);
            m.AddCssClass("edit");
            m.AddCssClass("hide");

            TagBuilder s = new TagBuilder("select");
            s.AddCssClass("edit");
            s.AddCssClass("hide");
            s.Attributes.Add("type", "select");
            s.Attributes.Add("id", groupid + fieldName);
            //s.Attributes.Add("selectedIndex", value);
            String str = "";
            for (int i = 0; i < AddlData.Count; i++)
            {
                str += "<option value=\"" + i + "\" " + (i.ToString() == value ? "selected" : "") +">" + AddlData[i] + "</option>";
            }
            s.InnerHtml = str;

            String input = "";
            switch (type)
            {
                case "select":
                    input = s.ToString();
                    break;
                case "text":
                    input = e.ToString();
                    break;
                case "multiline":
                    input = m.ToString();
                    break;
                default:
                    input = e.ToString();
                    break;
            }
            b.InnerHtml = t.ToString() + d.ToString() + input;// (type == "select" ? s.ToString() : e.ToString());

            return new HtmlString(b.ToString());
        }
    }
}