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

                var solutionFile = SolutionFile.Parse(Path.GetFullPath(args[0]));
                var projList = solutionFile.ProjectsInOrder.ToList();
                var configList = new string[] {"Debug","Release"};
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
                            var cnt = helper.SetAllProperty("GenerateDebugInformation","false");
                            Console.WriteLine($"{prj.ProjectName} => {cnt} ");

                            if (cnt >0)
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
