﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rexxar.Chat.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rexxar.Chat.Extensions
{
    public class MsgSender
    {
        public MsgSender(IConfiguration configuration)
        {
            factory = new ConnectionFactory();
            factory.HostName = configuration.GetValue<string>("RabbitMQ:Host");
            factory.UserName = configuration.GetValue<string>("RabbitMQ:User");
            factory.Password = configuration.GetValue<string>("RabbitMQ:Password");
        }
        ConnectionFactory factory;

        public void Send(MsgDto msg)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("chat_queue", false, false, false, null);//创建一个名称为hello的消息队列
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                    channel.BasicPublish("", "chat_queue", null, body); //开始传递
                }
            }
        }
    }

    public class MsgHandler : IDisposable
    {
        public MsgHandler(IConfiguration configuration, IHubContext<MessageHub> hubContext)
        {
            factory = new ConnectionFactory();
            factory.HostName = configuration.GetValue<string>("RabbitMQ:Host");
            factory.UserName = configuration.GetValue<string>("RabbitMQ:User");
            factory.Password = configuration.GetValue<string>("RabbitMQ:Password");
            this.hubContext = hubContext;
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

        }
        ConnectionFactory factory;
        // 注入SignalR的消息处理器上下文，用以发送消息到客户端
        IHubContext<MessageHub> hubContext;
        IConnection connection;
        IModel channel;
        public void BeginHandleMsg()
        {
            channel.QueueDeclare("chat_queue", false, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("chat_queue", false, consumer);
            consumer.Received += (model, arg) =>
            {
                var body = arg.Body;
                var message = Encoding.UTF8.GetString(body);
                var msg = JsonConvert.DeserializeObject<MsgDto>(message);
                // 通过消息处理器上下文发送消息到客户端
                hubContext.Clients?.Client(msg.ToUser.ConnectionId)
                                  ?.SendAsync("Receive", msg);

                channel.BasicAck(arg.DeliveryTag, false);
            };
        }

        public void Dispose()
        {
            channel?.Dispose();
            connection?.Dispose();
        }
    }
}
