using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorCodigoBimestreQuery : IRequest<IEnumerable<FrequenciaAlunoConsolidadoDto>>
    {
        public ObterFrequenciaAlunoPorCodigoBimestreQuery(string bimestre, string[] codigosAlunos, string turmaCodigo, TipoFrequenciaAluno tipoFrequencia, string componenteCurricularId = null)
        {
            Bimestre = bimestre;
            CodigosAlunos = codigosAlunos;
            TurmaCodigo = turmaCodigo;
            TipoFrequencia = tipoFrequencia;
            ComponenteCurricularId = componenteCurricularId;
        }

        public string Bimestre { get; set; }
        public string[] CodigosAlunos { get; set; }
        public string TurmaCodigo { get; set; }
        public TipoFrequenciaAluno TipoFrequencia { get; set; }
        public string ComponenteCurricularId { get; set; }
    }
}
