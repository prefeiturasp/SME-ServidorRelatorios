using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class FiltroRelatorioSincronoDto
    {
        public object Mensagem { get; set; }        
        public T ObterObjetoFiltro<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
