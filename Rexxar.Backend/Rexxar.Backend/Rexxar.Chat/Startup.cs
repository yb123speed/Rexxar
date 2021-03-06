﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rexxar.Chat.Dtos;
using Rexxar.Chat.Extensions;

namespace Rexxar.Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 注册消息处理器 消息发送器，在线用户类
            services.AddSingleton<MsgHandler>()
                .AddSingleton<MsgSender>()
                .AddSingleton<OnlineUsers>();

            // 增加认证服务
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "JwtBearer";
            })
            .AddIdentityServerAuthentication("JwtBearer", r=>
            {
                // 配置认证服务器
                r.Authority = Configuration.GetValue<string>("Authentication:Authority");
                // 配置无需验证https
                r.RequireHttpsMetadata = false;
                // 配置 当前资源服务器的名称
                r.ApiName = "chatapi";
                // 配置 当前资源服务器的连接密码
                r.ApiSecret = "secret";
                r.SaveToken = true;
            });

            // 跨域
            services.AddCors(r =>
            {
                r.AddPolicy("all", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    ;
                });
            });
            // 增加授权服务
            services.AddAuthorization();
            // 增加SignalR 服务
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MsgHandler msgHandler,IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCors("all");

            // signalr jwt认证 token添加
            app.Use(async (context, next) =>
            {
                // 这里从url中获取token参数，实际应用请实际考虑，加一些过滤条件
                if (context.Request.Query.TryGetValue("token", out var token))
                {
                    // 从url中拿到header，再添加到header中，一定要在UseAuthentication之前
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                }
                await next.Invoke();
            });

            app.UseAuthentication();
            // 使用SignalR 并添加MessageHub类的消息处理器
            app.UseSignalR(r =>
            {
                r.MapHub<MessageHub>("/msg");
            });

            // 应用启动时开始处理消息
            applicationLifetime.ApplicationStarted.Register(msgHandler.BeginHandleMsg);
            // 应用退出时,释放资源
            applicationLifetime.ApplicationStopping.Register(msgHandler.Dispose);
        }
    }
}
