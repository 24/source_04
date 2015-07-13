using System;
using System.IO;
using System.Text.RegularExpressions;
using pb.Text;

namespace pb.IO
{
    public class FilenameNumberInfo
    {
        public string BaseFilename;
        public int Number;

        private static Regex __filenameNumber = new Regex(@"(\(([0-9]+)\)|\[([0-9]+)\]|_([0-9]+))$", RegexOptions.Compiled);
        // used in PrintFileManager (PrintFileManager.cs)
        public static FilenameNumberInfo GetFilenameNumberInfo(string path)
        {
            FilenameNumberInfo filenameNumberInfo = new FilenameNumberInfo();
            string filename = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            Match match = __filenameNumber.Match(filename);
            if (match.Success)
            {
                filenameNumberInfo.BaseFilename = match.zReplace(filename, "") + ext;
                string number;
                if (match.Groups[2].Value != "")
                    number = match.Groups[2].Value;
                else if (match.Groups[3].Value != "")
                    number = match.Groups[3].Value;
                else
                    number = match.Groups[4].Value;
                filenameNumberInfo.Number = int.Parse(number);
            }
            else
            {
                filenameNumberInfo.BaseFilename = filename + ext;
                filenameNumberInfo.Number = 0;
            }
            return filenameNumberInfo;
        }

        public static string GetFilenameWithoutNumber(string path)
        {
            return __filenameNumber.Replace(Path.GetFileNameWithoutExtension(path), "") + Path.GetExtension(path);
        }

        public static int GetFilenameNumber(string path)
        {
            Match match = __filenameNumber.Match(Path.GetFileNameWithoutExtension(path));
            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            else
                return 0;
        }
    }
}
