using RabbitMQ.Client;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

namespace ProjetoTeste
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
                    channel.ExchangeDeclare(
                            exchange: "ExchangeTeste",
                            type: ExchangeType.Direct,//Ver para que isso serve
                            durable: false,
                            autoDelete: false,
                            arguments: null
                        );

                    TestObject objeto = new TestObject() { Codigo = 1, Nome = "Teste" };
                    var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objeto));

                    channel.BasicPublish(
                            exchange: "ExchangeTeste",
                            routingKey: "teste",
                            basicProperties: null,
                            body: body
                        );

                    channel.BasicPublish(
                            exchange: "ExchangeTeste",
                            routingKey: "teste",
                            basicProperties: null,
                            body: body
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

    public static class ObjectExtension
    {
        public static byte[] ObjectToByteArray(this Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static Object ByteArrayToObject(this byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
