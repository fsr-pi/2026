using Microsoft.Extensions.Configuration;

namespace Logging;

/// <summary>
/// Settings of the DataLoader. The class fills its own properties from the "Loader" section of the
/// configuration (appsettings.json, then user secrets, environment variables, command line...).
/// Try: dotnet run -- --Loader:MaxLines=2     or an environment variable   Loader__MaxLines=2
/// </summary>
public record LoaderSettings
{
  public int MaxLines { get; set; } = 100;
  public bool WarnOnInvalidLines { get; set; } = true;

  public LoaderSettings(IConfiguration configuration) => configuration.GetSection("Loader").Bind(this);
}
