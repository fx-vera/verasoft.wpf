using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VeraSoft.Wpf.Extensions;

/// <summary>
/// Class FilesManager
/// </summary>
namespace VeraSoft.Wpf.Utils
{
    /// <summary>
    /// Class to manage files, Copy, Delete, Tar, Untar
    /// </summary>
    public class FilesManager
    {
        static DateTime centuryBegin = new DateTime(2000, 1, 1);

        /// <summary>
        /// Gets the temporary path (and delete whatever was in it).
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetTempPath(string path)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), path);

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }

        /// <summary>
        /// Deletes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeletePath(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeletePath(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Uns the tar.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="log">if set to <c>true</c> [log].</param>
        public static void UnTar(string source, string destination, bool log = false)
        {
            if (string.Equals(Path.GetExtension(source), ".tar", StringComparison.CurrentCultureIgnoreCase))
            {
                if (log)
                {
                    //ValidationLogService.SendLog("Unarchiving " + Path.GetFileName(source));
                }

                TarArchive tar = TarArchive.CreateInputTarArchive(new FileStream(source, FileMode.Open, FileAccess.Read));
                tar.ExtractContents(destination);
                tar.Close();
            }

            if (Directory.Exists(source))
            {
                foreach (string f in Directory.GetFiles(source))
                {
                    if (string.Equals(Path.GetExtension(f), ".tar", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (log)
                        {
                            //ValidationLogService.SendLog("Unarchiving " + Path.GetFileName(f));
                        }

                        TarArchive tar = TarArchive.CreateInputTarArchive(new FileStream(f, FileMode.Open, FileAccess.Read));
                        string tardestination = Path.Combine(destination, Path.GetFileNameWithoutExtension(f));
                        tar.ExtractContents(tardestination);
                        tar.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Tars the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="ignoredFiles">The ignored files.</param>
        public static void Tar(string source, string destination, List<string> ignoredFiles)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            string current = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(source);

            string tarname = Path.GetFileNameWithoutExtension(source) + ".tar";
            string destpath = Path.Combine(destination, tarname);

            //ValidationLogService.SendLog("Archiving " + Path.GetFileName(destpath));
            TarArchive tar = TarArchive.CreateOutputTarArchive(new FileStream(destpath, FileMode.Create, FileAccess.Write));
            foreach (string folder in Directory.GetDirectories(source))
            {
                TarEntry entry = TarEntry.CreateEntryFromFile(folder);

                //put +rwx for owner, group and other. As per https://www.gnu.org/software/tar/manual/html_node/Standard.html, 777 in Octal => 511 in Dec
                //#define TUREAD   00400          /* read by owner */
                //#define TUWRITE  00200          /* write by owner */
                //#define TUEXEC   00100          /* execute/search by owner */
                //#define TGREAD   00040          /* read by group */
                //#define TGWRITE  00020          /* write by group */
                //#define TGEXEC   00010          /* execute/search by group */
                //#define TOREAD   00004          /* read by other */
                //#define TOWRITE  00002          /* write by other */
                //#define TOEXEC   00001          /* execute/search by other */
                entry.TarHeader.Mode = 511;
                tar.WriteEntry(entry, true);
            }

            foreach (string file in Directory.GetFiles(source))
            {
                if (ignoredFiles.Contains(Path.GetFileName(file)))
                {
                    //ValidationLogService.SendLog("File ignored " + Path.GetFileName(file));
                    continue;
                }

                TarEntry entry = TarEntry.CreateEntryFromFile(file);
                entry.TarHeader.Mode = 511;
                tar.WriteEntry(entry, true);
            }

            tar.Close();
            Directory.SetCurrentDirectory(current);
        }

        /// <summary>
        /// Deletes the empty tar.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public static void DeleteEmptyTar(string destination)
        {
            List<string> toberemoved = new List<string>();

            foreach (var file in Directory.GetFiles(destination))
            {
                if (string.Equals(Path.GetExtension(file), ".tar", StringComparison.CurrentCultureIgnoreCase))
                {
                    var currentTar = SharpCompress.Archives.Tar.TarArchive.Open(file);
                    if (currentTar.Entries.Count == 0)
                    {
                        toberemoved.Add(file);
                    }
                    currentTar.Dispose();
                }
            }

            foreach (var file in toberemoved)
            {
                File.Delete(file);
                //ValidationLogService.SendLog("Deleting" + Path.GetFileName(file));
            }
        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                string newFile = newPath.Replace(source, destination);
                if (!File.Exists(newFile))
                    File.Copy(newPath, newFile);
            }
        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyFolderByDate(string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                DateTime date = File.GetCreationTime(newPath) < File.GetLastWriteTime(newPath) ? File.GetCreationTime(newPath) : File.GetLastWriteTime(newPath);
                CopyItemByDate(source, destination, date, newPath);
            }
        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="extension">The extension.</param>
        public static void CopyFolder(string source, string destination, List<string> extension)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string sourceFile in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                if (extension.Contains(Path.GetExtension(sourceFile), StringComparison.OrdinalIgnoreCase))
                {
                    string newFile = sourceFile.Replace(source, destination);
                    if (!File.Exists(newFile))
                        File.Copy(sourceFile, newFile);
                }
            }
        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyFolderByDate(string source, string destination, List<string> extension)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                DateTime date = File.GetCreationTime(newPath) < File.GetLastWriteTime(newPath) ? File.GetCreationTime(newPath) : File.GetLastWriteTime(newPath);
                if (extension.Contains(Path.GetExtension(newPath), StringComparison.OrdinalIgnoreCase))
                {
                    CopyItemByDate(source, destination, date, newPath);
                }
            }
        }

        private static void CopyItemByDate(string source, string dest, DateTime date, string file)
        {
            var year = date.Year;
            var month = date.Month;
            string yearPath = dest + "\\" + year;
            string monthPath = yearPath + "\\" + GetMonth(month);
            if (!Directory.Exists(yearPath))
            {
                Directory.CreateDirectory(yearPath + "\\");
            }
            if (!Directory.Exists(monthPath))
            {
                Directory.CreateDirectory(monthPath);
            }
            string newFile = monthPath + "\\" + Path.GetFileName(file);  // file.Replace(Replace(source, monthPath);

            if (!File.Exists(newFile))
            {
                File.Copy(file, newFile);
                Console.WriteLine("Copiado el archivo " + newFile);
            }
            else// si un fichero con el mismo nombre ya existe, crea un nombre nuevo, con prefijo igual y sufijo los ticks desde año 2000
            {
                long elapsedTicks = DateTime.Now.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                string newFileExisting = monthPath + "\\" + Path.GetFileNameWithoutExtension(file) + elapsedSpan.TotalSeconds.ToString() + Path.GetExtension(file);
                File.Copy(file, newFileExisting);
                Console.WriteLine("Ya existe el archivo " + newFile);
            }
        }

        private static string GetMonth(int month)
        {
            switch (month)
            {
                case 1: return CurrentDictionary.January;
                case 2: return CurrentDictionary.February;
                case 3: return CurrentDictionary.March;
                case 4: return CurrentDictionary.April;
                case 5: return CurrentDictionary.May;
                case 6: return CurrentDictionary.June;
                case 7: return CurrentDictionary.July;
                case 8: return CurrentDictionary.August;
                case 9: return CurrentDictionary.September;
                case 10: return CurrentDictionary.October;
                case 11: return CurrentDictionary.November;
                case 12: return CurrentDictionary.December;
                default: return string.Empty;
            }
        }

        public void DeleteEmptyFolders(string source, string destination)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }
        }

        private bool DeleteDirectorySync(string directory, int timeoutInMilliseconds = 5000)
        {
            if (!Directory.Exists(directory))
            {
                return true;
            }

            var watcher = new FileSystemWatcher
            {
                Path = Path.Combine(directory, ".."),
                NotifyFilter = NotifyFilters.DirectoryName,
                Filter = directory,
            };
            var task = Task.Run(() => watcher.WaitForChanged(WatcherChangeTypes.Deleted, timeoutInMilliseconds));

            // we must not start deleting before the watcher is running
            while (task.Status != TaskStatus.Running)
            {
                Thread.Sleep(100);
            }

            try
            {
                Directory.Delete(directory, true);
            }
            catch
            {
                return false;
            }

            return !task.Result.TimedOut;
        }

        /// <summary>
        /// Copies the source root files.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopySourceRootFiles(string source, string destination)
        {
            foreach (string file in Directory.GetFiles(source))
            {
                if (!string.Equals(Path.GetExtension(file), ".tar", StringComparison.CurrentCultureIgnoreCase))
                {
                    //NotificationManager.AddLog(Severity.INFO, "Unarchiving " + Path.GetFileName(file), "XML Import Export");
                    // ValidationLogService.SendLog("Unarchiving " + Path.GetFileName(file));
                    string destFile = Path.Combine(destination, Path.GetFileName(file));
                    if (!File.Exists(destFile))
                        File.Copy(file, destFile);
                    else// si un fichero con el mismo nombre ya existe, crea un nombre nuevo, con prefijo igual y sufijo los ticks desde año 2000
                    {
                        long elapsedTicks = DateTime.Now.Ticks - centuryBegin.Ticks;
                        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                        string newFileExisting = Path.GetFileNameWithoutExtension(file) + elapsedSpan.TotalSeconds.ToString() + Path.GetExtension(file);
                        string newDdestFile = Path.Combine(destination, Path.GetFileName(file));
                        File.Copy(file, newFileExisting);
                        Console.WriteLine("Ya existe el archivo " + newDdestFile);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the log.
        /// </summary>
        /// <param name="desPath">The DES path.</param>
        /// <param name="logName">Name of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public static void SaveLog(string desPath, string logName, string message, bool overwrite = true)
        {
            if (!Directory.Exists(desPath))
            {
                Directory.CreateDirectory(desPath);
            }
            string name = Path.Combine(desPath, logName);

            if (!string.IsNullOrEmpty(message))
            {
                while (message.First() == '\n' || message.First() == '\r')
                {
                    message = message.Substring(1);
                }
            }

            using (var tw = new StreamWriter(name, !overwrite))
            {
                tw.WriteLine(message);
            }
        }
    }
}