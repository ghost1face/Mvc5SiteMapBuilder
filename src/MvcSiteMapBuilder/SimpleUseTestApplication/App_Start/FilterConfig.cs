using System.Web;
using System.Web.Mvc;

namespace SimpleUseTestApplication
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
