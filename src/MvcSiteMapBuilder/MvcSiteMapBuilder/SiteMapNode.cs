using System.Collections.Generic;

namespace MvcSiteMapBuilder
{
    public class SiteMapNode
    {
        public string Key { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Clickable { get; set; }
        public string DynamicNodeProvider { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<SiteMapNode> Children { get; set; }

        //internal string Route { get; set; }

        public SiteMapNode()
        {
            Children = new List<SiteMapNode>();
            Attributes = new Dictionary<string, string>();
        }
    }
}
