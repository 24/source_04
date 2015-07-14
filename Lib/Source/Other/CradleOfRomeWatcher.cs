using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Ionic.Zip;
using pb.IO;

namespace PB_Library
{
    [Flags]
    public enum CradleOfRomeFileState
    {
        None = 0,
        ProfilesChange = 0x0001,
        ConfigChange = 0x0002
    }

    public enum CradleOfRomeBlitzLevelType
    {
        None = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3
    }

    // Warning CS0660 'PB_Library.CradleOfRomeBlitzLevel' defines operator == or operator != but does not override Object.GetHashCode()
    // Warning CS0661 'PB_Library.CradleOfRomeBlitzLevel' defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable 660, 661
    public struct CradleOfRomeBlitzLevel : IComparable<CradleOfRomeBlitzLevel>//, IEquatable<CradleOfRomeBlitzLevel>
    {
        public int EasyLevel;
        public int NormalLevel;
        public int HardLevel;
        public int FileNumber;

        public static bool operator ==(CradleOfRomeBlitzLevel level1, CradleOfRomeBlitzLevel level2)
        {
            if (level1.EasyLevel == level2.EasyLevel && level1.NormalLevel == level2.NormalLevel && level1.HardLevel == level2.HardLevel && level1.FileNumber == level2.FileNumber)
                return true;
            else
                return false;
        }

        public static bool operator !=(CradleOfRomeBlitzLevel level1, CradleOfRomeBlitzLevel level2)
        {
            if (level1.EasyLevel == level2.EasyLevel && level1.NormalLevel == level2.NormalLevel && level1.HardLevel == level2.HardLevel && level1.FileNumber == level2.FileNumber)
                return false;
            else
                return true;
        }

        public string GetFilename()
        {
            //CradleOfRome2_e0600-n2702-h0781_2.zip
            return string.Format("CradleOfRome2_e{0:0000}-n{1:0000}-h{2:0000}{3}.zip", EasyLevel, NormalLevel, HardLevel, FileNumber == 1 ? "" : string.Format("_{0}", FileNumber));
        }

        public int CompareTo(CradleOfRomeBlitzLevel other)
        {
            if (EasyLevel < other.EasyLevel)
                return -1;
            if (EasyLevel > other.EasyLevel)
                return 1;
            if (NormalLevel < other.NormalLevel)
                return -1;
            if (NormalLevel > other.NormalLevel)
                return 1;
            if (HardLevel < other.HardLevel)
                return -1;
            if (HardLevel > other.HardLevel)
                return 1;
            if (FileNumber < other.FileNumber)
                return -1;
            if (FileNumber > other.FileNumber)
                return 1;
            return 0;
        }
    }

    public delegate void NotifyDelegate(string message);
    public delegate void CraddleFilesSavedDelegate();

