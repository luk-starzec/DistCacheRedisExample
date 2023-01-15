var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDistributedMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
    var redisPort = int.Parse(Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379");
    var redisPassword = Environment.GetEnvironmentVariable(variable: "REDIS_PASSWORD");

    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { { redisHost, redisPort } },
        Password = redisPassword,
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
