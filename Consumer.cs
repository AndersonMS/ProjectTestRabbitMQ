using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace TestConsumerRabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                            queue: "QueueTeste",
                            exclusive: false,
                            durable: false,
                            autoDelete: false,
                            arguments: null
                        );

                    channel.QueueBind(
                            queue: "QueueTeste",
                            exchange: "ExchangeTeste",
                            routingKey: "teste",
                            arguments: null
                        );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body;
                            var message = JsonConvert.DeserializeObject<TestObject>(Encoding.UTF8.GetString(body));
                            Console.WriteLine(message.Nome);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                        catch(Exception ex)
                        {
                            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                    };
                    channel.BasicConsume(
                            queue: "QueueTeste",
                            autoAck: false,
                            consumer: consumer
                        );

                    Console.ReadLine();
                }
            }
        }
    }
    
    public class TestObject
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }
}
