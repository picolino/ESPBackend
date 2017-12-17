#region Usings

using System.Web.Hosting;

#endregion

namespace Common
{
    public class PathNormalizer
    {
        public string Normalize(string relativePath)
        {
            var normalizedRelativePath = relativePath;
            if (!normalizedRelativePath.StartsWith("~/"))
            {
                normalizedRelativePath = $"~/{normalizedRelativePath}";
            }
            if (normalizedRelativePath.Contains("\\"))
            {
                normalizedRelativePath = normalizedRelativePath.Replace("\\", "/");
            }

            return HostingEnvironment.MapPath(normalizedRelativePath);
        }
    }
}