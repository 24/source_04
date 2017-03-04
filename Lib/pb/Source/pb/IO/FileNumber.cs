using System.Text.RegularExpressions;
using pb.Text;

namespace pb.IO
{
    public enum FileNumberType
    {
        None = 0,
        Parenthesis,
        Bracket,
        Underscore
    }

    //public class FilenameNumberInfo
    public class FileNumber
    {
        // example      : c:\toto\tata(1).txt or c:\toto\tata[1].txt or c:\toto\tata_1.txt
        // BasePath     : c:\toto\tata.txt
        // BaseFilename : tata.txt
        // Number       : 1

        public string BasePath;
        public string BaseFilename;
        public int Number;
        public FileNumberType NumberType;

        public string GetPath(int number)
        {
            //return zPath.Combine(zPath.GetDirectoryName(BasePath), zPath.GetFileNameWithoutExtension(BaseFilename) + $"{FormatNumber(number, NumberType)}" + zPath.GetExtension(BaseFilename));
            return GetPath(FormatNumber(number, NumberType));
        }

        public string GetPath(string number)
        {
            return zPath.Combine(zPath.GetDirectoryName(BasePath), zPath.GetFileNameWithoutExtension(BaseFilename) + $"{number}" + zPath.GetExtension(BaseFilename));
        }

        public static string FormatNumber(int number, FileNumberType numberType)
        {
            switch (numberType)
            {
                case FileNumberType.Parenthesis:
                    return $"({number})";
                case FileNumberType.Bracket:
                    return $"[{number}]";
                case FileNumberType.Underscore:
                    return $"_{number}";
                default:
                    //throw new PBException($"unable to format number of type {numberType}");
                    return "";
            }
        }

        private static Regex _fileNumber = new Regex(@"(\(([0-9]+)\)|\[([0-9]+)\]|_([0-9]+))$", RegexOptions.Compiled);
        // used in PrintFileManager (PrintFileManager.cs)
        public static FileNumber GetFileNumber(string path)
        {
            //FilenameNumberInfo filenameNumberInfo = new FilenameNumberInfo();
            string filename = zPath.GetFileNameWithoutExtension(path);
            string ext = zPath.GetExtension(path);
            FileNumberType numberType;
            Match match = _fileNumber.Match(filename);
            if (match.Success)
            {
                //filenameNumberInfo.BaseFilename = match.zReplace(filename, "") + ext;
                string textNumber;
                if (match.Groups[2].Value != "")
                {
                    textNumber = match.Groups[2].Value;
                    numberType = FileNumberType.Parenthesis;
                }
                else if (match.Groups[3].Value != "")
                {
                    textNumber = match.Groups[3].Value;
                    numberType = FileNumberType.Bracket;
                }
                else
                {
                    textNumber = match.Groups[4].Value;
                    numberType = FileNumberType.Underscore;
                }
                //filenameNumberInfo.Number = int.Parse(number);
                int number;
                if (int.TryParse(textNumber, out number))
                {
                    // modif le 08/08/2015 ajout zPath.GetDirectoryName(path)
                    //return new FilenameNumberInfo { BaseFilename = zPath.Combine(zPath.GetDirectoryName(path), match.zReplace(filename, "").Trim()) + ext, Number = number };
                    filename = match.zReplace(filename, "").Trim() + ext;
                    return new FileNumber { BasePath = zPath.Combine(zPath.GetDirectoryName(path), filename), BaseFilename = filename, Number = number, NumberType = numberType };
                }
            }
            //else
            //{
            //    filenameNumberInfo.BaseFilename = filename + ext;
            //    filenameNumberInfo.Number = 0;
            //}
            //return filenameNumberInfo;
            // modif le 08/08/2015 ajout zPath.GetDirectoryName(path)
            //return new FilenameNumberInfo { BaseFilename = zPath.Combine(zPath.GetDirectoryName(path), filename.Trim()) + ext, Number = 0 };
            filename = filename.Trim() + ext;
            return new FileNumber { BasePath = zPath.Combine(zPath.GetDirectoryName(path), filename), BaseFilename = filename, Number = 0, NumberType = FileNumberType.Parenthesis };
        }

        public static string GetFileWithoutNumber(string path)
        {
            // modif le 08/08/2015 ajout zPath.GetDirectoryName(path)
            return zPath.Combine(zPath.GetDirectoryName(path), _fileNumber.Replace(zPath.GetFileNameWithoutExtension(path), "").Trim()) + zPath.GetExtension(path);
        }

        public static int GetNumber(string path)
        {
            Match match = _fileNumber.Match(zPath.GetFileNameWithoutExtension(path));
            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            else
                return 0;
        }
    }
}
