using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcSiteMapBuilder.Web.Mvc
{
    public static class ControllerTypeResolver
    {
        private readonly static IDictionary<string, Type> cache = new ConcurrentDictionary<string, Type>();

        public static Type ResolveControllerType(string areaName, string controllerName)
        {
            // is the type cached?
            var cacheKey = areaName + "_" + controllerName;
            if (cache.ContainsKey(cacheKey))
            {
                return cache[cacheKey];
            }

            // find controller details
            IEnumerable<string> areaNamespaces = FindNamespacesForArea(areaName, RouteTable.Routes);

            var area = areaName;
            var controller = controllerName;
            var controllerBuilder = ControllerBuilder.Current;


            // Find controller type
            Type controllerType;
            HashSet<string> namespaces = null;
            if (areaNamespaces != null)
            {
                areaNamespaces = (from ns in areaNamespaces
                                  where ns != "Elmah.Mvc"
                                  //where !this.areaNamespacesToIgnore.Contains(ns)
                                  select ns).ToList();
                if (areaNamespaces.Any())
                {
                    namespaces = new HashSet<string>(areaNamespaces, StringComparer.OrdinalIgnoreCase);
                    if (string.IsNullOrEmpty(areaName))
                    {
                        namespaces = new HashSet<string>(namespaces.Union(controllerBuilder.DefaultNamespaces, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
            else if (controllerBuilder.DefaultNamespaces.Count > 0)
            {
                namespaces = controllerBuilder.DefaultNamespaces;
            }
            controllerType = GetControllerTypeWithinNamespaces(area, controller, namespaces);

            // Cache the result
            cache.Add(cacheKey, controllerType);

            // Return
            return controllerType;
        }

        /// <summary>
		/// Finds the namespaces for area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="routes">The routes.</param>
		/// <returns>
		/// A namespaces for area represented as a <see cref="string"/> instance
		/// </returns>
		private static IEnumerable<string> FindNamespacesForArea(string area, RouteCollection routes)
        {
            var namespacesForArea = new List<string>();
            var namespacesCommon = new List<string>();

            foreach (var route in routes.OfType<Route>().Where(r => r.DataTokens != null && r.DataTokens["Namespaces"] != null))
            {
                // search for area-based namespaces
                if (route.DataTokens["area"] != null && route.DataTokens["area"].ToString().Equals(area, StringComparison.OrdinalIgnoreCase))
                    namespacesForArea.AddRange((IEnumerable<string>)route.DataTokens["Namespaces"]);
                else if (route.DataTokens["area"] == null)
                    namespacesCommon.AddRange((IEnumerable<string>)route.DataTokens["Namespaces"]);
            }

            if (namespacesForArea.Count > 0)
            {
                return namespacesForArea;
            }

            if (namespacesCommon.Count > 0)
            {
                return namespacesCommon;
            }

            return null;
        }

        private static readonly Lazy<Dictionary<string, ILookup<string, Type>>> assemblyCache = new Lazy<Dictionary<string, ILookup<string, Type>>>(() =>
        {
            var controllerTypes = GetListOfControllerTypes();
            var groupedByName = controllerTypes.GroupBy(
                            t => t.Name.Substring(0, t.Name.IndexOf("Controller")),
                            StringComparer.OrdinalIgnoreCase);
            return groupedByName.ToDictionary(
                g => g.Key,
                g => g.ToLookup(t => t.Namespace ?? string.Empty, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
        });

        /// <summary>
		/// Gets the list of controller types.
		/// </summary>
		/// <returns></returns>
		private static List<Type> GetListOfControllerTypes()
        {
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;
            ICollection assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }
                typesSoFar = typesSoFar.Concat(typesInAsm);
            }
            return typesSoFar.Where(t => t != null && t.IsClass && t.IsPublic && !t.IsAbstract && t.Name.IndexOf("Controller", StringComparison.OrdinalIgnoreCase) != -1 && typeof(IController).IsAssignableFrom(t)).ToList();
        }

        /// <summary>
		/// Gets the controller type within namespaces.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="namespaces">The namespaces.</param>
		/// <returns>
		/// A controller type within namespaces represented as a <see cref="Type"/> instance 
		/// </returns>
		private static Type GetControllerTypeWithinNamespaces(string area, string controller, HashSet<string> namespaces)
        {
            if (string.IsNullOrEmpty(controller) || controller == "")
                return null;

            var assembliesCache = assemblyCache.Value;

            var matchingTypes = new HashSet<Type>();
            ILookup<string, Type> nsLookup;
            if (assembliesCache.TryGetValue(controller, out nsLookup))
            {
                // this friendly name was located in the cache, now cycle through namespaces
                if (namespaces != null)
                {
                    foreach (string requestedNamespace in namespaces)
                    {
                        foreach (var targetNamespaceGrouping in nsLookup)
                        {
                            if (IsNamespaceMatch(requestedNamespace, targetNamespaceGrouping.Key))
                            {
                                matchingTypes.UnionWith(targetNamespaceGrouping);
                            }
                        }
                    }
                }
                else
                {
                    // if the namespaces parameter is null, search *every* namespace
                    foreach (var nsGroup in nsLookup)
                    {
                        matchingTypes.UnionWith(nsGroup);
                    }
                }
            }

            if (matchingTypes.Count == 1)
            {
                return matchingTypes.First();
            }

            if (matchingTypes.Count > 1)
            {
                string typeNames = Environment.NewLine + Environment.NewLine;
                foreach (var matchingType in matchingTypes)
                    typeNames += matchingType.FullName + Environment.NewLine;
                typeNames += Environment.NewLine;

                throw new AmbiguousMatchException($"Multiple controller matches were found for controller {controller}, matches were {typeNames}");
            }
            return null;
        }

        /// <summary>
		/// Determines whether namespace matches the specified requested namespace.
		/// </summary>
		/// <param name="requestedNamespace">The requested namespace.</param>
		/// <param name="targetNamespace">The target namespace.</param>
		/// <returns>
		/// 	<c>true</c> if is namespace matches the specified requested namespace; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            // degenerate cases
            if (requestedNamespace == null)
            {
                return false;
            }

            if (requestedNamespace.Length == 0)
            {
                return true;
            }

            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
                // looking for exact namespace match
                return string.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
            }
            
            // looking for exact or sub-namespace match
            requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
            if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (requestedNamespace.Length == targetNamespace.Length)
            {
                // exact match
                return true;
            }

            if (targetNamespace[requestedNamespace.Length] == '.')
            {
                // good prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar.Baz"
                return true;
            }
            
            // bad prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar2"
            return false;
        }
    }
}
