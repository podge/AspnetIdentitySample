using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Common
{
    public class Common
    {
        public static IEnumerable<SelectListItem> GetCountryList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "United States", Value = "B"},
                new SelectListItem{Text = "Canada", Value = "B"},
                new SelectListItem{Text = "United Kingdom", Value = "B"},
                new SelectListItem{Text = "Texas", Value = "B"},
                new SelectListItem{Text = "Washington", Value = "B"}

            };
            return items;
        }
    }
}