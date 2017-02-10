using pb.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mega
{
    public class MegaDirectory
    {
        private Stack<string> _directory = new Stack<string>();

        public MegaDirectory(string directory = null)
        {
            if (directory != null)
                SetDirectory(directory);
        }

        public void SetDirectory(string directory)
        {
            if (directory.StartsWith("/"))
            {
                _directory = new Stack<string>();
                if (directory == "/")
                    return;
                directory = directory.Substring(1);
            }
            foreach (string directory2 in zsplit.Split(directory, '/'))
            {
                if (directory2 == "..")
                {
                    if (_directory.Count > 0)
                        _directory.Pop();
                }
                else
                    _directory.Push(directory2);
            }
        }

        public string GetDirectory()
        {
            if (_directory.Count == 0)
                return "/";
            StringBuilder sb = new StringBuilder();
            foreach (string directory in _directory.Reverse())
            {
                sb.Append("/");
                sb.Append(directory);
            }
            return sb.ToString();
        }
    }
}
