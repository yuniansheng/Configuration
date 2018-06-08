using Crisp.Extensions.Configuration.Zookeeper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddZookeeper(option =>
            {
                option.ConnectionString = "localhost:2181";
                option.ConnectionTimeout = 10000;
                option.RootPath = "/config";
                option.SessionTimeout = 3000;
            });
            var configuration = builder.Build();
            ChangeToken.OnChange(
                () => configuration.GetReloadToken(),
                () =>
                {
                    foreach (var item in configuration.AsEnumerable())
                    {
                        Console.WriteLine(item);
                    }
                });

            Console.ReadLine();
        }
    }
}
