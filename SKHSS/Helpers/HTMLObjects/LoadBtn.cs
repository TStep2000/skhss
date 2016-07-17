using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKHSS.Helpers.HTMLObjects
{
    public class EditLoadSaveBtn
    {
        public EditLoadSaveBtn()
        {
        }
        public HtmlString create(String DataGroup, String SaveType, String Class = "")
        {
            TagBuilder imgbtn = new TagBuilder("div");
            imgbtn.AddCssClass("imgbtn");
            imgbtn.AddCssClass(Class);
            imgbtn.Attributes.Add("data-group", DataGroup);

            TagBuilder edit = new TagBuilder("img");
            edit.AddCssClass("edit");
            edit.Attributes.Add("data-page", "1");
            edit.Attributes.Add("data-func", "edit");
            edit.Attributes.Add("src", "/Content/icons/edit.png");
            edit.Attributes.Add("alt", "edit");

            TagBuilder cancel = new TagBuilder("img");
            cancel.AddCssClass("cancel");
            cancel.Attributes.Add("data-page", "0");
            cancel.Attributes.Add("data-func", "cancel");
            cancel.Attributes.Add("src", "/Content/icons/undo.png");
            cancel.Attributes.Add("alt", "cancel");

            TagBuilder page1 = new TagBuilder("div");
            page1.AddCssClass("hide");
            page1.AddCssClass("hide1");
            page1.AddCssClass("1");

            TagBuilder save = new TagBuilder("img");
            save.AddCssClass("save");
            save.Attributes.Add("data-func", "save");
            save.Attributes.Add("data-save-type", SaveType);
            save.Attributes.Add("src", "/Content/icons/save.png");
            save.Attributes.Add("alt", "save");

            TagBuilder load = new TagBuilder("img");
            load.AddCssClass("hide");
            load.AddCssClass("hide1");
            load.AddCssClass("load");
            load.Attributes.Add("src", "/Content/loading.gif");
            load.Attributes.Add("alt", "loading");

            TagBuilder check = new TagBuilder("img");
            check.AddCssClass("hide");
            check.AddCssClass("hide1");
            check.AddCssClass("chk");
            check.Attributes.Add("src", "/Content/icons/gCheck.png");
            check.Attributes.Add("alt", "done");

            page1.InnerHtml = cancel.ToString() + save.ToString() + load.ToString() + check.ToString();
            imgbtn.InnerHtml = edit.ToString() + page1.ToString();

            return new HtmlString(imgbtn.ToString());
        }
    }
}