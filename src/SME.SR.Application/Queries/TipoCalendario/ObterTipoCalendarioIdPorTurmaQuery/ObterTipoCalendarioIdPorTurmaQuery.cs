using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTipoCalendarioIdPorTurmaQuery: IRequest<long>
    {
        public ObterTipoCalendarioIdPorTurmaQuery(Turma turma)
        {
            Turma = turma;
        }

        public Turma Turma { get; set; }
    }
}
