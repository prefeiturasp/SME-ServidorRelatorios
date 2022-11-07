using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaComponenteGlobalPorTurmaQuery(string[] turmasCodigo, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, int[] bimestres, long tipoCalendarioId, IEnumerable<(string codigoAluno, DateTime dataMatricula, DateTime? dataSituacao)> alunosDatasMatricula)
        {
            TurmasCodigo = turmasCodigo;
            ComponentesCurricularesPorTurma = componentesCurricularesPorTurma;
            Bimestres = bimestres;
            TipoCalendarioId = tipoCalendarioId;
            AlunosDatasMatricula = alunosDatasMatricula;
        }

        public string[] TurmasCodigo { get; set; }
        public IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> ComponentesCurricularesPorTurma { get; set; }
        public int[] Bimestres { get; set; }
        public long TipoCalendarioId { get; set; }
        public IEnumerable<(string codigoAluno, DateTime dataMatricula, DateTime? dataSituacao)> AlunosDatasMatricula { get; set; }
    }
}
