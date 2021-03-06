﻿using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
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
        public FonteDadosRecursoDto FonteDados { get; set; }

        [JsonProperty("query")]
        public QueryReferenciaRecursoDto Query { get; set; }

        [JsonProperty("jrxml")]
        public JRXMLRecursoDto JRXML { get; set; }

        [JsonProperty("inputControls")]
        public ControleEntradaJRXMLRecursoDto[] ControlesEntrada { get; set; }

        [JsonProperty("resources")]
        public RecursoJRXMLDto[] Recurso { get; set; }
    }
}
