using System.Web;
using System.Web.Mvc;

namespace EmployeeMGMT_MVP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
