namespace YeGods.Importer
{
  using Common.Enums;
  using Common.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using DataAccess;
  using Domain;

  class Program
  {
    private static readonly TextInfo TextInfo = new CultureInfo("en-GB", false).TextInfo;

    static void Main(string[] args)
    {
      List<Deity> deities = new List<Deity>();

      string folderPath = "File Imports/";

      foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
      {
        if (File.Exists(file))
        {
          using (StreamReader streamReader = new StreamReader(file))
          {
            while (!streamReader.EndOfStream)
            {
              string rawName = streamReader.ReadLine();
              string rawOrigin = streamReader.ReadLine();
              string rawSex = streamReader.ReadLine();
              string rawCategory = streamReader.ReadLine();
              string rawAliases = streamReader.ReadLine();
              string rawDescription = GetDescription(streamReader);

              Deity deity = new Deity();
              deity.Name = ProcessName(rawName);
              deity.Origin = ProcessOrigin(rawName, rawOrigin);
              deity.Sex = ProcessSex(rawName, rawSex);
              deity.CategoryId = ProcessCategory(rawName, rawCategory);
              deity.Aliases = ProcessAliases(rawAliases);
              deity.Description = rawDescription;
              deity.CreatedAt = DateTime.UtcNow;
              deity.Slugs = ProcessSlugs(deity.Name, deity.Aliases);

              deities.Add(deity);
              Console.WriteLine($"Parsed deity: {deity.Name}.");
            }
          }
        }
      }

      List<Glossary> glossaries = new List<Glossary>();

      string glossaryFile = "Glossary/Glossary.txt";

      if (File.Exists(glossaryFile))
      {
        using (StreamReader streamReader = new StreamReader(glossaryFile))
        {
          while (!streamReader.EndOfStream)
          {
            string rawName = streamReader.ReadLine();
            string rawOrigin = streamReader.ReadLine();
            string rawDescription = GetDescription(streamReader);

            Glossary glossary = new Glossary();
            glossary.Name = ProcessName(rawName);
            glossary.Origin = ProcessOrigin(glossary.Name, rawOrigin);
            glossary.Description = rawDescription;
            glossary.CreatedAt = DateTime.UtcNow;

            glossaries.Add(glossary);
            Console.WriteLine($"Parsed glossary: {glossary.Name}.");
          }
        }
      }

      SaveRecordsToDatabase(
        deities,
        glossaries);
    }

    private static void SaveRecordsToDatabase(
      List<Deity> deities,
      List<Glossary> glossaries)
    {
      using (YeGodsContext context = new YeGodsContextFactory().CreateDbContext())
      {
        context.Deities.AddRange(deities);
        context.Glossaries.AddRange(glossaries);
        context.SaveChanges();
      }
    }

    private static int ProcessCategory(string name, string rawCategory)
    {
      if (rawCategory.Contains("Category"))
      {
        string categoryName = rawCategory?.Trim().Replace("Category: ", string.Empty).Trim();

        using (YeGodsContext context = new YeGodsContextFactory().CreateDbContext())
        {
          Category category = context
            .Categories
            .FirstOrDefault(c => c.Name.ToLower() == categoryName.ToLower());

          if (category != null)
          {
            return category.Id;
          }

          Category newCategory = new Category();
          newCategory.Name = categoryName;
          newCategory.CreatedAt = DateTime.UtcNow;
          context.Categories.Add(newCategory);
          context.SaveChanges();
          return newCategory.Id;
        }
      }

      throw new Exception("Parser was expecting Category.");
    }

    private static ICollection<DeitySlug> ProcessSlugs(string name, ICollection<DeityAlias> aliases)
    {
      List<DeitySlug> slugs = new List<DeitySlug>();

      if (name != null)
      {
        DeitySlug deitySlug = new DeitySlug();
        deitySlug.Name = name.NormalizeStringForUrl();
        deitySlug.CreatedAt = DateTime.UtcNow;
        deitySlug.IsDefault = true;
        slugs.Add(deitySlug);
      }

      foreach (DeityAlias alias in aliases)
      {
        DeitySlug deitySlug = new DeitySlug();
        deitySlug.Name = alias.Name.NormalizeStringForUrl();
        deitySlug.CreatedAt = DateTime.UtcNow;
        deitySlug.IsDefault = false;
        slugs.Add(deitySlug);
      }

      return slugs;
    }

    private static string GetDescription(StreamReader streamReader)
    {
      string streamReadValue;
      string description = "";

      while (!string.IsNullOrWhiteSpace(streamReadValue = streamReader.ReadLine()))
      {
        description += streamReadValue;
      }
      return description;
    }

    private static Sex ProcessSex(string name, string rawSex)
    {
      if (rawSex.Contains("Sex"))
      {
        return DetermineSex(rawSex?.Trim().Replace("Sex: ", string.Empty).ToLower());
      }

      throw new Exception("Parser was expecting Sex.");
    }

    private static string ProcessOrigin(string name, string rawOrigin)
    {
      if (rawOrigin.Contains("Origin"))
      {
        return rawOrigin.Trim().Replace("Origin: ", string.Empty).Trim();
      }

      throw new Exception("Parser was expecting Origin.");
    }

    private static string ProcessName(string rawName)
    {
      return TextInfo.ToTitleCase(rawName.Trim().ToLower());
    }

    private static ICollection<DeityAlias> ProcessAliases(string rawAliases)
    {
      List<DeityAlias> aliases = new List<DeityAlias>();

      string aliasesString = rawAliases.Replace("Aliases: ", string.Empty);

      if (aliasesString != "None")
      {
        List<string> aliasStrings = new List<string>(aliasesString?.Split(','));

        foreach (string aliasString in aliasStrings)
        {
          DeityAlias deityAlias = new DeityAlias();
          deityAlias.Name = TextInfo.ToTitleCase(aliasString.ToLower().Trim());
          deityAlias.CreatedAt = DateTime.UtcNow;
          aliases.Add(deityAlias);
        }
      }

      return aliases;
    }

    private static Sex DetermineSex(string sex)
    {
      string female = "f";
      string male = "m";
      if (sex == female)
      {
        return Sex.Female;
      }
      else if (sex == male)
      {
        return Sex.Male;
      }
      else
      {
        return Sex.None;
      }
    }
  }
}