    public class CradleOfRomeWatcher
    {
        private static Regex _fileLevelsRegex = new Regex("CradleOfRome2_e([0-9]+)-n([0-9]+)-h([0-9]+)(?:_([0-9]+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string _cradleProgram = null;
        private string _cradleFileDirectory = null;
        private string _cradleFileSaveDirectory = null;
        private string _cradleFileArchiveDirectory = null;
        private string _cradleFileCopyDirectory = null;
        private DateTime _cradleLastFileCopiedTime = DateTime.MinValue;
        private CradleOfRomeBlitzLevelType _blitzLevelType = CradleOfRomeBlitzLevelType.None;
        private CradleOfRomeBlitzLevel _currentLevel = new CradleOfRomeBlitzLevel { EasyLevel = 0, NormalLevel = 0, HardLevel = 0, FileNumber = 1 };
        private CradleOfRomeBlitzLevel _lastLevel = new CradleOfRomeBlitzLevel { EasyLevel = 0, NormalLevel = 0, HardLevel = 0, FileNumber = 1 };
        private FileSystemWatcher _fileSystemWatcher = null;
        private CradleOfRomeFileState _fileState;
        private bool _fileWatcherRunning = false;
        public NotifyDelegate Notify;
        public CraddleFilesSavedDelegate CraddleFilesSaved;

        public CradleOfRomeWatcher()
        {
            //gCradleProgram = @"C:\Logiciel\Jeux\Cradle of Rome\Cradle Of Rome 2 v1.0.4.2014-okok\Cradle Of Rome 2\CradleOfRome2.exe";
            //gCradleProgram = @"C:\$pib\game\Cradle of Rome\Cradle Of Rome 2 v1.0.4.2014-okok\Cradle Of Rome 2\CradleOfRome2.exe";
            //gCradleProgram = @"C:\pib\game\Cradle of Rome\Cradle Of Rome 2 v1.0.4.2014-okok\Cradle Of Rome 2\CradleOfRome2.exe";
            _cradleProgram = @"C:\pib\prog\game\Cradle of Rome\Cradle Of Rome 2 v1.0.4.2014-okok\Cradle Of Rome 2\CradleOfRome2.exe";
            _cradleFileDirectory = @"c:\Users\Pierre\AppData\Roaming\Awem\CradleOfRome2";
            _cradleFileSaveDirectory = @"c:\Users\Pierre\AppData\Roaming\Awem\_CradleOfRome2\_copy\_06_v1.0.4.2014";
            _cradleFileArchiveDirectory = @"c:\Users\Pierre\AppData\Roaming\Awem\_CradleOfRome2\_copy\_06_v1.0.4.2014\a";
            _cradleFileCopyDirectory = @"c:\pib\drive\google\dev_data\exe\CradleOfRome\save";
            GetLevelsFromSaveDirectory();
            _lastLevel = _currentLevel;
        }

        // Cradle Of Rome 2
        //   changement de niveau : 
        //     file changed : 22/08/2012 14:20:03:0795718 name "profiles.xml"
        //     file changed : 22/08/2012 14:20:03:0825719 name "profiles.xml"
        //
        //   quitte Cradle Of Rome 2, sauve le niveau : 
        //     file changed : 22/08/2012 14:22:38:4164565 name "profiles.xml"
        //     file changed : 22/08/2012 14:22:38:4254570 name "profiles.xml"
        //     file changed : 22/08/2012 14:22:38:4394578 name "config.xml"
        //     file changed : 22/08/2012 14:22:38:4424580 name "config.xml"

        public string CradleProgram { get { return _cradleProgram; } set { _cradleProgram = value; } }
        public string CradleFileDirectory { get { return _cradleFileDirectory; } set { _cradleFileDirectory = value; } }
        public string CradleFileSaveDirectory { get { return _cradleFileSaveDirectory; } set { _cradleFileSaveDirectory = value; } }
        public string CradleFileArchiveDirectory { get { return _cradleFileArchiveDirectory; } set { _cradleFileArchiveDirectory = value; } }
        public CradleOfRomeBlitzLevelType BlitzLevelType { get { return _blitzLevelType; } set { _blitzLevelType = value; } }
        //public int EasyLevel { get { return gEasyLevel; } set { gEasyLevel = value; } }
        //public int NormalLevel { get { return gNormalLevel; } set { gNormalLevel = value; } }
        //public int HardLevel { get { return gHardLevel; } set { gHardLevel = value; } }
        //public int FileNumber { get { return gFileNumber; } set { gFileNumber = value; } }
        public CradleOfRomeBlitzLevel CurrentLevel { get { return _currentLevel; } }
        public CradleOfRomeBlitzLevel LastLevel { get { return _lastLevel; } set { _lastLevel = value; } }

        public void Start()
        {
            string filter = "*.xml";
            Stop();
            GetLevelsFromSaveDirectory();
            _lastLevel = _currentLevel;
            _fileState = CradleOfRomeFileState.None;
            _fileWatcherRunning = false;
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = _cradleFileDirectory;
            _fileSystemWatcher.Filter = filter;
            _fileSystemWatcher.Changed += new FileSystemEventHandler(FileChanged);
            _fileSystemWatcher.EnableRaisingEvents = true;
            Message("Craddle of rome 2 watcher started, blitz {0}, current level easy {1} normal {2} hard {3} number {4}", _blitzLevelType, _currentLevel.EasyLevel, _currentLevel.NormalLevel, _currentLevel.HardLevel, _currentLevel.FileNumber);
        }

        public void Stop()
        {
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _fileSystemWatcher.Changed -= new FileSystemEventHandler(FileChanged);
                _fileSystemWatcher = null;
                Message("Craddle of rome 2 watcher stoped");
            }
        }

        public bool IsStarted()
        {
            if (_fileSystemWatcher != null)
                return true;
            else
                return false;
        }

        private void StartFileWatcher()
        {
            if (!_fileWatcherRunning)
            {
                _fileWatcherRunning = true;
                ThreadStart threadStart = new ThreadStart(FileWatcher);
                Thread thread = new Thread(threadStart);
                thread.Start();
            }
        }

