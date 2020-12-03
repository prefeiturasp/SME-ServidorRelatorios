using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAtribuicaoCJUseCase : IRelatorioAtribuicaoCJUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAtribuicaoCJUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioAtribuicaoCJDto>();

            if (filtros.ExibirAtribuicoesExporadicas)
            {
                var lstAtribuicaoEsporadica = await mediator.Send(new ObterAtribuicoesEsporadicasPorFiltroQuery(filtros.AnoLetivo,
                                                                                                                filtros.UsuarioRf,
                                                                                                                filtros.DreCodigo,
                                                                                                                filtros.UsuarioRf,
                                                                                                                filtros.DreCodigo));

                var lstServidores = lstAtribuicaoEsporadica.Select(s => s.ProfessorRf);

                var cargosServidor = await mediator.Send(new ObterCargosServidoresPorAnoLetivoQuery(filtros.AnoLetivo, lstServidores.ToArray()));
            }

            var lstAtribuicaoCJ = await mediator.Send(new ObterAtribuicoesCJPorFiltroQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                DreCodigo = filtros.DreCodigo,
                UeId = filtros.DreCodigo,
                UsuarioRf = filtros.UsuarioRf,
                UsuarioNome = filtros.UsuarioRf
            });

        }
    }
}
