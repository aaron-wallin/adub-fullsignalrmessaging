using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adub.MessagingEndpoint.Configuration
{
    public class RabbitConnectionInfo : IRabbitConnectionInfo
    {
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string VHost { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }

        public RabbitConnectionInfo()
        {

        }

        public IRabbitConnectionInfo Initialize(string serviceName)
        {
            //var rabbitConfig = new Steeltoe.CloudFoundry.Connector.Rabbit.RabbitProviderConnectorOptions();

            //this.Host = rabbitConfig.Server;
            //this.Port = rabbitConfig.Port.ToString();
            //this.User = rabbitConfig.Username;
            //this.Password = rabbitConfig.Password;
            //this.VHost = rabbitConfig.VirtualHost;

            var baseData = "$..[?(@.name=='" + serviceName + "')].credentials.protocols.amqp";
            var jObj = JObject.Parse(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            if (jObj.SelectToken($"{baseData}") == null)
                throw new Exception("Expects a PCF managed RabbitMQ service binding named 'masstransit-poc'");

            this.Host = (string)jObj.SelectToken($"{baseData}.host");
            this.VHost = (string)jObj.SelectToken($"{baseData}.vhost");
            this.User = (string)jObj.SelectToken($"{baseData}.username");
            this.Password = (string)jObj.SelectToken($"{baseData}.password");
            this.Port = (string)jObj.SelectToken($"{baseData}.port");

            return this;
        }
    }
}
