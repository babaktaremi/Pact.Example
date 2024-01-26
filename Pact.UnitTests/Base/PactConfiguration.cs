using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;

namespace Pact.UnitTests.Base;

public class PactConfiguration
{
    

    public PactConfiguration()
    {
        
    }

    public static IPactBuilderV3 BuildServer()
    {
        var pactConfig = new PactConfig()
        {
            LogLevel = PactLogLevel.Debug,
            PactDir = @"c:\temp\pactTest\pactbuilderBase",
            DefaultJsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };


        var pact = PactNet.Pact.V3("PactApi", "PactDiscountApi", pactConfig);

        return pact.WithHttpInteractions();
    }
}