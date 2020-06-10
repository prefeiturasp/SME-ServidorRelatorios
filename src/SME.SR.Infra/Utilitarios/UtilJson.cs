using Newtonsoft.Json;

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
    }
}
