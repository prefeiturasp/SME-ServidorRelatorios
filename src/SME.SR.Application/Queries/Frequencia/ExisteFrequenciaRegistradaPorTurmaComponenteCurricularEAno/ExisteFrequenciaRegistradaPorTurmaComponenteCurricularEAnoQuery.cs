using MediatR;

namespace SME.SR.Application
{
    public class ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery : IRequest<bool>
    {
        public ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(string codigoTurma, string componenteCurricularId, int anoLetivo)
        {
            CodigoTurma = codigoTurma;
            ComponenteCurricularId = componenteCurricularId;
            AnoLetivo = anoLetivo;
        }

        public string CodigoTurma { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int AnoLetivo { get; set; }
    }
}
