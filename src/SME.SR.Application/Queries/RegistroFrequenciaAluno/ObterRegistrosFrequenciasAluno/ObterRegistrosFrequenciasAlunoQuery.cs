using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRegistrosFrequenciasAlunoQuery : IRequest<IEnumerable<RegistroFrequenciaAlunoDto>>
    {
        public string[] CodigosAlunos { get; set; }
        public string[] TurmasCodigo { get; set; }
        public string[] ComponentesCurricularesId { get; set; }
        public long TipoCalendarioId { get; set; }
        public int[] Bimestres { get; set; }

        public ObterRegistrosFrequenciasAlunoQuery(string[] codigosAlunos, string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres)
        {
            CodigosAlunos = codigosAlunos;
            TurmasCodigo = turmasCodigo;
            ComponentesCurricularesId = componentesCurricularesId;
            TipoCalendarioId = tipoCalendarioId;
            Bimestres = bimestres;
        }
    }
}
