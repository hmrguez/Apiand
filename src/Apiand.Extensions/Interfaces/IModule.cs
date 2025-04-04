using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Apiand.Extensions.Interfaces;

/// <summary>
/// Interface for defining a module in the Apiand framework.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Configures the application builder for the module, including services, logging, etc.
    /// </summary>
    /// <param name="builder">Generally the WebApplicationBuilder the program starts with</param>
    /// <param name="configuration">The application configuration</param>
    void ConfigureWebAppBuilder(IHostApplicationBuilder builder, IConfiguration configuration);
    
    /// <summary>
    /// Configures the middleware and request pipeline for the module.
    /// </summary>
    /// <param name="app">The application builder to configure the request pipeline.</param>
    void ConfigureApplication(IApplicationBuilder app);
    
    /// <summary>
    /// Registers any module-specific mappings, such as AutoMapper profiles.
    /// </summary>
    void RegisterMappings();
    
    /// <summary>
    /// Gets the name of the module for logging and diagnostics.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the priority of the module for ordering. Lower numbers load first.
    /// </summary>
    int Order { get; }
    
    /// <summary>
    /// Determines whether the module is enabled based on the configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>True if the module is enabled; otherwise, false.</returns>
    bool IsEnabled(IConfiguration configuration);
}