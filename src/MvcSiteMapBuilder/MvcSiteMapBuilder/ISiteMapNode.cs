using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcSiteMapBuilder
{
    public interface ISiteMapNode : ISortable, IEquatable<ISiteMapNode>
    {
        string Key { get; set; }
        string Area { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        string Description { get; set; }
        string TargetFrame { get; set; }
        string ImageUrl { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        bool Clickable { get; set; }
        string DynamicNodeProvider { get; set; }
        Dictionary<string, object> Attributes { get; set; }
        List<ISiteMapNode> ChildNodes { get; set; }
        ISiteMapNode Copy();

    }
}
