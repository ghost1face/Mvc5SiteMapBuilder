using System.Text;
using System.Web;

namespace MvcSiteMapBuilder.Cache
{
    public class SiteMapCacheKeyGenerator : ISiteMapCacheKeyGenerator
    {
        public virtual string GenerateKey()
        {
            var builder = new StringBuilder("sitemap://")
                .Append(GetHostName())
                .Append("/");

            return builder.ToString();
        }

        protected virtual string GetHostName()
        {
            var contextBase = new HttpContextWrapper(HttpContext.Current);
            var request = contextBase.Request;

            // In a cloud or web farm environment, use the HTTP_HOST 
            // header to derive the host name.
            if (request.ServerVariables["HTTP_HOST"] != null)
            {
                return request.ServerVariables["HTTP_HOST"];
            }

            return request.Url.DnsSafeHost;
        }
    }
}
