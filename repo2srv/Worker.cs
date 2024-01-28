using System.IO.Compression;

namespace repo2srv
{
    internal class Worker
    {
        IEnumerable<AppSetting> settings = null;
        
        public Worker(IEnumerable<AppSetting> settings)
        {
            this.settings = settings;
        }

        public bool Prepare()
        {
            bool valid = true;
            foreach (var setting in settings)
            {
                if (!valid)
                {
                    break;
                }

                if (!Utils.IsDirectoryExists(setting.SourcePath))
                {   
                    valid = false;
                    Console.WriteLine($"{setting.SourcePath} not found");
                    break;
                }

                foreach (var item in setting.DestinationPaths)
                {
                    if (!Utils.IsDirectoryExists(item))
                    {
                        if (!Utils.CreateDirectory(item))
                        {
                            valid = false;
                            Console.WriteLine($"cant create directory for path: {item}");
                            break;                        }
                        
                    }
                }
                
                if (!valid)
                {
                    //cleanup
                    var files = Directory.EnumerateFiles(setting.SourcePath);
                    foreach (var item in settings)
                    {
                        item.Actions.ForEach(p =>
                        {
                            p.RemoveFiles.ForEach(c => {
                                if (files.Any(c => c.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    files.Where(c => c.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(c => 
                                    {
                                        Utils.DeleteFile(c);
                                        Console.WriteLine($"File deleted {c}");
                                    });
                                }
                            });
                        });
                    }
                }
            }
            
            return valid;
        }
    
        public bool Compress() 
        {
            var valid = true;

            foreach (var item in settings)
            {
                string zipFIleName = $"{item.Name}.zip";
                Utils.Compress(item.SourcePath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zipFIleName));                                
            }

            return valid;
        }

        public bool CopyToDest()
        {
            foreach (var item in settings)
            {
                string zipFIleName = $"{item.Name}.zip";

                item.DestinationPaths.ForEach(p =>
                {
                    Utils.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zipFIleName), Path.Combine(p, zipFIleName));
                    Console.WriteLine($"{zipFIleName} copied to: {p}");
                });
            }

            return true;
        }
    }

    internal static class Utils
    {
        internal static bool IsDirectoryExists(string path) 
        {
            return Directory.Exists(path);
        }

        internal static bool CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);

            return true;
        }

        internal static bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        internal static bool DeleteFile(string path)
        {
            if (IsFileExists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        internal static bool Compress(string path, string destination)
        {
            if (Utils.IsFileExists(destination))
            {
                Utils.DeleteFile(destination);
            }

            ZipFile.CreateFromDirectory(path, destination);

            return true;
        }

        internal static bool Copy(string zipFile, string destination)
        {
            File.Copy(zipFile, destination, true);
            Console.WriteLine($"File copy succeded to {destination}");
            return true;
        }
    }
}
