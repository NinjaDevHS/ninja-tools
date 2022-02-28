using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp
{
   public class MetadataAttribute
   {
      [JsonPropertyName("trait_type")]
      public string TraitType { get; set; }
      [JsonPropertyName("value")]
      public string Value { get; set; }
   }

   public class NFTMetadata
   {
      public List<MetadataAttribute> Attributes { get; set; }
   }
   public class Token
   {
      public string Id { get; set; }
      public string Keyword { get; set; }
      public string Language { get; set; }
      public string Category { get; set; }
      public string Theme { get; set; }
      public string Pattern { get; set; }
      public string Font { get; set; }
      public string Color { get; set; }
      public string BorderColor { get; set; }
      public string BackgroundColor { get; set; }
   }

   public class Program
   {
      public static void Main(string[] args)
      {
         List<Token> tokens = new List<Token>();
         int numFiles = Directory.GetFiles("input", "*.json").Length;

         for (int i = 1; i <= numFiles; i++)
         {
            var nftMetadata = JsonSerializer.Deserialize<NFTMetadata>(
               File.ReadAllText($"input/{i}.json"),
               new JsonSerializerOptions
               {
                  PropertyNameCaseInsensitive = true
               });

            tokens.Add(new Token
            {
               Id = i.ToString(),
               Keyword = GetAttributeValue(nftMetadata.Attributes, "Keyword"),
               Language = GetAttributeValue(nftMetadata.Attributes, "Language"),
               Category = GetAttributeValue(nftMetadata.Attributes, "Category"),
               Theme = GetAttributeValue(nftMetadata.Attributes, "Theme"),
               Pattern = GetAttributeValue(nftMetadata.Attributes, "Pattern"),
               Font = GetAttributeValue(nftMetadata.Attributes, "Font"),
               Color = GetAttributeValue(nftMetadata.Attributes, "Color"),
               BorderColor = GetAttributeValue(nftMetadata.Attributes, "Border Color"),
               BackgroundColor = GetAttributeValue(nftMetadata.Attributes, "Background Color")
            });
         }

         if (!Directory.Exists("output"))
            Directory.CreateDirectory("output");

         File.WriteAllText("output/tokens.json", JsonSerializer.Serialize(
            tokens, new JsonSerializerOptions
            {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
               WriteIndented = true
            }));

         Console.WriteLine("DONE");
      }

      private static string GetAttributeValue(List<MetadataAttribute> attributes, string attributeName)
      {
         var attr = attributes.SingleOrDefault(a => a.TraitType.Equals(attributeName));
         return attr != null ? attr.Value : "";
      }
   }
}