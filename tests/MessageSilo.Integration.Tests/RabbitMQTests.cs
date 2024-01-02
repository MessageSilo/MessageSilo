using RabbitMQ.Client;
using System.Text;

namespace MessageSilo.Integration.Tests
{
    public class RabbitMQTests
    {
        [Theory]
        [InlineData(1)]
        public void SendMessageTest(int count)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "MessageSilo.Integration.Tests.Input",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            channel.QueueDeclare(queue: "MessageSilo.Integration.Tests.Output",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            for (int i = 0; i < count; i++)
            {
                var message = "{ \"id\": " + i +
                    @",
                        ""glossary"": {
                        ""title"": ""example glossary"",
                        ""GlossDiv"": {
                            ""title"": ""S"",
                            ""GlossList"": {
                            ""GlossEntry"": {
                                ""ID"": ""SGML"",
                                ""SortAs"": ""SGML"",
                                ""GlossTerm"": ""Standard Generalized Markup Language"",
                                ""Acronym"": ""SGML"",
                                ""Abbrev"": ""ISO 8879:1986"",
                                ""GlossDef"": {
                                ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
                                ""GlossSeeAlso"": [
                                    ""GML"",
                                    ""XML""
                                ]
                                },
                                ""GlossSee"": ""markup""
                            }
                            }
                        }
                        }
                    }";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "MessageSilo.Integration.Tests.Input",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}