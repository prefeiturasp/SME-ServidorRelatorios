using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFrequenciaUseCase : IRelatorioAcompanhamentoFrequenciaUseCase
    {
        public readonly IMediator mediator;
        public RelatorioAcompanhamentoFrequenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFrequencia;
            var codigoAlunosTodos = new List<string>();
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroAcompanhamentoFrequenciaJustificativaDto>();
            var relatorio = new RelatorioFrequenciaIndividualDto();
            await MapearCabecalho(relatorio, filtroRelatorio);
            if (filtroRelatorio.AlunosCodigo.Contains("-99"))
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery() { TurmaCodigo = filtroRelatorio.TurmaCodigo });
                foreach (var item in alunos)
                {
                    codigoAlunosTodos.Add(item.CodigoAluno.ToString());
                }
            }
            else
                codigoAlunosTodos = filtroRelatorio.AlunosCodigo;

            var dadosFrequencia = await mediator.Send(new ObterFrequenciaAlunoPorCodigoBimestreQuery(filtroRelatorio.Bimestre, codigoAlunosTodos.ToArray()));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("AcompanhamentoFrequencia", relatorio, request.CodigoCorrelacao));
        }
        private async Task MapearCabecalho(RelatorioFrequenciaIndividualDto relatorio, FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio)
        {
            relatorio.RF = filtroRelatorio.UsuarioRF;
            relatorio.Usuario = filtroRelatorio.UsuarioNome;
            relatorio.DreNome = await ObterNomeDre(filtroRelatorio.DreCodigo);
            relatorio.UeNome = await ObterNomeUe(filtroRelatorio.UeCodigo);
        }

        private async Task<string> ObterNomeDre(string dreCodigo)
        {
            var dre = await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));
            return dre.Abreviacao;
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
            return ue.NomeRelatorio ;
        }

    }
}
