namespace MvcSiteMapBuilder.Security
{
    public interface IAclModule
    {
        /// <summary>
        /// Determines whether node is accessible to user.
        /// </summary>
        /// <param name="siteMapNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if accessible to user; otherwise, <c>false</c>.
        /// </returns>
        bool IsAccessibleToUser(SiteMapNode siteMapNode);
    }
}
