using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Mvc5SiteMapBuilder.Cache
{
    public class RuntimeFileCacheDependency : ICacheDependency
    {
        public RuntimeFileCacheDependency(
           string fileName
           )
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            this.fileName = fileName;
        }

        protected readonly string fileName;

        #region ICacheDependency Members

        public virtual object Dependency
        {
            get
            {
                var list = new List<ChangeMonitor>();
                list.Add(new HostFileChangeMonitor(new string[] { fileName }));
                return list;
            }
        }

        #endregion
    }
}
