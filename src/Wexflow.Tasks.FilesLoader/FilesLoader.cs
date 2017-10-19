using System;
using Wexflow.Core;
using System.Xml.Linq;
using System.Threading;
using System.IO;

namespace Wexflow.Tasks.FilesLoader
{
    public class FilesLoader : Task
    {
        public string[] Folders { get; private set; }
        public string[] FlFiles { get; private set; }

        public FilesLoader(XElement xe, Workflow wf): base(xe, wf)
        {
            Folders = GetSettings("folder");
            FlFiles = GetSettings("file");
        }

        public override TaskStatus Run()
        {
            Info("Loading files...");

            bool success = true;

            try
            {
                foreach (string folder in Folders)
                {
                 
                    string myfolder = folder;
                    string pattern = "";   

                    if (!Directory.Exists(myfolder))
                    {                    
                        // If is not a folder assuming a pattern is needed
						pattern = myfolder.Substring(myfolder.LastIndexOf('\\') + 1);
						if (pattern != "")
						{
							myfolder = myfolder.Replace(pattern, "");
						}
                    }
                    
                    foreach (string file in Directory.GetFiles(folder, pattern))
                    {
                        var fi = new FileInf(file, Id);
                        Files.Add(fi);
                        InfoFormat("File loaded: {0}", file);
                    }
                }

                foreach (string file in FlFiles)
                {
                    if (File.Exists(file))
                    {
                        Files.Add(new FileInf(file, Id));
                        InfoFormat("File loaded: {0}", file);
                    }
                    else
                    {
                        ErrorFormat("File not found: {0}", file);
                        success = false;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while loading files.", e);
                success = false;
            }

            var status = Status.Success;

            if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status, false);

        }
    }
}
