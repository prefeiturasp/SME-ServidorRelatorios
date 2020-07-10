using Newtonsoft.Json;
using System;

namespace SME.SR.Infra
{
    public class FiltroRelatorioDto
    {
        public string Action { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioLogadoRF { get; set; }
        public string PerfilUsuario { get; set; }
        public T ObterObjetoFiltro<T>() where T: class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
