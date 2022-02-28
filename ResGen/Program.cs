using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MyApp
{
   public class Program
   {
      public static void Main(string[] args)
      {
         if (args.Length == 0)
            GenerateResources();
         else
            ReplaceImageFolder(args[0]);
      }

      private static void ReplaceImageFolder(string folderName)
      {
         Console.WriteLine($"SETTING FOLDER NAME");
         string[] jsonFiles = Directory.GetFiles("output/metadata").OrderBy(f => f).ToArray();
         foreach (string f in jsonFiles)
         {
            Console.WriteLine(f);            
            File.WriteAllText(f, File.ReadAllText(f).Replace("_IMGFOLDER_", folderName));
         }
         Console.WriteLine($"DONE");
      }

      private static void GenerateResources()
      {
         Console.WriteLine($"GENERATING RESOURCES");

         string[] jsonFiles = Directory.GetFiles("input", "*.json").OrderBy(f => f).ToArray();
         Random random = new Random();
         jsonFiles = jsonFiles.OrderBy(x => random.Next()).ToArray();

         if (!Directory.Exists("output/metadata"))
            Directory.CreateDirectory("output/metadata");
         if (!Directory.Exists("output/images"))
            Directory.CreateDirectory("output/images");

         for (int i = 1; i <= jsonFiles.Length; i++)
         {
            string f = jsonFiles[i - 1];
            Console.WriteLine($"Processing file #{i}: {f}");
            // write metadata file
            File.WriteAllText($"output/metadata/{i}.json",
                File.ReadAllText(f)
                  .Replace("_IMGID_", i.ToString())
                  .Replace("Test Collection", "Ninja Developer Hacking Squad")
                  .Replace("Test Token", "")
            );
            // copy image file
            File.Copy(f.Replace(".json", ".png"), $"output/images/{i}.png");
         }
         Console.WriteLine($"DONE");
      }
   }

}