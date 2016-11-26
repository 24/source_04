using System;
using System.Collections.Generic;
using System.Linq;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Data.Mongo;
using System.Xml.Linq;
using pb;
using pb.Data;

namespace Download.Print
{
    static partial class WebData
    {
        public static void ManageDirectories(IEnumerable<string> sourceDirectories, string destinationDirectory, string bonusDirectory = null, bool usePrintDirectories = true,
            bool simulate = true, bool moveFiles = false, bool moveInfoFiles = false, Func<string, bool> directoryFilter = null)
        {
            PrintFileManager_v2 printFileManager = CreatePrintFileManager_v2(simulate: simulate, moveFiles: moveFiles, moveInfoFiles: moveInfoFiles);
            PrintDirectoryManager printDirectoryManager = CreatePrintDirectoryManager();
            foreach (string sourceDirectory in sourceDirectories)
            {
                foreach (EnumDirectoryInfo directory in CreatePrintDirectoryManager().GetDirectories(sourceDirectory, usePrintDirectories: usePrintDirectories))
                {
                    if (directoryFilter != null)
                    {
                        if (!directoryFilter(directory.SubDirectory))
                            continue;
                    }
                    printFileManager.ManageDirectory(directory.Directory, zPath.Combine(destinationDirectory, directory.SubDirectory), bonusDirectory);
                    //directory.Directory.zTrace();
                    //zPath.Combine(destinationDirectory, directory.SubDirectory).zTrace();
                    //"".zTrace();
                }
            }
        }

        public static void RenameDailyPrintFiles(string sourceDirectory, string destinationDirectory, string logFile, bool simulate = true, string parameters = null)
        {
            // parameters : (FindPrintManager) int version, bool dailyPrintManager, int gapDayBefore, int gapDayAfter
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            XElement xe = GetDownloadAutomateManagerConfig(GetTestValue(parameters2));
            int version = xe.zXPathValue("Version").zTryParseAs(6);
            FindPrintManager findPrintManager = FindPrintManagerCreator.Create(xe, parameters2, version);
            //PrintFileManager_v2.GetDailyPrintFiles(sourceDirectory).zRenameDailyPrintFiles(findPrintManager, destinationDirectory, simulate: simulate).zSave(logFile);

            string lastDirectory = null;
            PrintFileManager_v2.GetDailyPrintFiles(sourceDirectory).zGetRenameDailyPrintFilesInfos(findPrintManager)
                .Select(renamePrintFile =>
                {
                    if (!simulate)
                    {
                        string directory = zPath.GetDirectoryName(renamePrintFile.SourceFile);
                        if (directory != lastDirectory)
                        {
                            // remove empty directories
                            if (lastDirectory != null)
                                //zdir.DeleteEmptyDirectory(lastDirectory, deleteOnlySubdirectory: false);
                                zdir.DeleteEmptyDirectory(lastDirectory, recurse: true);
                            lastDirectory = directory;
                        }
                        if (renamePrintFile.RenameFile)
                            renamePrintFile.DestinationFile = PrintFileManager_v2.RenamePrintFile(renamePrintFile.SourceFile, zPath.Combine(destinationDirectory, renamePrintFile.File));
                    }
                    return renamePrintFile;
                }).zSave(logFile);
            // remove empty directories
            if (!simulate && lastDirectory != null)
                //zdir.DeleteEmptyDirectory(lastDirectory, deleteOnlySubdirectory: false);
                zdir.DeleteEmptyDirectory(lastDirectory, recurse: true);
        }

        public static PrintFileManager_v2 CreatePrintFileManager_v2(UncompressQueueManager uncompressManager = null, bool simulate = false, bool moveFiles = false, bool moveInfoFiles = false)
        {
            DownloadAutomateManagerCreator downloadAutomateManagerCreator = GetDownloadAutomateManagerCreator();
            if (uncompressManager == null)
                uncompressManager = downloadAutomateManagerCreator.CreateUncompressManager();
            RegexValuesList bonusDirectories = new RegexValuesList(XmlConfig.CurrentConfig.GetConfig("PrintList2Config").GetElements("BonusDirectories/Directory"), compileRegex: true);
            PrintFileManager_v2 printFileManager = new PrintFileManager_v2();
            printFileManager.Simulate = simulate;
            printFileManager.MoveFiles = moveFiles;
            printFileManager.MoveInfoFiles = moveInfoFiles;
            printFileManager.UncompressManager = uncompressManager;
            printFileManager.BonusDirectories = bonusDirectories;
            return printFileManager;
        }

        public static PrintDirectoryManager CreatePrintDirectoryManager()
        {
            return new PrintDirectoryManager(XmlConfig.CurrentConfig.GetConfig("PrintList2Config").GetValues("PrintDirectories/Directory").ToArray());
        }
    }
}
