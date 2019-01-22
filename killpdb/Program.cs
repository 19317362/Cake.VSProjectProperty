using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.VSProjectProperty;
using Microsoft.Build.Construction;

namespace killpdb
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length<1)
            {
                Console.WriteLine("Usage:killpdb <something.sln>");
            }
            else
            {

                //https://stackoverflow.com/questions/707107/parsing-visual-studio-solution-files

                var solutionFile = SolutionFile.Parse(args[0]);
                var projList = solutionFile.ProjectsInOrder.ToList();
                var configList = new string[] { "Debug","Release"};
                foreach (var prj in projList)
                {
                    //<GenerateDebugInformation>true</GenerateDebugInformation>
                    try
                    {
                        bool changed = false;
                        var path = Path.GetExtension(prj.RelativePath);
                        if (path.Equals(".vcxproj"))
                        {
                            ProjectFileHelper helper = new ProjectFileHelper(prj.RelativePath);
                            foreach (var config in configList)
                            {
                                var prop = helper.GetProperty("GenerateDebugInformation", config,"Win32");
                                if (string.IsNullOrEmpty(prop))
                                {
                                    Console.WriteLine("Error GenerateDebugInformation not found");
                                }
                                if (prop != "false")
                                {
                                    helper.SetProperty("GenerateDebugInformation","false", config,"Win32");
                                    Console.WriteLine($"{prj.ProjectName} PDB was set to false");
                                    changed = true;
                                }
                                else
                                {
                                    Console.WriteLine($"{prj.ProjectName} PDB is false already");
                                }
                            }

                            if (changed)
                            {
                                helper.Save();
                            }
                        }
                        else
                        {

                        }


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }
            }
        }
    }
}
