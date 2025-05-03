var builder = DistributedApplication.CreateBuilder(args);

// backing services
var postgres = builder.AddPostgres("postgres").WithPgAdmin().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);
var catalogDb = postgres.AddDatabase("catalogDb");
var cache = builder.AddRedis("cache").WithRedisInsight().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);
var rabbitMq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin().WithDataVolume().WithLifetime(ContainerLifetime.Persistent);

// projects
var catalog = builder.AddProject<Projects.Catalog>("catalog").WithReference(catalogDb).WithReference(rabbitMq).WaitFor(catalogDb).WaitFor(rabbitMq);

var basket = builder.AddProject<Projects.Basket>("basket").WithReference(cache).WithReference(catalog).WithReference(rabbitMq).WaitFor(cache).WaitFor(rabbitMq);

builder.Build().Run();
