using MediatR;

namespace SME.SR.Application
{
    public class VerificarAulasNormaisCriadasProfessorRegenciaQuery : IRequest<bool>
    {
        public VerificarAulasNormaisCriadasProfessorRegenciaQuery(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId)
        {           
            Turmaid = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
            TipoCalendarioId = tipoCalendarioId;
        }

        public long Turmaid { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
