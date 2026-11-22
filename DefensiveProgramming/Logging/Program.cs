using Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

Console.WriteLine("=== 1) Without dependency injection (WithoutDI.cs)");
WithoutDiDemo.Run();

Console.WriteLine();
Console.WriteLine("=== 2) ServiceCollection built by hand, ILogger with NLog (BuildDI below)");
using (var serviceProvider = BuildDI())
{
  var dataLoader = serviceProvider.GetRequiredService<IDataLoader>();
  var list = dataLoader.LoadData("data.txt");
  var sortQuery = list.OrderByDescending(t => t.Item2)
                      .ThenBy(t => t.Item1);

  Console.WriteLine("Valid data: ");
  foreach (var item in sortQuery)
  {
    Console.WriteLine($"{item.Item2:yyyy-MM-dd} {item.Item1}");
  }
}

ServiceProvider BuildDI()
{
  var configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();

  IServiceCollection services = new ServiceCollection();
  var provider = services.AddLogging(configure =>
                          {
                            configure.AddConfiguration(configuration.GetSection("Logging"));
                            configure.AddConsole();
                            configure.AddNLog(new NLogProviderOptions { RemoveLoggerFactoryFilter = false });
                          })
                         .AddSingleton<IConfiguration>(configuration) 
                         .AddTransient<LoaderSettings>()
                         .AddTransient<IDataLoader, DataLoader>()
                         .BuildServiceProvider();
  return provider;
}
