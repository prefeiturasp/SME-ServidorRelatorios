using MediatR;

namespace SME.SR.Application
{
    public class VerificarAulasNormaisCriadasProfessorRegenciaQuery : IRequest<bool>
    {
        public VerificarAulasNormaisCriadasProfessorRegenciaQuery(long turmaId, string componenteCurricularId)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
        }
        public long TurmaId { get; set; }
        public string ComponenteCurricularId { get; set; }
    }
}