        void FileWatcher()
        {
            //Message("FileWatcher");
            //Thread.Sleep(100);
            //Thread.Sleep(4000);
            DateTime dt = DateTime.Now;
            // wait for 1/10 sec
            while (DateTime.Now.Subtract(dt).TotalMilliseconds < 4000)
            {
                if ((_fileState & CradleOfRomeFileState.ConfigChange) == CradleOfRomeFileState.ConfigChange) break;
                Thread.Sleep(100);
            }
            bool profilesChange = (_fileState & CradleOfRomeFileState.ProfilesChange) == CradleOfRomeFileState.ProfilesChange;
            bool configChange = (_fileState & CradleOfRomeFileState.ConfigChange) == CradleOfRomeFileState.ConfigChange;
            //Message("FileWatcher profilesChange {0} configChange {1}", profilesChange, configChange);
            if (profilesChange)
            {
                SaveCraddleFiles(!configChange);
            }
            _fileState = CradleOfRomeFileState.None;
            _fileWatcherRunning = false;
        }

        void FileChanged(object sender, FileSystemEventArgs e)
        {
            // sender = FileSystemWatcher
            //wr.Print("file changed : {0} name \"{1}\"", GetTimeString(), e.Name);
            if (IsCraddleRunning())
            {
                //Message("file changed \"{0}\"", e.Name);
                StartFileWatcher();
                if (e.Name == "profiles.xml")
                    _fileState |= CradleOfRomeFileState.ProfilesChange;
                else if (e.Name == "config.xml")
                    _fileState |= CradleOfRomeFileState.ConfigChange;
            }
        }

        void SaveCraddleFiles(bool newLevel)
        {
            //CradleOfRome2_e0600-n2702-h0685_2.zip
            GetLevelsFromSaveDirectory();
            _lastLevel = _currentLevel;
            if (newLevel)
            {
                if (_blitzLevelType == CradleOfRomeBlitzLevelType.Easy)
                    _currentLevel.EasyLevel++;
                else if (_blitzLevelType == CradleOfRomeBlitzLevelType.Normal)
                    _currentLevel.NormalLevel++;
                else if (_blitzLevelType == CradleOfRomeBlitzLevelType.Hard)
                    _currentLevel.HardLevel++;
                else
                    //throw new Exception(string.Format("Error bad blitz level type \"{0}\"", gBlitzLevelType));
                    Message("Error bad blitz level type \"{0}\"", _blitzLevelType);
                _currentLevel.FileNumber = 1;
            }
            else
                _currentLevel.FileNumber++;
            //string zipFile = string.Format("CradleOfRome2_e{0:0000}-n{1:0000}-h{2:0000}{3}.zip", gEasyLevel, gNormalLevel, gHardLevel, gFileNumber == 1 ? "" : string.Format("_{0}", gFileNumber));
            string zipFile = _currentLevel.GetFilename();
            Message("save craddle files to \"{0}\"", zipFile);
            //////////throw new Exception("error test test test test test");
            zipFile = Path.Combine(_cradleFileSaveDirectory, zipFile);
            ZipFile zip = new ZipFile();
            if (!Directory.Exists(_cradleFileSaveDirectory)) Directory.CreateDirectory(_cradleFileSaveDirectory);
            zip.AddFile(Path.Combine(_cradleFileDirectory, "config.xml"), "");
            zip.AddFile(Path.Combine(_cradleFileDirectory, "hiscores.xml"), "");
            zip.AddFile(Path.Combine(_cradleFileDirectory, "profiles.xml"), "");
            zip.Save(zipFile);
            zip.Dispose();
            if (CraddleFilesSaved != null)
                CraddleFilesSaved();
            ArchiveOldLevels();
            //_cradleFileCopyDirectory
            if (newLevel)
                CopySavedFile(zipFile);
        }

        public static bool IsCraddleRunning()
        {
            if (GetProcessId("CradleOfRome2") == 0)
                return false;
            else
                return true;
        }

        public void RunCraddle()
        {
            Message("Start craddle of rome 2 ({0})", _cradleProgram);
            Process.Start(_cradleProgram);
        }

        public void RunCraddleLastLevel()
        {
            if (_currentLevel != _lastLevel)
            {
                DeleteFileLevel(_currentLevel);
                _currentLevel = _lastLevel;
            }
            Message("Start craddle of rome 2 {0}", _lastLevel.GetFilename());
            CopyFileLevel(_lastLevel);
            Process.Start(_cradleProgram);
        }

        public void DeleteCraddleLastLevel()
        {
            if (_currentLevel != _lastLevel)
            {
                DeleteFileLevel(_currentLevel);
            }
            DeleteFileLevel(_lastLevel);
            GetLevelsFromSaveDirectory();
            _lastLevel = _currentLevel;
        }

        private void DeleteFileLevel(CradleOfRomeBlitzLevel level)
        {
            Message("Delete craddle of rome 2 level {0}", level.GetFilename());
            File.Delete(Path.Combine(_cradleFileSaveDirectory, level.GetFilename()));
        }

