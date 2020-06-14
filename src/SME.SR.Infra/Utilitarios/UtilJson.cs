using Newtonsoft.Json;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Infra
{
    public static class UtilJson
    {
        public static string ConverterApenasCamposNaoNulos<T>(T paraConverter)
        {
            return JsonConvert.SerializeObject(paraConverter,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }

        public static JsonSerializerSettings ObterConfigConverterNulosEmVazio()
        {
            return new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
        }
    }
}
