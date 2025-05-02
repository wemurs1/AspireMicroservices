var builder = DistributedApplication.CreateBuilder(args);

// backing services
var postgres = builder.AddPostgres("postgres").WithPgAdmin().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);
var catalogDb = postgres.AddDatabase("catalogDb");

// projects
var catalog = builder.AddProject<Projects.Catalog>("catalog").WithReference(catalogDb).WaitFor(catalogDb);

builder.Build().Run();
