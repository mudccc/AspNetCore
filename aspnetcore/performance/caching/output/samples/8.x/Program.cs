using Microsoft.AspNetCore.OutputCaching;

namespace OCMinimal;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        // <redisoptions>
        builder.Services.AddStackExchangeRedisOutputCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
            options.InstanceName = "SampleInstance";
        });
        builder.Services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => 
                builder.Expire(TimeSpan.FromSeconds(10)));
        });
        // </redisoptions>
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();
        app.UseOutputCache();
        app.UseAuthorization();

        app.MapGet("/", Gravatar.WriteGravatar);

        app.MapGet("/cached", Gravatar.WriteGravatar).CacheOutput();
        app.MapGet("/attribute", [OutputCache] (context) => 
            Gravatar.WriteGravatar(context));

        app.Run();
    }
}
