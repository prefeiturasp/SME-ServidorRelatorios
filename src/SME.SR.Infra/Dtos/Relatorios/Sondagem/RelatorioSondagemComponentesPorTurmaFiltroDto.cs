using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaFiltroDto
    {
        public Guid CodigoCorrelacao { get; set; }
        public object Mensagem { get; set; }
        public string DreId { get; set; }
        public string TurmaId { get; set; }
        public string UeId { get; set; }
        public string Ano { get; set; }
        public T ObterObjetoFiltro<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
