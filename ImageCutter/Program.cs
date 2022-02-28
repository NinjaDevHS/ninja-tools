using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using SixLabors.ImageSharp.Formats;
using System.Text.Json;

namespace MyApp
{
   public class Keyword
   {
      public string text { get; set; } = "";
      public string lang { get; set; } = "";
      public string category { get; set; } = "";
      public string bg { get; set; } = "";
      public string bgcol { get; set; } = "";
      public string template { get; set; } = "";
      public string font { get; set; } = "";
      public string color { get; set; } = "";
      public string border { get; set; } = "";
   }

   public class Program
   {
      public static void Main(string[] args)
      {
         string lang = "all";
         if (args.Length >= 1)
         {
            lang = args[0];
         }

         bool genImages = true;
         bool genText = true;
         bool genJson = true;
         if (args.Length >= 2)
         {
            if (args[1].ToLower().Equals("images"))
            {
               genText = false;
               genJson = false;
            }
            else if (args[1].ToLower().Equals("text"))
            {
               genImages = false;
               genJson = false;
            }
            else if (args[1].ToLower().Equals("json"))
            {
               genImages = false;
               genText = false;
            }
         }

         List<Keyword> keywords = JsonSerializer.Deserialize<List<Keyword>>(
             File.ReadAllText("input/" + lang + ".json"));

         // filter keywords by input language
         if (!lang.Equals("all"))
         {
            keywords = keywords.Where(k => k.lang.ToLower() == lang.ToLower()).ToList();
         }

         if (!Directory.Exists("output"))
            Directory.CreateDirectory("output");

         int i = 0;
         int j = 0;
         int imgSize = 1000;
         int imagesPerFile = 32;
         int sourceImageIndex = 1;

         var filename = $"{lang}-{sourceImageIndex}.png";
         Console.WriteLine($"Cutting {filename}");
         var image = Image.Load("input/" + filename);

         // cut images and generate their metadata file            
         foreach (var k in keywords)
         {
            if (j == imagesPerFile)
            {
               sourceImageIndex++;
               image.Dispose();
               filename = $"{lang}-{sourceImageIndex}.png";
               Console.WriteLine($"Cutting {filename}");
               image = Image.Load("input/" + filename);
               j = 0;
            }

            Console.WriteLine($"Generating {i + 1}: {k.lang}-{k.text}");

            // image
            var clone = image.Clone(x => x.Resize(image.Width, image.Height));
            clone.Mutate(x => x.Crop(new Rectangle(0, j * imgSize, imgSize, imgSize)));
            //clone.SaveAsPng($"output/{i+1}_{sourceImageIndex}.{j+1}.png");
            string destImageFilePath = $"output/{k.lang}_{k.text}.png";
            if (File.Exists(destImageFilePath))
               destImageFilePath = $"output/{k.lang}_{k.text}-2.png";
            if (genImages)
               clone.SaveAsPng(destImageFilePath);

            // metadata 
            //File.WriteAllText($"output/{i+1}_{sourceImageIndex}.{j+1}.txt",
            string destMetaFilePath = $"output/{k.lang}_{k.text}.txt";
            if (File.Exists(destMetaFilePath))
               destMetaFilePath = $"output/{k.lang}_{k.text}-2.txt";

            // generate txt file
            if (genText)
            {
               File.WriteAllText(destMetaFilePath,
               $"Name={k.lang} / {k.text}\n" +
               $"Language={k.lang}\n" +
               $"Keyword={k.text}\n" +
               (k.category.Length > 0 ? $"Category={k.category}\n" : "") +
               $"Theme={GetThemeName(k.template)}\n" +
               (GetPatternName(k.bg).Length > 0 ? $"Pattern={GetPatternName(k.bg)}\n" : "") +
               $"Font={k.font}\n" +
               (GetTextColorName(k).Length > 0 ? $"Color={GetTextColorName(k)}\n" : "") +
               (GetBorderColorName(k).Length > 0 ? $"Border Color={GetBorderColorName(k)}\n" : "") +
               (GetBgColorName(k).Length > 0 ? $"Background Color={GetBgColorName(k)}\n" : ""));
            }

            // generate standard metadata json for smart contract
            string destJsonMetaFilePath = destMetaFilePath.Replace(".txt", ".json");
            if (genJson)
            {
               File.WriteAllText(destJsonMetaFilePath,
                   $"{{\n" +
                   $"\"description\": \"Ninja Developer Hacking Squad\",\n" +
                   $"\"image\": \"ipfs://_IMGFOLDER_/_IMGID_.png\",\n" +
                   $"\"name\": \"#_IMGID_\",\n" +
                   $"\"attributes\": [" +
                       $"{{ \"trait_type\": \"Keyword\", \"value\": \"{k.text}\" }},\n" +
                       $"{{ \"trait_type\": \"Language\", \"value\": \"{k.lang}\" }},\n" +
                       $"{{ \"trait_type\": \"Category\", \"value\": \"{k.category}\" }},\n" +
                       $"{{ \"trait_type\": \"Theme\", \"value\": \"{GetThemeName(k.template)}\" }},\n" +
                       $"{{ \"trait_type\": \"Pattern\", \"value\": \"{GetPatternName(k.bg)}\" }},\n" +
                       $"{{ \"trait_type\": \"Font\", \"value\": \"{k.font}\" }},\n" +
                       $"{{ \"trait_type\": \"Color\", \"value\": \"{GetTextColorName(k)}\" }},\n" +
                       $"{{ \"trait_type\": \"Border Color\", \"value\": \"{GetBorderColorName(k)}\" }},\n" +
                       $"{{ \"trait_type\": \"Background Color\", \"value\": \"{GetBgColorName(k)}\" }}\n" +
                   $"]\n" +
                   $"}}");
            }

            i++;
            j++;
         }

         Console.WriteLine($"DONE");
      }

