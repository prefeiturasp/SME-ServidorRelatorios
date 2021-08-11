using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioRegistroIndividualUseCase : IRelatorioRegistroIndividualUseCase
    {
        private readonly IMediator mediator;

        public RelatorioRegistroIndividualUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {
            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioRegistroIndividualDto>();
            filtro.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroRegistroIndividual;

            var turma = await mediator.Send(new ObterComDreUePorTurmaIdQuery(parametros.TurmaId));
            if (turma == null)
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var ueEndereco = await mediator.Send(new ObterEnderecoUeEolPorCodigoQuery(long.Parse(turma.Ue.Codigo)));

            var alunosEol = await mediator.Send(new ObterAlunosReduzidosPorTurmaEAlunoQuery(turma.Codigo, parametros.AlunoCodigo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var alunosCodigos = alunosEol.Select(a => a.AlunoCodigo).ToArray();

            var registrosIndividuais = await mediator.Send(new ObterRegistrosIndividuaisPorTurmaEAlunoQuery(parametros.TurmaId, alunosCodigos, parametros.DataInicio, parametros.DataFim));
            if (registrosIndividuais == null || !registrosIndividuais.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var relatorioDto = await mediator.Send(new ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery(turma, ueEndereco, alunosEol, registrosIndividuais, parametros));

            await mediator.Send(new GerarRelatorioHtmlPDFRegistroIndividualCommand(relatorioDto, filtro.CodigoCorrelacao));
        }
    }
}
