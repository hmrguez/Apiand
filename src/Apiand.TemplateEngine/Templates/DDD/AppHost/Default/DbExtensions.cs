using Projects;

namespace XXXnameXXX.AppHost;

public static class DbExtensions
{
    public static void AddMongo(this IDistributedApplicationBuilder builder)
    {
        var mongo = builder.AddMongoDB("mongo")
            .WithLifetime(ContainerLifetime.Persistent);

        var mongodb = mongo.AddDatabase("mongodb", databaseName: "storm");

        builder.AddProject<XXXnameXXX_Presentation>("api")
            .WithReference(mongodb)
            .WaitFor(mongodb)
            .WithSwaggerUi()
            .WithScalar();
    }

    public static void AddPostgres(this IDistributedApplicationBuilder builder)
    {
        var postgres = builder.AddPostgres("postgres")
            .WithLifetime(ContainerLifetime.Persistent);

        var postgresdb = postgres.AddDatabase("postgresdb");

        builder.AddProject<XXXnameXXX_Presentation>("api")
            .WithReference(postgresdb)
            .WaitFor(postgresdb)
            .WithSwaggerUi()
            .WithScalar();;
    }

    public static void AddSqlServer(this IDistributedApplicationBuilder builder)
    {
        var sql = builder.AddSqlServer("sql")
            .WithLifetime(ContainerLifetime.Persistent);

        var db = sql.AddDatabase("sqldb");

        builder.AddProject<XXXnameXXX_Presentation>("api")
            .WithReference(db)
            .WaitFor(db)
            .WithSwaggerUi()
            .WithScalar();;
    }
}