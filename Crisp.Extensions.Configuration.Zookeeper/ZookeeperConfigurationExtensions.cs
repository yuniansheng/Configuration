using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// zookeeper configuration provider extensions
    /// </summary>
    public static class ZookeeperConfigurationExtensions
    {
        /// <summary>
        /// use zookeeper as configuration source
        /// </summary>
        /// <param name="builder">ConfigurationBuilder</param>
        /// <param name="connectionString">the zookeeper connection string</param>
        /// <param name="rootPath">the zookeeper node path which you want to read it's sub node as key-value</param>
        /// <param name="timeout">zookeeper connection timeout in millsecond</param>
        /// <remarks>
        /// for example:
        /// you have these nodes in zookeeper
        /// 
        /// /AccountApp/Rate/USD=6.35
        /// /AccountApp/Rate/HKD=0.87
        /// /AccountApp/AccountChangeNotificationMethod=Email
        /// /OrderApp/Payment/Timeout=30
        /// 
        /// if you are develop the AccountApp,you may use this package like this:
        /// <code>
        /// builder.AddZookeeper("localhost:2181", "/AccountApp", 3000);
        /// var configuration = builder.Build();
        /// var usdRate = configuration["Rate:USD"];    //6.35
        /// var hkdRate = configuration["Rate:HKD"];    //0.87
        /// var hkdRate = configuration["AccountChangeNotificationMethod"];    //Email
        /// </code>
        /// 
        /// somebody else may response for the OrderApp,they can use this package like this:
        /// <code>
        /// builder.AddZookeeper("localhost:2181", "/OrderApp", 3000);
        /// var configuration = builder.Build();
        /// var paymentTimeoutMinutes = configuration["Payment:Timeout"];    //30
        /// </code>
        /// 
        /// as you see,the configuration key is the whole node path in zookeeper subtract the value of 
        /// {rootPath} paramteter,and the path dlimiter "/" in zookeeper will be replace with
        /// Microsoft.Extensions.Configuration.ConfigurationPath.KeyDelimiter( default is ":")
        /// you can also add multi zookeeper source,just like this:
        /// <code>
        /// builder
        ///     .AddZookeeper("localhost:2181", "/AccountApp", 3000)
        ///     .AddZookeeper("localhost:2181", "/OrderApp", 3000);
        /// </code>
        /// </remarks>
        public static void AddZookeeper(this IConfigurationBuilder builder,
            string connectionString, string rootPath, int timeout)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var source = new ZookeeperConfigurationSource()
            {
                ConnectionString = connectionString,
                RootPath = rootPath,
                SessionTimeout = timeout
            };
            builder.Add(source);
        }
    }
}
