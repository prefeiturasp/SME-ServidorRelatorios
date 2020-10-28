using MediatR;

namespace SME.SR.Application
{
    public class VerificarAulasNormaisCriadasProfessorRegenciaQuery : IRequest<bool>
    {
        public VerificarAulasNormaisCriadasProfessorRegenciaQuery(string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {           
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
            TipoCalendarioId = tipoCalendarioId;
        }

        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
