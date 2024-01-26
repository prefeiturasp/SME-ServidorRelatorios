using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTipoNotaPorTurmaQueryHandler : IRequestHandler<ObterTipoNotaPorTurmaQuery, NotaTipoValor>
    {
        private readonly IMediator mediator;

        public ObterTipoNotaPorTurmaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<NotaTipoValor> Handle(ObterTipoNotaPorTurmaQuery request, CancellationToken cancellationToken)
        {
            if (request.Turma.ModalidadeCodigo.EhCelp())
                return new NotaTipoValor() { TipoNota = TipoNota.Conceito };

            var ciclo = await mediator.Send(new ObterCicloPorModalidadeQuery(request.Turma.Ano, request.Turma.ModalidadeCodigo));

            var dataAvaliacao = new DateTime(request.AnoLetivo, 3, 1);

            if (ciclo == null)
                throw new NegocioException("Não foi encontrado o ciclo da turma informada");

            var notaTipoValor = await mediator.Send(new ObterPorCicloIdDataAvalicacaoQuery(ciclo.Id, dataAvaliacao));

            return notaTipoValor;                                       
        }
    }
}
