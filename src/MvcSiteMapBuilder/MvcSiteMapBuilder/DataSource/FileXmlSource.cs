using System;
using System.IO;
using System.Xml.Linq;

namespace Mvc5SiteMapBuilder.DataSource
{
    /// <summary>
    /// Provides an Xml stream instance based on an XML file source.
    /// </summary>
    public class FileXmlSource : ISiteMapXmlDataSource
    {
        protected readonly string fileName;

        /// <summary>
        /// Creates a new instance of FileXmlSource.
        /// </summary>
        /// <param name="fileName">The absolute path to the Xml file.</param>
        public FileXmlSource(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            this.fileName = fileName;
        }

        public XDocument GetSiteMapData()
        {
            return (XDocument)((ISiteMapDataSource)this).GetSiteMapData();
        }

        object ISiteMapDataSource.GetSiteMapData()
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"SiteMap file: {fileName} was not found.");

            return XDocument.Load(fileName);
        }
    }
}