      private static string GetThemeName(string s)
      {
         if (s == "1")
            return "Miami";
         else if (s == "2")
            return "Las Vegas";
         else if (s == "3")
            return "Paris";
         else if (s == "4")
            return "London";
         else if (s == "5")
            return "New York";
         else if (s == "6")
            return "San Francisco";

         throw new Exception("It shouldn't get here...");
      }

      private static string GetPatternName(string s)
      {
         if (s == "1")
            return "Squares";
         else if (s == "2")
            return "Triangles";
         else if (s == "3")
            return "Crosses";
         else if (s == "4")
            return "Cubes";
         else if (s == "")
            return "";

         throw new Exception("It shouldn't get here...");
      }

      private static string GetTextColorName(Keyword k)
      {
         if (k.template == "1")
            return GetColorName(k.border);
         else if (k.template == "3")
            return "Multiple";
         else if (k.template == "6")
            return "Transparent";
         else
            return GetColorName(k.color);
      }

      private static string GetBgColorName(Keyword k)
      {
         if (k.template == "3")
            return "Black";
         else
            return GetColorName(k.bgcol);
      }

      private static string GetBorderColorName(Keyword k)
      {
         if (k.template == "1" || k.template == "3" || k.template == "5")
            return "";
         else if (k.template == "6")
            return "Multiple";
         else
            return GetColorName(k.border);
      }

      private static string GetColorName(string s)
      {
         if (s == "#f09")
            return "Magenta";
         else if (s == "#00ffff")
            return "Cyan";
         else if (s == "#6600ff")
            return "Violet";
         else if (s == "#66ff33")
            return "Green";
         else if (s == "#ffff00")
            return "Yellow";
         else if (s == "#ff66ff")
            return "Pink";
         else if (s == "#ff9933")
            return "Orange";
         else if (s == "#ff3300")
            return "Red";
         else if (s == "#fff")
            return "White";
         else if (s == "#000")
            return "Black";
         else if (s == "#1a1a1a")
            return "Dark Grey";
         else if (s == "#000033")
            return "Dark Blue";
         else if (s == "#011c08")
            return "Dark Green";
         else if (s == "#2b0000")
            return "Dark Red";
         else if (s == "#26004d")
            return "Dark Violet";
         else if (s == "")
            return "";

         throw new Exception("It shouldn't get here...");
      }
   }
}