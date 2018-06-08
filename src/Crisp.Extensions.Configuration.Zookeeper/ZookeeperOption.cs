using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// the zookeeper connection option
    /// </summary>
    public class ZookeeperOption
    {
        /// <summary>
        /// the zookeeper connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// the zookeeper node path which you want to read it's sub node as key-value
        /// </summary>
        /// <remarks>
        /// for example:
        /// you have these nodes in zookeeper
        /// 
        /// /AccountApp/Rate/USD=6.35
        /// /AccountApp/Rate/HKD=0.87
        /// /AccountApp/AccountChangeNotificationMethod=Email
        /// /OrderApp/Payment/Timeout=30
        /// 
        /// if you set RootPath to "AccountApp",you may get values like this:
        /// <code>
        /// var usdRate = configuration["Rate:USD"];    //6.35
        /// var hkdRate = configuration["Rate:HKD"];    //0.87
        /// var notifyMethod = configuration["AccountChangeNotificationMethod"];    //Email
        /// </code>
        /// 
        /// somebody else may response for the development of OrderApp,they may set RootPath to "OrderApp",and the will get values like this:
        /// <code>
        /// var paymentTimeoutMinutes = configuration["Payment:Timeout"];    //30
        /// </code>
        /// 
        /// as you see,the configuration key is the whole node path in zookeeper subtract the value of 
        /// RootPath Property,and the path dlimiter "/" in zookeeper will be replace with
        /// Microsoft.Extensions.Configuration.ConfigurationPath.KeyDelimiter( default is ":")
        /// you can also add multi zookeeper source,just like this:
        /// <code>
        /// builder
        ///     .AddZookeeper(options => { options.RootPath = "AccountApp" } )
        ///     .AddZookeeper(options => { options.RootPath = "OrderApp" } );
        /// </code>
        /// </remarks>
        public string RootPath { get; set; }

        /// <summary>
        /// zookeeper connection timeout in millsecond,exception will throwed when timeout,default 20000
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// the zookeeper session timeout in millsecond
        /// </summary>
        public int SessionTimeout { get; set; }

        /// <summary>
        /// the default option
        /// </summary>
        public static ZookeeperOption Default
        {
            get
            {
                return new ZookeeperOption()
                {
                    RootPath = "/",
                    ConnectionTimeout = 20000
                };
            }
        }
    }
}
