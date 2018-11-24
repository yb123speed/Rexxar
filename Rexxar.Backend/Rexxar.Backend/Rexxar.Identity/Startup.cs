using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rexxar.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace Rexxar.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region DbContext
            //配置AspNetCore DbContext 服务
            services.AddDbContext<RexxarDbContext>(o => 
            {
                o.UseNpgsql(Configuration.GetConnectionString("RexxarIdentity"),options=> 
                {
                    //配置迁移时程序集
                    options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
            #endregion

            #region Identity
            //配置AspNetCore Identity服务用户密码的验证规则
            services.AddIdentity<RexxarUser, RexxarRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 6;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            })
            // 告诉AspNetCore Identity 使用RexxarDbContext为数据库上下文
            .AddEntityFrameworkStores<RexxarDbContext>()
            .AddDefaultTokenProviders();
            #endregion

            #region IdentityServer4
            //配置IdentityServer4服务
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<RexxarUser>()
                .AddConfigurationStore(o =>
                {
                    o.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(Configuration.GetConnectionString("RexxarIdentity"), options =>
                        {
                            //配置迁移时程序集
                            options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        });
                    };
                })
                // 使用数据库存储授权操作相关操作，数据库表PersistedGrants
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(Configuration.GetConnectionString("RexxarIdentity"), sql =>
                        {
                            sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        });
                    };
                })
                // ids4使用自定义的用户档案服务
                .Services.AddScoped<IProfileService, ProfileService>(); ;
            #endregion

            #region CORS
            // 配置跨域，允许所有
            services.AddCors(o =>
            {
                o.AddPolicy("all", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    ;
                });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("all");

            app.UseIdentityServer();

            MigrateDatabase(app).Wait();
        }

        public static async Task MigrateDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // 迁移DemoDbContext上下文
                scope.ServiceProvider.GetRequiredService<RexxarDbContext>().Database.Migrate();
                // 迁移PersistedGrantDbContext上下文
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                // 迁移ConfigurationDbContext上下文
                configurationDbContext.Database.Migrate();

                // 注入用户管理 增加用户
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RexxarUser>>();
                foreach (var user in SeedData.Users())
                {
                    if (userManager.FindByNameAsync(user.UserName).Result == null)
                    {
                        await userManager.CreateAsync(user, "123456");
                    }
                }

                // 增加ApiResources IdentityResources Clients
                if (!configurationDbContext.ApiResources.Any())
                    configurationDbContext.ApiResources.AddRange(Config.GetApiResouces().Select(r => r.ToEntity()));
                if (!configurationDbContext.IdentityResources.Any())
                    configurationDbContext.IdentityResources.AddRange(Config.GetIdentityResources().Select(r => r.ToEntity()));
                if (!configurationDbContext.Clients.Any())
                    configurationDbContext.Clients.AddRange(Config.GetClients().Select(r => r.ToEntity()));
                await configurationDbContext.SaveChangesAsync();
            }
        }

    }
}
