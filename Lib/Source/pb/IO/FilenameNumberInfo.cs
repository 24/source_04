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
            //FilenameNumberInfo filenameNumberInfo = new FilenameNumberInfo();
            string filename = zPath.GetFileNameWithoutExtension(path);
            string ext = zPath.GetExtension(path);
            Match match = __filenameNumber.Match(filename);
            if (match.Success)
            {
                //filenameNumberInfo.BaseFilename = match.zReplace(filename, "") + ext;
                string textNumber;
                if (match.Groups[2].Value != "")
                    textNumber = match.Groups[2].Value;
                else if (match.Groups[3].Value != "")
                    textNumber = match.Groups[3].Value;
                else
                    textNumber = match.Groups[4].Value;
                //filenameNumberInfo.Number = int.Parse(number);
                int number;
                if (int.TryParse(textNumber, out number))
                    return new FilenameNumberInfo { BaseFilename = match.zReplace(filename, "").Trim() + ext, Number = number };
            }
            //else
            //{
            //    filenameNumberInfo.BaseFilename = filename + ext;
            //    filenameNumberInfo.Number = 0;
            //}
            //return filenameNumberInfo;
            return new FilenameNumberInfo { BaseFilename = filename.Trim() + ext, Number = 0 };
        }

        public static string GetFilenameWithoutNumber(string path)
        {
            return __filenameNumber.Replace(zPath.GetFileNameWithoutExtension(path), "").Trim() + zPath.GetExtension(path);
        }

        public static int GetFilenameNumber(string path)
        {
            Match match = __filenameNumber.Match(zPath.GetFileNameWithoutExtension(path));
            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            else
                return 0;
        }
    }
}
