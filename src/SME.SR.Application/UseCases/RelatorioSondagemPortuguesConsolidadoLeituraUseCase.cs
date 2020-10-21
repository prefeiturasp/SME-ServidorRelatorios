using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class RelatorioSondagemPortuguesConsolidadoLeituraUseCase : IRelatorioSondagemPortuguesConsolidadoLeituraUseCase
    {
        private readonly IMediator mediator;
        public RelatorioSondagemPortuguesConsolidadoLeituraUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();

            var semestre = (filtros.Bimestre <= 2) ? 1 : 2;

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, filtros.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(Int32.Parse(filtros.TurmaCodigo), dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto relatorio = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros, dataDoPeriodo),
                Planilhas = await ObterPlanilhas(filtros, alunosDaTurma)
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem de Português ({relatorio.Cabecalho.Proficiencia}) da turma {relatorio.Cabecalho.Turma} da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre}) - {relatorio.Cabecalho.Turma}";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoLeitura", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros, DateTime periodo)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = filtros.DreCodigo,
                Periodo = periodo.ToString("dd/MM/yyyy"),
                Rf = filtros.UsuarioRF,
                Turma = filtros.TurmaCodigo.ToString(),
                Ue = filtros.UeCodigo,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.AnoTurma,
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.Name(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>> ObterPlanilhas(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros, IEnumerable<Aluno> alunos)
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
