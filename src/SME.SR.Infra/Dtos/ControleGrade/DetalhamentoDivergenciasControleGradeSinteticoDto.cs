using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DetalhamentoDivergenciasControleGradeSinteticoDto
    {
        public DetalhamentoDivergenciasControleGradeSinteticoDto()
        {
            AulasNormaisExcedido = new List<AulaNormalExcedidoControleGradeDto>();
            AulasTitularCJ = new List<AulaTitularCJDataControleGradeDto>();
            AulasDiasNaoLetivos = new List<AulaDiasNaoLetivosControleGradeDto>();
            AulasDuplicadas = new List<AulaDuplicadaControleGradeDto>();
        }
        public IEnumerable<AulaNormalExcedidoControleGradeDto> AulasNormaisExcedido { get; set; }
        public IEnumerable<AulaTitularCJDataControleGradeDto> AulasTitularCJ { get; set; }
        public IEnumerable<AulaDiasNaoLetivosControleGradeDto> AulasDiasNaoLetivos { get; set; }
        public IEnumerable<AulaDuplicadaControleGradeDto> AulasDuplicadas { get; set; }
    }
}
