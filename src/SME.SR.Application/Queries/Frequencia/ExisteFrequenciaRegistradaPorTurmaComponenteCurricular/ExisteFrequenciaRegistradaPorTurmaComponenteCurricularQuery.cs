using MediatR;

namespace SME.SR.Application
{
    public class ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery : IRequest<bool>
    {
        public ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery(string codigoTurma, string componenteCurricularId, long periodoEscolarId, int[] bimestres)
        {
            CodigoTurma = codigoTurma;
            ComponenteCurricularId = componenteCurricularId;
            PeriodoEscolarId = periodoEscolarId;
            Bimestres = bimestres;
        }

        public string CodigoTurma { get; set; }
        public string ComponenteCurricularId { get; set; }
        public long PeriodoEscolarId { get; set; }
        public int[] Bimestres { get; set; }
    }
}
