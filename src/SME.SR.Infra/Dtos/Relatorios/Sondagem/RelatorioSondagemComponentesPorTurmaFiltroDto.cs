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
        public int DreId { get; set; }
        public int TurmaId { get; set; }
        public int Semestre { get; set; }
        public int Ano { get; set; }
        public T ObterObjetoFiltro<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
