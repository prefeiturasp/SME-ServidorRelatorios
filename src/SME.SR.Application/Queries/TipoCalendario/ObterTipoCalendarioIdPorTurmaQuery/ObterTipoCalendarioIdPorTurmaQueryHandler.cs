using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTipoCalendarioIdPorTurmaQueryHandler: IRequestHandler<ObterTipoCalendarioIdPorTurmaQuery, long>
    {
        private readonly ITipoCalendarioRepository repositorioTipoCalendario;
        public ObterTipoCalendarioIdPorTurmaQueryHandler(ITipoCalendarioRepository repositorioTipoCalendario)
        {
            this.repositorioTipoCalendario = repositorioTipoCalendario ?? throw new ArgumentNullException(nameof(repositorioTipoCalendario));
        }

        public async Task<long> Handle(ObterTipoCalendarioIdPorTurmaQuery request, CancellationToken cancellationToken)
            => await repositorioTipoCalendario.ObterIdPorAnoLetivoEModalidadeAsync(request.Turma.AnoLetivo
                    , request.Turma.ModalidadeTipoCalendario
                    , request.Turma.Semestre);
    }
}
