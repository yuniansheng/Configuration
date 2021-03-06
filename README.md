Travis: [![Build Status](https://travis-ci.org/yuniansheng/Configuration.svg?branch=master)](https://travis-ci.org/yuniansheng/Configuration)

# Introduction
微软在Microsoft.Extensions.Configuration系列nuget包中提供了使用各种配置源的类库，包括使用Json文件作为配置源、使用XML文件、环境变更、命令行参数等。同时，作为应用程序使用配置的最佳实践，Microsoft.Extensions.Configuration.Abstraction定义了规范的API，遵循这些API使用配置非常方便。但是，微软提供的配置源总是有限的，像在分布式环境中，经常会使用zookeeper作为配置源，所以该项目目的是提供一些官方包之外的配置源,同时遵循最佳实践，方便使用方能够通过统一接口使用各种配置源。

# Usage
install the nuget package  
```Install-Package Crisp.Extensions.Configuration.Zookeeper```

assume you have these nodes in zookeeper  
/AccountApp/Rate/USD = 6.35  
/AccountApp/Rate/HKD = 0.87  
/AccountApp/AccountChangeNotificationMethod = Email  

```C#
IConfigurationBuilder builder = new ConfigurationBuilder();
builder.AddZookeeper(option =>
{
    option.ConnectionString = "localhost:2181";
    option.ConnectionTimeout = 10000;
    option.RootPath = "/AccountApp";
    option.SessionTimeout = 3000;
});
var configuration = builder.Build();

var usdRate = configuration["Rate:USD"];    //6.35
var hkdRate = configuration["Rate:HKD"];    //0.87
var notifyMethod = configuration["AccountChangeNotificationMethod"];    //Email
```