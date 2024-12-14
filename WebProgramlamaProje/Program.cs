using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/LoginPage"; // Giriþ sayfasý yolu
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Oturum süresi
                });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Seed Admin User
            void SeedAdminUser(ApplicationDbContext dbContext)
            {
                if (!dbContext.Users.Any(u => u.Role == "Admin"))
                {
                    var adminUser = new User
                    {
                        Name = "Admin",
                        Surname = "Admin",
                        Email = "b191210047@sakarya.edu.tr",
                        Password = "123456789",
                        Role = "Admin"
                    };

                    dbContext.Users.Add(adminUser);
                    dbContext.SaveChanges();
                }
            }

            // Call the seed method
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedAdminUser(dbContext);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization(); 

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
