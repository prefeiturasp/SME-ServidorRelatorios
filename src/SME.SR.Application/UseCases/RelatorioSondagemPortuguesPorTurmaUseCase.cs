using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPortuguesPorTurmaUseCase : IRelatorioSondagemPortuguesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemPortuguesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesPorTurmaFiltroDto>();

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(filtros.Semestre, filtros.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(filtros.TurmaCodigo, dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var relatorioPerguntas = await ObterPerguntas(filtros);

            var relatorioCabecalho = await ObterCabecalho(filtros, dataDoPeriodo, relatorioPerguntas);

            var relatorio = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery() { 
                Alunos = alunosDaTurma,
                Cabecalho = relatorioCabecalho,
                Ano = filtros.Ano,
                AnoLetivo = filtros.AnoLetivo,
                ComponenteCurricular = filtros.ComponenteCurricularId,
                DreCodigo = filtros.DreCodigo,
                Proficiencia = filtros.ProficienciaId,
                Semestre = filtros.Semestre,
                TurmaCodigo = filtros.TurmaCodigo,
                UeCodigo = filtros.UeCodigo,
                UsuarioRF = filtros.UsuarioRF,
            });

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem de Português ({relatorio.Cabecalho.Proficiencia}) da turma {relatorio.Cabecalho.Turma} da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre}) - {relatorio.Cabecalho.Turma}";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }

        private async Task<RelatorioSondagemPortuguesPorTurmaCabecalhoDto> ObterCabecalho(
            RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, DateTime periodo,
            List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            return await Task.FromResult(new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = filtros.DreCodigo,
                Periodo = periodo.ToString("dd/MM/yyyy"),
                Rf = filtros.UsuarioRF,
                Turma = filtros.TurmaCodigo.ToString(),
                Ue = filtros.UeCodigo,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                Perguntas = perguntas,
                Ano = filtros.Ano,
                ComponenteCurricular = filtros.ComponenteCurricularId.ToString(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>> ObterPerguntas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros)
        {
            switch (filtros.ProficienciaId)
            {
                case ProficienciaSondagemEnum.Leitura:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = 1,
                                Nome = "Proficiência"
                            },
                        });
                case ProficienciaSondagemEnum.Escrita:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = 1,
                                Nome = ""
                            },
                        });
                case ProficienciaSondagemEnum.LeituraVozAlta:
                    return await Task.FromResult(
                        new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                            {
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 1,
                                    Nome = "Não conseguiu ou não quis ler"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 2,
                                    Nome = "Leu com muita dificuldade"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 3,
                                    Nome = "Leu com alguma fluência	"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 4,
                                    Nome = "Leu com fluência"
                                },
                            });
                default:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
            }
        }
    }
}
