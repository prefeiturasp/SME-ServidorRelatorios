using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class FiltroRelatorioSincronoDto
    {
        public object Mensagem { get; set; }        
        public T ObterObjetoFiltro<T>() where T : class
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString(), settings);
        }
    }
}
