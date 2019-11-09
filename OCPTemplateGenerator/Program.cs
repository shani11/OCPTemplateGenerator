using System;
using System.IO;
using System.Text;

namespace OCPTemplateGenerator
{
    class Program
    {
        private static string ProjectName;
        private static string TestProjectName;
        private static string ProjectPath;
        private static string TestProjectPath;
        private static string ProjectPathcsproj;
        private static string TestProjectPathcsproj;


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the OCP Template Generator..");
            string path = string.Empty;
            int numberOfArguments = args.Length;

            if (numberOfArguments > 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("Plese enter the Solution path: ");
                path = Console.ReadLine();
            }
           
          
            if (!string.IsNullOrEmpty(path))
            {
                //Search .csproj file at the project path
                string[] files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

                //Get project path and Test Project csproj Path
                foreach (var item in files)
                {
                    StreamReader csprojreader = new StreamReader(item);
                    string readcsproj = csprojreader.ReadToEnd();
                    bool isTestProject = readcsproj.Contains("Test", StringComparison.CurrentCultureIgnoreCase);
                    if (isTestProject)
                    {
                        TestProjectPathcsproj = item;
                    }
                    else
                    {
                        ProjectPathcsproj = item;
                    }

                }

                //Get the project and test project name
                ProjectName = Path.GetFileNameWithoutExtension(ProjectPathcsproj);
                TestProjectName = Path.GetFileNameWithoutExtension(TestProjectPathcsproj);
                Console.WriteLine("-------------------------");
                Console.WriteLine("Project name is " + ProjectName);
                Console.WriteLine("NUnit Test project name is " + TestProjectName);
                Console.WriteLine("-------------------------");

                //Create OCP folder
                Console.WriteLine("Creating OCP Folder in solution directory");
                string ocpPath = path + "/OCP";
                if (!Directory.Exists(ocpPath))
                {
                    Directory.CreateDirectory(path + "/OCP");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("OCP Folder created successfully.");
                    Console.WriteLine("-------------------------");
                }
                else
                {
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("OCP folder already exists in your solution.");
                    Console.WriteLine("-------------------------");
                }
                //Now move ocp files from source to project root directory
                string sourcePth = @"D:\OCP";
                Console.WriteLine("Copying the OCP files from Source to Solution directory..");
                Copy(sourcePth, ocpPath);
                Console.WriteLine("-------------------------");
                Console.WriteLine("OCP files Copied Successfully from source to Solution Directory.");
                Console.WriteLine("-------------------------");
                // Now move openshift configuration file from source to project directory..
                string ocpConfigFile = "ocpConfiguration.txt";
                string sourcePath = @"D:\";
                string targetPath = path;
                // Use Path class to manipulate file and directory paths.
                string sourceFile = System.IO.Path.Combine(sourcePath, ocpConfigFile);
                string destFile = System.IO.Path.Combine(targetPath, ocpConfigFile);
                File.Copy(sourceFile, destFile, true);
                Console.WriteLine("-------------------------");
                Console.WriteLine("All set doing some final update as per project");
                Console.WriteLine("-------------------------");

                //Now read the file and make required changes as per the solution
                StreamReader reader = new StreamReader(path + @"\ocpConfiguration.txt");
                StringBuilder readedData = new StringBuilder();
                readedData.Append(reader.ReadToEnd());
                reader.Close();

                //Get the project and test project path
                ProjectPath = new DirectoryInfo(ProjectPathcsproj).Parent.ToString();
                TestProjectPath = new DirectoryInfo(TestProjectPathcsproj).Parent.ToString();
                //modify what you want
                readedData.Replace("projectname", ProjectName);
                readedData.Replace("testname", TestProjectName);
                readedData.Replace("projectpathcsproj", ProjectPathcsproj);
                readedData.Replace("testpathcsproj", TestProjectPathcsproj);
                readedData.Replace("projectpath", ProjectPath);
                readedData.Replace("testpath", TestProjectPath);

                //Write new file or append on existing file
                StreamWriter writer = new StreamWriter(path + @"\ocpConfiguration.txt", false);
                writer.Write(readedData);
                writer.Close();
                Console.WriteLine("-------------------------");
                Console.WriteLine("Done. OCP Configuration is completed for your Project "+ProjectName);
                Console.WriteLine("-------------------------");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Invalid Path..");
            }
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, ProjectName), true);
            }
            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }
}
