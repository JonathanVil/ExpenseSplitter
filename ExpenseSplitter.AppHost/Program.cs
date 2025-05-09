using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050))
    .WithDataVolume(isReadOnly: false);
var postgresdb = postgres.AddDatabase("postgresdb");

var api = builder.AddProject<ExpenseSplitter_API>("api")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();