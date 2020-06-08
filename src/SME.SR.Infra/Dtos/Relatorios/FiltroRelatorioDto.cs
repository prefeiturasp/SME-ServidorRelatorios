using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class FiltroRelatorioDto
    {
        public string Action { get; set; }
        public object Filtros { get; set; }

        public T ObterObjetoFiltro<T>() where T: class
        {
            return JsonConvert.DeserializeObject<T>(Filtros.ToString());
        }
    }
}
