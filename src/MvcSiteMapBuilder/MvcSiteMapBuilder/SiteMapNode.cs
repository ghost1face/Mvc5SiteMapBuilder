using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcSiteMapBuilder
{
    public class SiteMapNode : ISortable, IEquatable<SiteMapNode>
    {
        public string Key { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string TargetFrame { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Clickable { get; set; }
        public string DynamicNodeProvider { get; set; }
        public int Order { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<SiteMapNode> ChildNodes { get; set; }

        //internal string Route { get; set; }

        #region Constructor

        public SiteMapNode()
        {
            ChildNodes = new List<SiteMapNode>();
            Attributes = new Dictionary<string, object>();
        }

        #endregion

        public bool HasChildNodes
        {
            get
            {
                return !(ChildNodes == null || !ChildNodes.Any());
            }
        }

        public bool Equals(SiteMapNode node)
        {
            if (Equals(node))
                return true;

            if (node == null)
                return false;

            return Key.Equals(node.Key);
        }

        /// <summary>
        /// Copies an existing sitemap node and all properties
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SiteMapNode Copy()
        {
            return new SiteMapNode
            {
                Action = Action,
                Area = Area,
                Attributes = new Dictionary<string, object>(Attributes),
                ChildNodes = ChildNodes.Select(n => n.Copy()).ToList(),
                Clickable = Clickable,
                Controller = Controller,
                Description = Description,
                DynamicNodeProvider = DynamicNodeProvider,
                ImageUrl = ImageUrl,
                Key = Key,
                Order = Order,
                TargetFrame = TargetFrame,
                Title = Title,
                Url = Url
            };
        }
    }
}
