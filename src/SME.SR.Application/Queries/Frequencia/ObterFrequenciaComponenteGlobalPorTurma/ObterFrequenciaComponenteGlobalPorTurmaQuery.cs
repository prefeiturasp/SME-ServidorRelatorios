using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaComponenteGlobalPorTurmaQuery(string[] turmasCodigo, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, int[] bimestres, long tipoCalendarioId)
        {
            TurmasCodigo = turmasCodigo;
            ComponentesCurricularesPorTurma = componentesCurricularesPorTurma;
            Bimestres = bimestres;
            TipoCalendarioId = tipoCalendarioId;
        }

        public string[] TurmasCodigo { get; set; }
        public IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> ComponentesCurricularesPorTurma { get; set; }
        public int[] Bimestres { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
