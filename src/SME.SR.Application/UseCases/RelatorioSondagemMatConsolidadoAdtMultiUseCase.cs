using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemMatConsolidadoAdtMultiUseCase : IRelatorioSondagemMatConsolidadoAdtMultiUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemMatConsolidadoAdtMultiUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioSondagemComponentesMatematicaAditivoMultiplicativoConsolidadoDto>();

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

            var dataReferencia = DateTime.Now;

            if (filtros.AnoLetivo < 2022 && filtros.Semestre > 0)
                dataReferencia = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(filtros.Semestre, filtros.AnoLetivo));
            else if (filtros.AnoLetivo >= 2022 && filtros.Bimestre > 0)
                dataReferencia = await mediator.Send(new ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery(filtros.Bimestre, filtros.AnoLetivo));

            var quantidadeTotalAlunosUeAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(filtros.Ano, ue?.UeCodigo, filtros.AnoLetivo, dataReferencia, filtros.DreCodigo, new int[] { 5, 13 }));

            var relatorio = await mediator.Send(new ObterSondagemMatAditMultiConsolidadoQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                Dre = dre,
                Ue = ue,
                Semestre = filtros.Semestre,
                Bimestre = filtros.Bimestre,
                TurmaAno = int.Parse(filtros.Ano),
                Usuario = usuario,
                QuantidadeTotalAlunos = quantidadeTotalAlunosUeAno,
                Proficiencia = filtros.ProficienciaId
            });

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesMatematicaAditivoMultiplicativoConsolidado", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }
    }
}
