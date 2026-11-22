using System.Globalization;

namespace Logging;

/// <summary>A concrete logger that writes to a file.</summary>
public class FileLogger(string path)
{
  public void Info(string message) =>
    File.AppendAllText(path, $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} {message}{Environment.NewLine}");
}

/// <summary>
/// Before: the class creates its dependency itself (new).
/// - replacing the logger means changing DataLoaderBefore,
/// - a test writes to the real file.
/// After: see DataLoader.cs, where ILogger&lt;DataLoader&gt; arrives through the constructor.
/// </summary>
public class DataLoaderBefore
{
  private readonly FileLogger logger = new FileLogger("log.txt");

  public List<string> Load(string path)
  {
    logger.Info("Reading " + path);
    return File.ReadAllLines(path).ToList();
  }
}

public static class WithoutDiDemo
{
  public static void Run()
  {
    var before = new DataLoaderBefore();
    Console.WriteLine($"   loaded {before.Load("data.txt").Count} lines (and wrote to log.txt)");
  }
}
