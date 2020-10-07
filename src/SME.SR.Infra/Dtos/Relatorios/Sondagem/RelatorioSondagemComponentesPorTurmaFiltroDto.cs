using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaFiltroDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int ProficienciaId { get; set; }
        public int Semestre { get; set; }
        public string UsuarioRF { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public object Mensagem { get; set; }
        public T ObterObjetoFiltro<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}
