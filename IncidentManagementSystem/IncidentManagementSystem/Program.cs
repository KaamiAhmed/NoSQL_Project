using IncidentManagementSystem.Repositories;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services;
using IncidentManagementSystem.Services.Interfaces;
using MongoDB.Driver;

namespace IncidentManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load .env before building configuration so env vars are available
            DotNetEnv.Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();
            builder.Services.AddScoped<IResetPasswordService, ResetPasswordService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 1) Register MongoClient as a SINGLETON (one shared instance for the whole app)
            // WHY: MongoClient is thread-safe and internally manages a connection pool.
            // Reusing one instance is fast and efficient. Creating many clients would waste resources.
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                // Read the connection string from configuration (env var via .env)
                var conn = builder.Configuration["Mongo_ConnectionString"];
                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("Mongo_ConnectionString is not configured. Did you set it in .env?");

                // Optional: tweak settings (timeouts, etc.)
                var settings = MongoClientSettings.FromConnectionString(conn);
                // settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

                return new MongoClient(settings);
            });

            // 2) Register IMongoDatabase as SCOPED (new per HTTP request)
            // WHY: Fits the ASP.NET request lifecycle and keeps each request cleanly separated.
            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();

                var dbName = builder.Configuration["Mongo:Database"]; // from appsettings.json
                if (string.IsNullOrWhiteSpace(dbName))
                    throw new InvalidOperationException("Mongo:Database is not configured in appsettings.json.");

                return client.GetDatabase(dbName);
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