        private void CopyFileLevel(CradleOfRomeBlitzLevel level)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            ZipFile zip = new ZipFile(Path.Combine(_cradleFileSaveDirectory, level.GetFilename()));
            //zip["config.xml"].Extract(gCradleFileDirectory, true);
            zip["config.xml"].Extract(_cradleFileDirectory, ExtractExistingFileAction.OverwriteSilently);
            //zip["hiscores.xml"].Extract(gCradleFileDirectory, true);
            zip["hiscores.xml"].Extract(_cradleFileDirectory, ExtractExistingFileAction.OverwriteSilently);
            //zip["profiles.xml"].Extract(gCradleFileDirectory, true);
            zip["profiles.xml"].Extract(_cradleFileDirectory, ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static int GetProcessId(string processName)
        {
            Process[] processList = Process.GetProcessesByName(processName);
            if (processList.Length == 0) return 0;
            return processList[0].Id;
        }

        private void GetLevelsFromSaveDirectory()
        {
            // CradleOfRome2_e0600-n2702-h0717.zip
            string[] files = Directory.GetFiles(_cradleFileSaveDirectory, "CradleOfRome2_*.zip", SearchOption.TopDirectoryOnly);
            //Regex rx = new Regex("CradleOfRome2_e([0-9]+)-n([0-9]+)-h([0-9]+)(?:_([0-9]+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            int maxEasyLevel = 0;
            int maxNormalLevel = 0;
            int maxHardLevel = 0;
            int maxFileNumber = 0;
            foreach (string file in files)
            {
                //Match match = rx.Match(file);
                //if (match.Success)
                //{
                //    int easyLevel = int.Parse(match.Groups[1].Value);
                //    int normalLevel = int.Parse(match.Groups[2].Value);
                //    int hardLevel = int.Parse(match.Groups[3].Value);
                //    int fileNumber = 1;
                //    if (match.Groups[4].Value != "")
                //        fileNumber = int.Parse(match.Groups[4].Value);

                int easyLevel, normalLevel, hardLevel, fileNumber;
                if (GetFileLevels(file, out easyLevel, out normalLevel, out hardLevel, out fileNumber))
                {
                    if (easyLevel > maxEasyLevel || normalLevel > maxNormalLevel || hardLevel > maxHardLevel
                        || (easyLevel == maxEasyLevel && normalLevel == maxNormalLevel && hardLevel == maxHardLevel && fileNumber > maxFileNumber))
                    {
                        maxEasyLevel = easyLevel;
                        maxNormalLevel = normalLevel;
                        maxHardLevel = hardLevel;
                        maxFileNumber = fileNumber;
                    }
                }
            }
            _currentLevel.EasyLevel = maxEasyLevel;
            _currentLevel.NormalLevel = maxNormalLevel;
            _currentLevel.HardLevel = maxHardLevel;
            _currentLevel.FileNumber = maxFileNumber;
        }

        private static bool GetFileLevels(string file, out int easyLevel, out int normalLevel, out int hardLevel, out int fileNumber)
        {
            easyLevel = 0;
            normalLevel = 0;
            hardLevel = 0;
            fileNumber = 0;
            Match match = _fileLevelsRegex.Match(file);
            if (!match.Success)
                return false;
            easyLevel = int.Parse(match.Groups[1].Value);
            normalLevel = int.Parse(match.Groups[2].Value);
            hardLevel = int.Parse(match.Groups[3].Value);
            fileNumber = 1;
            if (match.Groups[4].Value != "")
                fileNumber = int.Parse(match.Groups[4].Value);
            return true;
        }

        private void ArchiveOldLevels()
        {
            string[] files = Directory.GetFiles(_cradleFileSaveDirectory, "CradleOfRome2_*.zip", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                int easyLevel, normalLevel, hardLevel, fileNumber;
                if (GetFileLevels(file, out easyLevel, out normalLevel, out hardLevel, out fileNumber))
                {
                    if (easyLevel < _currentLevel.EasyLevel - 2 || normalLevel < _currentLevel.NormalLevel - 2 || hardLevel < _currentLevel.HardLevel - 2)
                        File.Move(file, zpath.PathSetDirectory(file, _cradleFileArchiveDirectory));
                }
            }
        }

        private void CopySavedFile(string savedFile)
        {
            if (DateTime.Now.Subtract(_cradleLastFileCopiedTime).Days >= 1)
            {
                _cradleLastFileCopiedTime = DateTime.Now;
                zdir.CreateDirectory(_cradleFileCopyDirectory);
                File.Copy(savedFile, zpath.PathSetDirectory(savedFile, _cradleFileCopyDirectory));
            }
        }

        private void Message(string message, params object[] prm)
        {
            if (Notify != null)
            {
                if (prm.Length > 0)
                    message = string.Format(message, prm);
                Notify(message);
            }
        }
    }
}
