using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace ReleaseTool
{
    class Program
    {
        private static readonly string _InputDirectory;
        private static readonly string _OutputDirectory;
        private const string _OutputFile = "WarframeDPSTool.zip";
        private static readonly IReadOnlyCollection<string> _InputFiles = new string[]
        {
            "WarframeDPSTool.exe",
            "WFLib.dll",
            "*.ini"
        };

        static Program()
        {
            var ass = System.Reflection.Assembly.GetEntryAssembly();
            _OutputDirectory = Path.GetDirectoryName(ass.Location);
            _OutputDirectory = Path.GetDirectoryName(_OutputDirectory);
            _OutputDirectory = Path.GetDirectoryName(_OutputDirectory);
            _OutputDirectory = Path.GetDirectoryName(_OutputDirectory);
            _OutputDirectory += "\\";
            _InputDirectory = _OutputDirectory + "WarframeSnipers\\bin\\";
#if DEBUG
            _InputDirectory += "Debug";
#else
            _InputDirectory += "Release";
#endif
            _InputDirectory += "\\";
        }

        static void Main(string[] args)
        {
            try
            {
                using (FileStream fsOut = File.Create(_OutputDirectory + _OutputFile))
                using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
                    zipStream.Password = null;

                    foreach (string pattern in _InputFiles)
                    {
                        var files = Directory.GetFiles(_InputDirectory, pattern);
                        foreach (string filename in files)
                        {

                            FileInfo fi = new FileInfo(filename);

                            string entryName = filename.Substring(_InputDirectory.Length); // Makes the name in zip based on the folder
                            entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                            ZipEntry newEntry = new ZipEntry(entryName);
                            newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                            newEntry.Size = fi.Length;

                            zipStream.PutNextEntry(newEntry);

                            byte[] buffer = new byte[4096];
                            using (FileStream streamReader = File.OpenRead(filename))
                            {
                                StreamUtils.Copy(streamReader, zipStream, buffer);
                            }
                            zipStream.CloseEntry();
                        }
                    }

                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                var v = ex.Message;
                Debugger.Break();
            }
        }
    }
}