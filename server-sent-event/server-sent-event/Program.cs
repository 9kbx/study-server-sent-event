using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using server_sent_event.db;

namespace server_sent_event
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.WebHost.ConfigureKestrel(serverOptions =>
            //{
            //    serverOptions.ConfigureEndpointDefaults(listenOptions =>
            //    {
            //    });
            //});

            var services = builder.Services;

            #region use server sent event
            // https://stackoverflow.com/questions/47735133/asp-net-core-synchronous-operations-are-disallowed-call-writeasync-or-set-all
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            #endregion use server sent event


            InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();
            string _connectionString = Guid.NewGuid().ToString();
            builder.Services.AddEntityFrameworkInMemoryDatabase();
            builder.Services.AddDbContext<MemDbContext>(options =>
            {
                options.UseInMemoryDatabase(_connectionString, _databaseRoot);
            });

            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromHours(24);
            });
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}