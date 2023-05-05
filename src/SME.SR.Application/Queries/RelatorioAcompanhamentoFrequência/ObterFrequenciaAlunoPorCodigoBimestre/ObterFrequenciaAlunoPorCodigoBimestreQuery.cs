using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorCodigoBimestreQuery : IRequest<IEnumerable<FrequenciaAlunoConsolidadoDto>>
    {
        public ObterFrequenciaAlunoPorCodigoBimestreQuery(string bimestre, string[] codigosAlunos, string turmaCodigo, TipoFrequenciaAluno tipoFrequencia, string[] componentesCurricularesIds)
        {
            Bimestre = bimestre;
            CodigosAlunos = codigosAlunos;
            TurmaCodigo = turmaCodigo;
            TipoFrequencia = tipoFrequencia;
            ComponentesCurricularesIds = componentesCurricularesIds;
        }

        public string Bimestre { get; set; }
        public string[] CodigosAlunos { get; set; }
        public string TurmaCodigo { get; set; }
        public TipoFrequenciaAluno TipoFrequencia { get; set; }
        public string[] ComponentesCurricularesIds { get; set; }
    }
}
