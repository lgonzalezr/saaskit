namespace AspNetSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();
            //builder.Services.AddControllers();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            //app.UseAuthorization();

            app.Map(
                new PathString("/onboarding"),
                branch => branch.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync("Onboarding");
                })
            );
            app.UseMultitenancy<AppTenant>();
            //app.MapControllers();

            app.Use(async (ctx, next) =>
            {
                if (ctx.GetTenant<AppTenant>().Name == "Default")
                {
                    ctx.Response.Redirect("/onboarding");
                }
                else
                {
                    await next();
                }
            });

            app.UseMiddleware<LogTenantMiddleware>();

            app.Run();
        }
    }
}