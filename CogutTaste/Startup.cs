using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CogutTaste.DataAccess;
using CogutTaste.DataAccess.Data.Repository.IRepository;
using CogutTaste.DataAccess.Data.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using CogutTaste.Utility;
using Stripe;
using CogutTaste.DataAccess.Data.Initializer;

namespace CogutTaste
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //.AddEntityFrameworkStores<ApplicationDbContext>(); bunu default identiy ile kullanıyorduk.. bizim roller devreye gireceğinden aşağıdaki gibi değiştirdik..
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
               // .AddDefaultUI(UIFrameworkAttribute.Bootstrap4)// asp.net core 3.0 sonrası buna gerek yok...
               .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddScoped<IDbInitializer, DbInitializer>(); // adding intervade and class to our scope..

            services.AddSingleton<IEmailSender, EmailSender>();// identity e role eklendiğinde hataveriyordu.. Utility e email sender ekleyince ve onu burada servislere ekleyince düzeldi..

            services.ConfigureApplicationCookie(options => //ders 98 dolayısıyla...
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));//appseeting.json da Stripe isimli bir section var.. ona buradan ulaşarak stripe secret ve public keyleri alıyoruz..

            /*services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);*/ // display propertyler için mvc kullanmam?z gerekecek. .mvc yi de kullan?rken endpoint routing le de?il de klasik mvc ile kullan?yoruz.. 
            // proje sonuna kadar endpoint routing yukarısı aktif oduğundan disabled idi. yani mvc routing kullanıyorduk.. Son derste yukarıyı comment leyip, endpoint routing i aktif hale getirdik..

            services.AddRazorPages(); // bunu proje sonunda endpoint routing için ekledik.. aşağıdaki de yukarıda iptal ettiğimiz mvc routing i yapacak zaten..
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //services.AddRazorPages().AddRazorRuntimeCompilation(); yukar?dakini ekledi?imizden bunu sildik..

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "3467712393285213";
                facebookOptions.AppSecret = "55656f0c3c5ebd974c561f779f97401c";
            });

            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = "6a9ea837-4504-430c-a8e0-ce271833916a";
                options.ClientSecret = "?L-?.aKA3UGkqYnHQB3oR88W.Ee2c2Oq";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env) böyleydi.. buna aşağıdaki gibi İnitializer ekledik...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            //app.UseRouting(); aşağıdaki aç?klama nedeniyle bunu da sildik

            app.UseRouting(); //yukarıda servicess de proje sonunda Razor Page routing i tekrar aktif hale getirdiğimizden bunu yine açtık proje sonunda

            dbInitializer.Initialize();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //}); mvc klasik routing kullanaca??m?zdan end point routing i yukar?da false yapt?k.. burada da sildik..

            //app.UseMvc(); proje sonunda bunu kapapttık ve end point routing i tekrara açtık..
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
        }
    }
}
