using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiChain.RabbitMQ
{
    /// <summary>
    /// EasyNetQ是对RabbitMq封装的一个包
    /// </summary>
    public class EasyNetQApi
    {
        private readonly IBus bus;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">rabbitmq连接字符串</param>
        public EasyNetQApi(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = RabbitMqSetting.CONNECTIONSTRING;
            }
            bus = RabbitHutch.CreateBus(connectionString);
        }

        /// <summary>
        /// 产生消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages"></param>
        /// <param name="topic"></param>
        public void ProduceMessage<T>(IList<T> messages, string topic) where T : class
        {
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    if (string.IsNullOrEmpty(topic))
                    {
                        bus.Publish(message);
                        Thread.Sleep(50);
                    }
                    else
                    {
                        bus.Publish(message, x => x.WithTopic(topic));
                        Thread.Sleep(50);
                    }
                }
            }
        }

        /// <summary>
        /// 异步产生消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task ProduceMessageAsync<T>(IList<T> messages, string topic) where T : class
        {
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    if (string.IsNullOrEmpty(topic))
                    {
                        await bus.PublishAsync(message);
                        Thread.Sleep(50);
                    }
                    else
                    {
                        await bus.PublishAsync(message, x => x.WithTopic(topic));
                        Thread.Sleep(50);
                    }
                }
            }
        }

        /// <summary>
        /// 发送消息到指定队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="messages"></param>
        public void SendMessageToQueue<T>(string queue, IList<T> messages) where T : class
        {
            foreach (var message in messages)
            {
                bus.Send(queue, message);
                Thread.Sleep(50);//必须加上，以防消息阻塞
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="process"></param>
        public void ReceiveMessage<T>(string queue, Func<T, Task> process) where T : class
        {
            bus.Receive(queue, process);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriptionId"></param>
        /// <param name="process"></param>
        /// <param name="topic"></param>
        public void SubscribeMessage<T>(string subscriptionId, Func<T, Task> process, string topic = null) where T : class
        {
            if (string.IsNullOrEmpty(topic))
            {
                bus.Subscribe<T>(subscriptionId, message => process(message));
            }
            else
            {
                bus.Subscribe<T>(subscriptionId, message => process(message), x => x.WithTopic(topic));
            }
        }

        /// <summary>
        /// 自动订阅消息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="subscriptionIdPrefix"></param>
        /// <param name="topic"></param>
        public void AutoSubscribeMessage(string assemblyName, string subscriptionIdPrefix, string topic)
        {
            var subscriber = new AutoSubscriber(bus, subscriptionIdPrefix);
            if (!string.IsNullOrEmpty(topic))
            {
                subscriber.ConfigureSubscriptionConfiguration = x => x.WithTopic(topic);
            }
            subscriber.Subscribe(Assembly.Load(assemblyName));
        }
    }
}
