using Hangfire;

namespace LetterToSelf;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHangfire(config => config.UseSqlServerStorage("Server=104.247.162.242\\MSSQLSERVER2019;Initial Catalog=miraykar_letter;User Id=miraykar_letterdbuser;Password=Se87l4?a0;TrustServerCertificate=True")); 
        builder.Services.AddHangfireServer();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseHangfireDashboard();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}