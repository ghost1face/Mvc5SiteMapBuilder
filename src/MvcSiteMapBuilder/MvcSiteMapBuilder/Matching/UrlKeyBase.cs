using System;
using MvcSiteMapBuilder.Web;

namespace MvcSiteMapBuilder.Matching
{
    /// <summary>
    /// An abstract class containing the logic for comparing 2 IUrlKey instances.
    /// </summary>
    public abstract class UrlKeyBase
        : IUrlKey
    {
        protected string hostName;
        protected string rootRelativeUrl;

        public virtual string HostName { get { return hostName; } }

        public virtual string RootRelativeUrl { get { return rootRelativeUrl; } }

        protected virtual void SetUrlValues(string relativeOrAbsoluteUrl)
        {
            if (UrlPath.IsAbsolutePhysicalPath(relativeOrAbsoluteUrl) || UrlPath.IsAppRelativePath(relativeOrAbsoluteUrl))
            {
                rootRelativeUrl = UrlPath.ResolveVirtualApplicationToRootRelativeUrl(relativeOrAbsoluteUrl);
            }
            else if (UrlPath.IsAbsoluteUrl(relativeOrAbsoluteUrl))
            {
                var absoluteUri = new Uri(relativeOrAbsoluteUrl, UriKind.Absolute);

                // NOTE: this will cut off any fragments, but since they are not passed
                // to the server, this is desired.
                rootRelativeUrl = absoluteUri.PathAndQuery;
                hostName = absoluteUri.Host;
            }
            else
            {
                // We must assume we already have a relative root URL
                rootRelativeUrl = relativeOrAbsoluteUrl;
            }
        }

        // Source: http://stackoverflow.com/questions/70303/how-do-you-implement-gethashcode-for-structure-with-two-string#21604191
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;

                // String properties
                hashCode = (hashCode * 397) ^ (HostName != null ? HostName.GetHashCode() : string.Empty.GetHashCode());
                hashCode = (hashCode * 397) ^ (RootRelativeUrl != null ? RootRelativeUrl.GetHashCode() : string.Empty.GetHashCode());

                //// int properties
                //hashCode = (hashCode * 397) ^ intProperty;

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == null)
            {
                return false;
            }
            var objB = obj as IUrlKey;
            if (objB == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!string.Equals(RootRelativeUrl, objB.RootRelativeUrl, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!string.Equals(HostName, objB.HostName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return string.Format("[HostName: {0}, RootRelativeUrl: {1}]", HostName, RootRelativeUrl);
        }
    }
}
