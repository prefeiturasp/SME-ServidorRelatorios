using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class UnidadeRelatorioRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("alwaysPromptControls")]
        public bool SempreExibirControles { get; set; }

        [JsonProperty("controlsLayout")]
        public string ControleLayout { get; set; }

        [JsonProperty("inputControlRenderingView")]
        public string ControleEntradaRenderizandoVisualizacao { get; set; }

        [JsonProperty("reportRenderingView")]
        public string RelatiorioRenderizandoVisualizacao { get; set; }

        [JsonProperty("dataSource")]
        public QueryFonteDadosRecursoDto FonteDados { get; set; }

        [JsonProperty("query")]
        public ControleEntradaQueryRecursoDto ClasseDriver { get; set; }

        [JsonProperty("jrxml")]
        public JRXMLRecursoDto JRXML { get; set; }

        [JsonProperty("inputControls")]
        public ControleEntradaJRXMLRecursoDto[] ControlesEntrada { get; set; }

        [JsonProperty("resources")]
        public RecursoJRXMLDto[] Recurso { get; set; }
    }
}
