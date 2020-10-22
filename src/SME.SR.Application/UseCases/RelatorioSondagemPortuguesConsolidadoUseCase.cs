using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPortuguesConsolidadoUseCase : IRelatorioSondagemPortuguesConsolidadoUseCase
    {
        private readonly IMediator mediator;
        public RelatorioSondagemPortuguesConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();

            if (filtros.ProficienciaId != ProficienciaSondagemEnum.Leitura)
                throw new NegocioException($"{ filtros.ProficienciaId } fora do esperado.");

            RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto relatorio = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros),
                Planilhas = await ObterPlanilhas(filtros)
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem Consolidado de Português ({relatorio.Cabecalho.Proficiencia}) da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem Consolidado (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoLeitura", relatorio, Guid.NewGuid(), mensagemDaNotificacao, mensagemTitulo, false));
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });
            var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = $"{ filtros.Bimestre }° Bimestre",
                Rf = filtros.UsuarioRF,
                Ue = ue.NomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.AnoTurma,
                Turma = "Todas",
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.Name(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>> ObterPlanilhas(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto> linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.AnoTurma,
                Bimestre = filtros.Bimestre,
            });

            var planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();

            var ordens = linhasSondagem.GroupBy(o => o.Ordem).Select(x => x.FirstOrDefault());
            foreach (var ordem in ordens)
            {
                var perguntasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>();

                var perguntas = linhasSondagem.GroupBy(o => new {o.Ordem, o.Pergunta }).Select(x => x.FirstOrDefault());
                foreach (var pergunta in perguntas)
                {
                    var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();

                    var respostas = linhasSondagem.Where(o => o.Ordem == ordem.Ordem && o.Pergunta == pergunta.Pergunta);
                    foreach (var resposta in respostas)
                    {
                        var totalRespostas = respostas.Sum(o => o.Quantidade);
                        respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                        {
                            Resposta = resposta.Resposta,
                            Quantidade = resposta.Quantidade,
                            Total = totalRespostas,
                            Percentual = resposta.Quantidade / totalRespostas
                        });
                    }

                    perguntasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
                    {
                        Pergunta = pergunta.Pergunta,
                        Respostas = respostasDto
                    });
                }

                planilhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
                {
                    Ordem = ordem.Ordem,
                    Perguntas = perguntasDto
                });
            }
        
            return await Task.FromResult(planilhas);
        }
    }
}
