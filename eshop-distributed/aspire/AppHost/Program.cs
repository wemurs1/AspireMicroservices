var builder = DistributedApplication.CreateBuilder(args);

// backing services
var postgres = builder.AddPostgres("postgres").WithPgAdmin().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);
var catalogDb = postgres.AddDatabase("catalogDb");
var cache = builder.AddRedis("cache").WithRedisInsight().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);

// projects
var catalog = builder.AddProject<Projects.Catalog>("catalog").WithReference(catalogDb).WaitFor(catalogDb);

var basket = builder.AddProject<Projects.Basket>("basket").WithReference(cache).WaitFor(cache).WithReference(catalog);

builder.Build().Run();
