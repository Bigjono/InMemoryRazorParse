using System;
using System.IO;
using System.Linq;

namespace InMemoryRazorParsing.Tools
{
    public class ResourceFilesReader
    {
        public static string GetResourceFileContent(Type type, string filename)
        {

            var resourceNames = type.Assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames.Where(resourceName => resourceName.ToLower().EndsWith(filename.ToLower())))
            {
                return ResouceStreamToString(type, resourceName);
            }

            return "";
        }


        private static string ResouceStreamToString(Type type, string resourceName)
        {
            using (var stream = type.Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return "";
                using (var reader = new StreamReader(stream)) { return reader.ReadToEnd(); }
            }
        }


    }
}
