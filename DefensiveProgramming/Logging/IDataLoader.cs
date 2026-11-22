namespace Logging;

public interface IDataLoader
{
  List<(string, DateOnly)> LoadData(string filename);
}
