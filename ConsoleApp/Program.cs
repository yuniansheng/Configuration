using Crisp.Extensions.Configuration.Zookeeper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.UseZookeeper("localhost:2181", "/config", 3000);
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

            foreach (var item in configuration.AsEnumerable())
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
