using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Mvc5SiteMapBuilder.DataSource;
using Newtonsoft.Json.Linq;

namespace Mvc5SiteMapBuilder.Providers
{
    public class JSONSiteMapNodeProvider : IJSONSiteMapNodeProvider
    {
        protected readonly NodeKeyGenerator nodeKeyGenerator = new NodeKeyGenerator();

        public virtual IEnumerable<SiteMapNode> GetSiteMapNodes(ISiteMapDataSource dataSource)
        {
            var jsonDataSource = dataSource as ISiteMapJSONDataSource;
            if (jsonDataSource == null)
                throw new InvalidOperationException($"Invalid datasource type, expected {typeof(ISiteMapJSONDataSource)} but received {dataSource.GetType()}");

            var json = jsonDataSource.GetSiteMapData();
            if (json == null)
                throw new InvalidOperationException("File datasource is empty");

            return LoadSiteMapFromJSON(json);
        }

        protected virtual IEnumerable<SiteMapNode> LoadSiteMapFromJSON(JObject json)
        {
            var jsonNodeHierarchy = json.ToObject<SiteMapNode>();
            var root = GetSiteMapNodeFromJSONNode(jsonNodeHierarchy, null);

            // get collection of sitemap nodes
            // add root node to collection
            var siteMapNodes = new List<SiteMapNode> { root };

            // recursively process nodes
            ProcessNodes(jsonNodeHierarchy.ChildNodes, root);

            return siteMapNodes;
        }

        protected virtual void ProcessNodes(IEnumerable<SiteMapNode> nodes, SiteMapNode rootNode)
        {
            foreach (var node in nodes)
            {
                var siteMapNode = GetSiteMapNodeFromJSONNode(node, rootNode);

                if (!string.IsNullOrEmpty(siteMapNode.DynamicNodeProvider))
                {
                    // has dynamic node provider 
                    var dynamicNodes = DynamicNodeBuilder.BuildDynamicNodes(siteMapNode.DynamicNodeProvider, rootNode);

                    // add to root node
                    rootNode.ChildNodes.AddRange(dynamicNodes);

                    // add non-dynamic children for every dynamic node
                    ProcessNodes(dynamicNodes, rootNode);
                }
                else
                {
                    rootNode.ChildNodes.Add(siteMapNode);

                    ProcessNodes(node.ChildNodes, siteMapNode);
                }
            }
        }

        protected virtual SiteMapNode GetSiteMapNodeFromJSONNode(SiteMapNode jsonNode, SiteMapNode parentNode)
        {
            // get data required to generate the node instance

            // get area and controller
            var area = InheritAreaIfNotProvided(jsonNode, parentNode);
            var controller = InheritControllerIfNotProvided(jsonNode, parentNode);
            var action = jsonNode.Action;
            var url = jsonNode.Url;
            var explicitKey = jsonNode.Key;
            var parentKey = parentNode == null ? string.Empty : parentNode.Key;
            var httpMethod = (string.IsNullOrEmpty(jsonNode.HttpMethod) ? HttpVerbs.Get.ToString() : jsonNode.HttpMethod).ToUpperInvariant();
            var clickable = jsonNode.Clickable;
            var title = jsonNode.Title;
            var description = jsonNode.Description;
            var targetFrame = jsonNode.TargetFrame;
            var imageUrl = jsonNode.ImageUrl;
            var order = jsonNode.Order;
            var dynamicNodeProvider = jsonNode.DynamicNodeProvider;

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

        protected virtual string InheritAreaIfNotProvided(SiteMapNode node, SiteMapNode parentNode)
        {
            var result = node.Area;
            if (string.IsNullOrEmpty(result) && parentNode != null)
            {
                result = parentNode.Area;
            }

            return result;
        }

        protected virtual string InheritControllerIfNotProvided(SiteMapNode node, SiteMapNode parentNode)
        {
            var result = node.Controller;
            if (string.IsNullOrEmpty(result) && parentNode != null)
            {
                result = parentNode.Controller;
            }

            return result;
        }
    }
}
