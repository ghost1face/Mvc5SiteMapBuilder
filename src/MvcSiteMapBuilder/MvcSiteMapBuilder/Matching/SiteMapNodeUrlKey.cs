using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcSiteMapBuilder.Matching
{
    ///// <summary>
    ///// Uses an <see cref="T:MvcSiteMapProvider.ISiteMapNode"/>  instance to create a key 
    ///// that can be used for matching relative or absolute URLs.
    ///// </summary>
    //public class SiteMapNodeUrlKey
    //    : UrlKeyBase
    //{
    //    public SiteMapNodeUrlKey(
    //        SiteMapNode node)
    //    {
    //        if (node == null)
    //            throw new ArgumentNullException(nameof(node));

    //        this.node = node;

    //        // Host name in absolute URL overrides this one.
    //        this.hostName = node.HostName;

    //        // Fixes #322 - If using a custom URL resolver, we need to account for the case that
    //        // the URL will be provided by the resolver instead of specified explicitly.
    //        if (!string.IsNullOrEmpty(node.UnresolvedUrl))
    //        {
    //            this.SetUrlValues(node.UnresolvedUrl);
    //        }
    //        else if (!node.UsesDefaultUrlResolver())
    //        {
    //            // For a custom URL resolver, if the unresolved URL property
    //            // is not set use the one returned from the URL resolver.
    //            // This ensures URLs that are unidentifiable by MVC can still
    //            // be matched by URL.
    //            this.SetUrlValues(node.Url);
    //        }
    //    }

    //    private readonly SiteMapNode node;

    //    public override string HostName
    //    {
    //        // The host name of the node can be modified at runtime, so we need to ensure
    //        // we have the most current value.
    //        get { return string.IsNullOrEmpty(node.HostName) ? this.hostName : node.HostName; }
    //    }
    //}
}
