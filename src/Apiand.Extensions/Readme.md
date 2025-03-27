I'll create a comprehensive README for your Apiand.Extensions project that explains its purpose, features, and usage.

# Apiand.Extensions

A flexible library providing common extensions and utilities for building modular, maintainable .NET applications. Core library for the Apiand framework

## Features

- **Service Attribute Registration**: Automatically register services using attributes
- **Modular Architecture**: Build applications with isolated, reusable modules
- **Domain-Driven Design**: Base classes for DDD aggregates and domain events
- **Clean Architecture Support**: Utilities that promote clean, maintainable code

## Installation

```bash
dotnet add package Apiand.Extensions
```

## Usage

The utilities contain include and are not limited to the following:

### Service Attribute Registration

Easily register services with their interfaces using attributes:

```csharp
// Define your attribute
[Service(ServiceLifetimeType.Scoped)]
public class UserService : IUserService
{
    // Implementation
}

// In your startup or module, only once
services.AddServicesWithAttribute(typeof(YourClassInAssembly).Assembly);
```

### Modular Architecture

Create isolated application modules:

```csharp
public class UsersModule : IModule
{
    public string Name => "Users";
    public int Order => 10;

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
        // Configure middleware
    }

    public void RegisterMappings()
    {
        // Register AutoMapper profiles
    }

    public bool IsEnabled(IConfiguration configuration) => true;
}
```

### Domain-Driven Design

Build domain models using the provided base classes:

```csharp
public class Order : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Status { get; private set; }
    
    public void Submit()
    {
        Status = "Submitted";
        AddDomainEvent(new OrderSubmittedEvent(Id));
    }
}
```

## Requirements

- .NET 9.0+