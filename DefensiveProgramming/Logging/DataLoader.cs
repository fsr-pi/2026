using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Logging;

public class DataLoader : IDataLoader
{
  private readonly ILogger<DataLoader> logger;
  private readonly LoaderSettings settings;

  // Both dependencies arrive from the container: ILogger<DataLoader> (from AddLogging)
  // and LoaderSettings (which itself needs IConfiguration)
  public DataLoader(ILogger<DataLoader> logger, LoaderSettings settings)
  {
    this.logger = logger;
    this.settings = settings;
  }

  public List<(string, DateOnly)> LoadData(string filename)
  {
    logger.LogInformation("Reading {FileName}", filename);
    List<(string, DateOnly)> list = new();
    if (!File.Exists(filename))
    {
      //message template with a named parameter (structured logging), not string interpolation
      logger.LogError("File {FileName} does not exist", filename);
      throw new FileNotFoundException($"File {filename} does not exist", filename);
    }
    else
    {
      //use File.ReadAllLines for small files
      using (var reader = new StreamReader(File.OpenRead(filename)))
      {
        string? line;
        int lineNumber = 0;
        while((line = reader.ReadLine()) != null) {
          if (++lineNumber > settings.MaxLines)
          {
            logger.LogWarning("Reading stopped after {MaxLines} lines", settings.MaxLines);
            break;
          }
          string pattern = "[0-3]?[0-9].[0-3]?[0-9].[0-9]{4}.";
          var match = Regex.Match(line, pattern);
          if (match.Success)
          {
            string datetext = line.Substring(match.Index, match.Length);
            if (DateOnly.TryParseExact(datetext, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
              var item = (line.Remove(match.Index, match.Length).Trim(), date);
              logger.LogTrace("Parsed {Name} with date {Date}", item.Item1, date);
              list.Add(item);
            }
            else if (settings.WarnOnInvalidLines)
            {
              logger.LogWarning("Invalid date in line: {Line}", line);
            }
          }
          else if (settings.WarnOnInvalidLines)
          {
            logger.LogWarning("No date found in line: {Line}", line);
          }
        }
      }
    }
    return list;
  }
}
