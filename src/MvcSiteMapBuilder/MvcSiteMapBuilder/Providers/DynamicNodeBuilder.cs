using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcSiteMapBuilder.Providers
{
    public static class DynamicNodeBuilder
    {
        private static readonly object syncLock = new object();
        private static readonly Dictionary<Type, IDynamicNodeProvider> cachedNodeProviders = new Dictionary<Type, IDynamicNodeProvider>();
        private static readonly NodeKeyGenerator nodeKeyGenerator = new NodeKeyGenerator();

        public static IEnumerable<SiteMapNode> BuildDynamicNodes(string assemblyQualifiedTypeString, SiteMapNode parentNode)
        {
            var type = Type.GetType(assemblyQualifiedTypeString);
            if (type == null)
                throw new Exception($"Type {assemblyQualifiedTypeString} was not found.");

            IDynamicNodeProvider dynamicNodeProvider;
            if (!cachedNodeProviders.TryGetValue(type, out dynamicNodeProvider))
            {
                lock (syncLock)
                {
                    if (!cachedNodeProviders.TryGetValue(type, out dynamicNodeProvider))
                    {
                        dynamicNodeProvider = Activator.CreateInstance(type) as IDynamicNodeProvider;

                        cachedNodeProviders.Add(type, dynamicNodeProvider);
                    }
                }
            }
            
            foreach(var dynamicNode in dynamicNodeProvider.GetSiteMapNodes())
            {
                yield return ProcessNode(dynamicNode, parentNode);
            }
        }

        private static SiteMapNode ProcessNode(SiteMapNode nodeToProcess, SiteMapNode parentNode)
        {
            var area = string.IsNullOrEmpty(nodeToProcess.Area) ? parentNode.Area : nodeToProcess.Area;
            var controller = string.IsNullOrEmpty(nodeToProcess.Controller) ? parentNode.Controller : nodeToProcess.Controller;
            var action = nodeToProcess.Action;
            var url = nodeToProcess.Url;
            var explicitKey = nodeToProcess.Key;
            var parentKey = parentNode == null ? "" : parentNode.Key;
            var httpMethod = HttpVerbs.Get.ToString().ToUpperInvariant();
            var clickable = true;
            var title = nodeToProcess.Title;

            string key = nodeKeyGenerator.GenerateKey(
                parentKey,
                explicitKey,
                url,
                title,
                area,
                controller,
                action,
                httpMethod,
                clickable
            );

            nodeToProcess.Area = area;
            nodeToProcess.Controller = controller;
            nodeToProcess.Action = action;
            nodeToProcess.Url = url;
            nodeToProcess.UnresolvedUrl = url;
            nodeToProcess.Key = key;
            nodeToProcess.Clickable = clickable;
            nodeToProcess.Title = title;

            return nodeToProcess;
        }
    }
}
