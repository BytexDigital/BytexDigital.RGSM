using System.IO;

namespace BytexDigital.RGSM.Node.Application.Helpers
{
    public static class PathExtensions
    {
        public static bool IsSubdirectoryOfOrMatches(this string path, string parent)
        {
            DirectoryInfo dir1 = new DirectoryInfo(parent);
            DirectoryInfo dir2 = new DirectoryInfo(path);

            if (dir1.FullName == dir2.FullName) return true;

            while (dir2.Parent != null)
            {
                if (dir2.Parent.FullName == dir1.FullName)
                {
                    return true;
                }
                else
                {
                    dir2 = dir2.Parent;
                }
            }

            return false;
        }
    }
}
