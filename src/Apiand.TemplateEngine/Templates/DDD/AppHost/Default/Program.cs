using Microsoft.Extensions.Configuration;
using XXXnameXXX.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var dbType = builder.Configuration.GetValue<string>("DatabaseSettings:DbType");

switch (dbType)
{
    case "mongodb":
        builder.AddMongo();
        break;
    case "sqlserver":
        builder.AddSqlServer();
        break;
    case "postgres":
        builder.AddPostgres();
        break;
    default:
        throw new Exception("Invalid database type specified in configuration.");
}

builder.Build().Run();
