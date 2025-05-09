using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgWeb();
var postgresdb = postgres.AddDatabase("postgresdb");

var api = builder.AddProject<ExpenseSplitter_API>("api")
    .WithReference(postgresdb);

builder.Build().Run();