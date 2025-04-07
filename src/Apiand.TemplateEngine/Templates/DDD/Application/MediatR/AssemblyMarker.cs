using System.Reflection;
using Apiand.Extensions.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XXXnameXXX.Application.DI;
using Microsoft.Extensions.Hosting;

namespace XXXnameXXX.Application;

public class ApplicationModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void ConfigureWebAppBuilder(IHostApplicationBuilder builder, IConfiguration configuration)
    {
        
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
    }

    public void RegisterMappings()
    {
        MappingInstaller.RegisterMappings();
    }

    public bool IsEnabled(IConfiguration configuration)
    {
        return true;
    }

    public string Name { get; } = "Application";
    public int Order { get; } = 1;
}