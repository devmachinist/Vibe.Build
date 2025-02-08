using Microsoft.Build.Framework;
using System;
using System.IO;

namespace Vibe.Build
{
    public class CompileCsxTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string ProjectDirectory { get; set; }

        [Output]
        public ITaskItem[] GeneratedSyntaxTrees { get; private set; }

        public override bool Execute()
        {
            try
            {
                Console.WriteLine($"Starting compilation of .csx files in {ProjectDirectory}...");

                var scripting = new Scripting(new System.Dynamic.ExpandoObject())
                {
                    _projectDirectory = ProjectDirectory
                };

                GeneratedSyntaxTrees = scripting.CompileCsxFiles("");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

    }
    public class DeleteFolderTask : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// The full path of the folder to delete.
        /// </summary>
        [Required]
        public string FolderPath { get; set; }

        public override bool Execute()
        {
            if (Directory.Exists(FolderPath))
            {
                try
                {
                    // Remove the read-only attribute from all files within the folder.
                    foreach (string file in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }

                    // Delete the folder and all its contents.
                    Directory.Delete(FolderPath, recursive: true);
                    Log.LogMessage(MessageImportance.High, $"Vibe Wiped: {FolderPath}");
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex, showStackTrace: true);
                    return false; // Signal failure to MSBuild.
                }
            }
            else
            {
                Log.LogMessage(MessageImportance.Low, $"Folder not found: {FolderPath}");
            }

            return true;
        }
    }
}
