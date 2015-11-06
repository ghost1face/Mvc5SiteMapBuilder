using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mvc5SiteMapBuilder.DataSource
{
    public class FileJSONSource : ISiteMapJSONDataSource
    {
        protected readonly string fileName;

        /// <summary>
        /// Creates a new instance of FileXmlSource.
        /// </summary>
        /// <param name="fileName">The absolute path to the Xml file.</param>
        public FileJSONSource(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            this.fileName = fileName;
        }

        public JObject GetSiteMapData()
        {
            return (JObject)((ISiteMapDataSource)this).GetSiteMapData();
        }

        object ISiteMapDataSource.GetSiteMapData()
        {
            // read JSON directly from a file
            var file = File.OpenText(fileName);
            using (var reader = new JsonTextReader(file))
            {
                return (JObject)JToken.ReadFrom(reader);
            }
        }
    }
}
