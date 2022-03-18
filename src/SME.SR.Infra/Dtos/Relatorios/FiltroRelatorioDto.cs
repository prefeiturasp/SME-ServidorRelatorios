using Newtonsoft.Json;
using System;

namespace SME.SR.Infra
{
    public class FiltroRelatorioDto
    {
        public FiltroRelatorioDto()
        {
            RotaProcessando = RotasRabbitSR.RotaRelatoriosProcessando;
            RotaErro = RotasRabbitSGP.RotaRelatorioComErro;
        }
        public string Action { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioLogadoRF { get; set; }
        public string PerfilUsuario { get; set; }
        public string RotaProcessando { get; set; }
        public string RotaErro { get; set; }
        public bool RelatorioEscolaAqui { get; set; }
        public T ObterObjetoFiltro<T>() where T: class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
