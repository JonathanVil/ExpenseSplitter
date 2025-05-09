using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<ExpenseSplitter_API>("api");

builder.Build().Run();