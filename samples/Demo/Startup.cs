using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.WeChat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Demo.Data;
using Demo.Models;
using Demo.Services;
using System;
using System.Threading.Tasks;

namespace Demo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
           // services.AddTransient<IEmailSender, EmailSender>();

            services.AddAuthentication()
            //    .AddMicrosoftAccount(microsoftOptions =>
            //{
            //    microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
            //    microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:Password"];
            //})/*.AddGoogle(googleOptions => //Google
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //})*/.AddQQ(qqOptions =>
            //{
            //    qqOptions.AppId = Configuration["Authentication:QQ:AppId"];
            //    qqOptions.AppKey = Configuration["Authentication:QQ:AppKey"];
            //})
            .AddWeChat(options => {
                options.AppId = Configuration["Authentication:WeChat:AppId"];
                options.AppSecret = Configuration["Authentication:WeChat:AppSecret"];

                //1.创建Ticket之前触发
                options.Events.OnCreatingTicket = ContextBoundObject =>
                {

                    return Task.CompletedTask;

                };
                //    //2.创建Ticket失败时触发
                options.Events.OnRemoteFailure = context =>
                {
                    return Task.CompletedTask;
                };
                //    //3.Tick接收完成之后触发
                options.Events.OnTicketReceived = context =>
                {
                    return Task.CompletedTask;
                };
                //    //4.Challenge时触发,默认转到OAuth服务器
                options.Events.OnRedirectToAuthorizationEndpoint = context =>
                {

                    //var uri = new UriBuilder(context.RedirectUri);
                    //uri.Query = new Regex("(?<=redirect_uri=).+(?=&response_type=code)").Replace(uri.Query, WebUtility.UrlEncode(context.Properties.RedirectUri));

                    //context.Response.Redirect(uri.ToString());
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            }) ;

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (true) // env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //loggerFactory.AddConsole();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
