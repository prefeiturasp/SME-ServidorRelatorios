using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPtPorTurmaCapLeituraUseCase : IRelatorioSondagemPtPorTurmaCapLeituraUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemPtPorTurmaCapLeituraUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioSondagemPortuguesCapacidadeLeituraDto>();

            Dre dre = null;
            Ue ue = null;
            Usuario usuario = null;

            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                if (ue == null)
                    throw new NegocioException("Não foi possível obter a UE.");
            }

            if (filtros.DreCodigo > 0)
            {
                dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo.ToString() });
                if (dre == null)
                    throw new NegocioException("Não foi possível obter a DRE.");
            }

            if (!string.IsNullOrEmpty(filtros.UsuarioRf))
            {
                usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRf });
                if (usuario == null)
                    throw new NegocioException("Não foi possível obter o usuário.");
            }

            var relatorio = await mediator.Send(new ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                Dre = dre,
                Ue = ue,
                TurmaAno = int.Parse(filtros.Ano),
                Usuario = usuario,
                TurmaCodigo = filtros.TurmaCodigo,
                Bimestre = filtros.Bimestre,
                Semestre = filtros.Semestre,
            });

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesCapacidadeLeituraPorTurma", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }
    }
}
