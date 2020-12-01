using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTipoNotaPorTurmaQuery : IRequest<NotaTipoValor>
    {
        public ObterTipoNotaPorTurmaQuery(Turma turma, int anoLetivo)
        {
            this.turma = turma;
            AnoLetivo = anoLetivo;
        }

        public Turma turma { get; set; }
        public int AnoLetivo { get; set; }
    }
}
