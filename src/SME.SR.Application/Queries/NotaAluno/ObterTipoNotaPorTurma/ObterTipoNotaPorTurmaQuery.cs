using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTipoNotaPorTurmaQuery : IRequest<NotaTipoValor>
    {
        public ObterTipoNotaPorTurmaQuery(Turma turma, int anoLetivo)
        {
            Turma = turma;
            AnoLetivo = anoLetivo;
        }

        public Turma Turma { get; set; }
        public int AnoLetivo { get; set; }
    }
}
