using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
