using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc5SiteMapBuilder
{
    [Serializable]
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
        public string UnresolvedUrl { get; set; }
        public bool Clickable { get; set; }
        public string DynamicNodeProvider { get; set; }
        public int Order { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<SiteMapNode> ChildNodes { get; set; }
        public string HttpMethod { get; set; }

        //internal string Route { get; set; }

        #region Constructor

        public SiteMapNode()
        {
            ChildNodes = new List<SiteMapNode>();
            Attributes = new Dictionary<string, object>();
            Clickable = true;
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
            if (ReferenceEquals(this, node))
                return true;

            if (node == null)
                return false;

            return Key.Equals(node.Key);
        }

        public override bool Equals(object obj)
        {
            var node = obj as SiteMapNode;
            if (node == null)
                return false;
            return Equals(node);
        }

        public static bool operator ==(SiteMapNode node1, SiteMapNode node2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(node1, node2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)node1 == null) || ((object)node2 == null))
            {
                return false;
            }

            return node1.Equals(node2);
        }

        public static bool operator !=(SiteMapNode node1, SiteMapNode node2)
        {
            return !(node1 == node2);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Key;
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
                HttpMethod = HttpMethod,
                ImageUrl = ImageUrl,
                Key = Key,
                Order = Order,
                TargetFrame = TargetFrame,
                Title = Title,
                Url = Url,
                UnresolvedUrl = UnresolvedUrl
            };
        }
    }
}
