using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Linq;
using Mvc5SiteMapBuilder.DataSource;
using Mvc5SiteMapBuilder.Extensions;

namespace Mvc5SiteMapBuilder.Providers
{
    public class XmlSiteMapNodeProvider : IXmlSiteMapNodeProvider
    {
        public NodeProviderType ProviderFormatType
        {
            get { return NodeProviderType.Xml; }
        }

        protected readonly string rootName = "mvcSiteMap";
        protected readonly string nodeName = "mvcSiteMapNode";
        protected readonly string xmlSiteMapNamespace = string.Empty;
        protected readonly NodeKeyGenerator nodeKeyGenerator = new NodeKeyGenerator();

        public virtual IEnumerable<SiteMapNode> GetSiteMapNodes(ISiteMapDataSource dataSource)
        {
            var xmlDataSource = dataSource as ISiteMapXmlDataSource;
            if (xmlDataSource == null)
                throw new InvalidOperationException($"Invalid datasource type, expected {typeof(ISiteMapXmlDataSource)} but received {dataSource.GetType()}");

            var xml = xmlDataSource.GetSiteMapData();
            if (xml == null)
                throw new InvalidOperationException("File datasource is empty");

            return LoadSiteMapFromXml(xml);
        }

        protected virtual void FixXmlNamespaces(XDocument xml)
        {
            // If no namespace is present (or the wrong one is present), replace it
            foreach (var node in xml.Descendants())
            {
                if (string.IsNullOrEmpty(node.Name.Namespace.NamespaceName) || node.Name.Namespace.NamespaceName != xmlSiteMapNamespace)
                {
                    node.Name = XName.Get(node.Name.LocalName, xmlSiteMapNamespace);
                }
            }
        }

        protected virtual IEnumerable<SiteMapNode> LoadSiteMapFromXml(XDocument xml)
        {
            FixXmlNamespaces(xml);
            var rootElement = GetRootElement(xml);
            var root = GetRootNode(rootElement);

            // get collection of sitemap nodes
            // add root node to collection
            var siteMapNodes = new List<SiteMapNode> { root };

            // recursively process xml nodes
            ProcessXmlNodes(root, rootElement);

            return siteMapNodes;
        }

        protected virtual XElement GetRootElement(XDocument xml)
        {
            // get the root mvcSiteMapNode element, and map this to an mvcSiteMapNode
            return xml.Element(rootName).Element(nodeName);
        }

        protected virtual SiteMapNode GetRootNode(XElement rootElement)
        {
            return GetSiteMapNodeFromXmlElement(rootElement, null);
        }

        protected virtual SiteMapNode GetSiteMapNodeFromXmlElement(XElement node, SiteMapNode parentNode)
        {
            // get data required to generate the node instance

            // get area and controller from node declaration
            var area = InheritAreaIfNotProvided(node, parentNode);
            var controller = InheritControllerIfNotProvided(node, parentNode);
            var action = node.GetAttributeValue("action");
            var url = node.GetAttributeValue("url");
            var explicitKey = node.GetAttributeValue("key");
            var parentKey = parentNode == null ? string.Empty : parentNode.Key;
            var httpMethod = node.GetAttributeValueOrFallback("httpMethod", HttpVerbs.Get.ToString()).ToUpperInvariant();
            var clickable = bool.Parse(node.GetAttributeValueOrFallback("clickable", "true"));
            var title = node.GetAttributeValue("title");
            var description = node.GetAttributeValue("description");
            var targetFrame = node.GetAttributeValue("targetFrame");
            var imageUrl = node.GetAttributeValue("imageUrl");
            var order = int.Parse(node.GetAttributeValueOrFallback("order", "0"));
            var dynamicNodeProvider = node.GetAttributeValue("dynamicNodeProvider");
            //var implicitResourceKey = node.GetAttributeValue("resourceKey");

            var siteMapNode = new SiteMapNode
            {
                Key = nodeKeyGenerator.GenerateKey(parentKey, explicitKey, url, title, area, controller, action, httpMethod, clickable),
                Title = title,
                Clickable = clickable,
                Area = area,
                Controller = controller,
                Action = action,
                DynamicNodeProvider = dynamicNodeProvider,
                Attributes = new Dictionary<string, object>(),
                ChildNodes = new List<SiteMapNode>(),
                Description = description,
                TargetFrame = targetFrame,
                ImageUrl = imageUrl,
                Url = url,
                UnresolvedUrl = url,
                Order = order
            };

            return siteMapNode;
        }

        /// <summary>
        /// Recursively process our XML document, parsing our siteMapNodes and dynamicNode(s).
        /// </summary>
        /// <param name="rootNode">The main root siteMap node.</param>
        /// <param name="rootElement">the main root XML element.</param>
        protected virtual void ProcessXmlNodes(SiteMapNode rootNode, XElement rootElement)
        {
            // loop through each element below the current root element
            foreach (XElement node in rootElement.Elements())
            {
                SiteMapNode childNode;
                if (!node.Name.LocalName.Equals(nodeName, StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Element of type {node.Name.LocalName} is an invalid node.");

                // if this is a normal mvcSieMapNode then map the xml element
                // to an mvcSiteMapNode, and add the node to the current root.
                childNode = GetSiteMapNodeFromXmlElement(node, rootNode);

                if (!string.IsNullOrEmpty(childNode.DynamicNodeProvider))
                {
                    // has dynamic node provider
                    var dynamicNodes = DynamicNodeBuilder.BuildDynamicNodes(childNode.DynamicNodeProvider, rootNode);

                    // add to root node
                    rootNode.ChildNodes.AddRange(dynamicNodes);

                    // add non-dynamic children for every dynamic node
                    foreach (var dynamicNode in dynamicNodes)
                    {
                        ProcessXmlNodes(dynamicNode, node);
                    }
                }
                else
                {
                    rootNode.ChildNodes.Add(childNode);

                    ProcessXmlNodes(childNode, node);
                }
            }
        }

        /// <summary>
        /// Inherits the area from the parent node if it is not provided in the current siteMapNode XML element and the parent node is not null.
        /// </summary>
        /// <param name="node">The siteMapNode element.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>The value provided by either the siteMapNode or parentNode.Area.</returns>
        protected virtual string InheritAreaIfNotProvided(XElement node, SiteMapNode parentNode)
        {
            var result = node.GetAttributeValue("area");
            if (node.Attribute("area") == null && parentNode != null)
            {
                result = parentNode.Area;
            }

            return result;
        }

        /// <summary>
        /// Inherits the controller from the parent node if it is not provided in the current siteMapNode XML element and the parent node is not null.
        /// </summary>
        /// <param name="node">The siteMapNode element.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>The value provided by either the siteMapNode or parentNode.Controller.</returns>
        protected virtual string InheritControllerIfNotProvided(XElement node, SiteMapNode parentNode)
        {
            var result = node.GetAttributeValue("controller");
            if (node.Attribute("controller") == null && parentNode != null)
            {
                result = parentNode.Controller;
            }

            return result;
        }
    }
}
