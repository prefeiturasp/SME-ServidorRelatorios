using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DetalhamentoDivergenciasControleGradeSinteticoDto
    {
        public DetalhamentoDivergenciasControleGradeSinteticoDto()
        {
            AulasNormaisExcedido = new List<AulasNormaisExcedidoControleGradeSinteticoDto>();
            AulasTitularCJ = new List<AulasTitularCJDataControleGradeSinteticoDto>();
            AulasDiasNaoLetivos = new List<AulasDiasNaoLetivosControleGradeSinteticoDto>();
            AulasDuplicadas = new List<AulasDuplicadasControleGradeSinteticoDto>();
        }
        public IEnumerable<AulasNormaisExcedidoControleGradeSinteticoDto> AulasNormaisExcedido { get; set; }
        public IEnumerable<AulasTitularCJDataControleGradeSinteticoDto> AulasTitularCJ { get; set; }
        public IEnumerable<AulasDiasNaoLetivosControleGradeSinteticoDto> AulasDiasNaoLetivos { get; set; }
        public IEnumerable<AulasDuplicadasControleGradeSinteticoDto> AulasDuplicadas { get; set; }
    }
}
