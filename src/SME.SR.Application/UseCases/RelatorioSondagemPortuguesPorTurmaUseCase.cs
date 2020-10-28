using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesPorTurmaFiltroDto>();

            if (filtros.ProficienciaId == ProficienciaSondagemEnum.Autoral && filtros.GrupoId != GrupoSondagemEnum.LeituraVozAlta.Name())
                throw new NegocioException("Grupo fora do esperado.");

            var semestre = (filtros.Bimestre <= 2) ? 1 : 2;

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, filtros.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(Int32.Parse(filtros.TurmaCodigo), dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var relatorioPerguntas = await ObterPerguntas(filtros);

            RelatorioSondagemPortuguesPorTurmaRelatorioDto relatorio = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros, relatorioPerguntas, dataDoPeriodo),
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = await ObterLinhas(filtros, alunosDaTurma)
                }
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<RelatorioSondagemPortuguesPorTurmaCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas, DateTime periodo)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });
            var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
            var turma = await mediator.Send(new ObterTurmaSondagemEolPorCodigoQuery(Int32.Parse(filtros.TurmaCodigo)));

            return await Task.FromResult(new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = $"{ filtros.Bimestre }° Bimestre",
                Rf = filtros.UsuarioRF,
                Turma = turma.Nome,
                Ue = ue.NomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                Perguntas = perguntas,
                AnoTurma = filtros.Ano,
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.ShortName(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>> ObterPerguntas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros)
        {
            switch (filtros.ProficienciaId)
            {
                case ProficienciaSondagemEnum.Leitura:
                case ProficienciaSondagemEnum.Escrita:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = String.Empty,
                                Nome = "Proficiência"
                            },
                        });
                case ProficienciaSondagemEnum.Autoral:
                    if (filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name())
                    {
                        return await Task.FromResult(
                            new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                                {
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "0bf845cc-29dc-45ec-8bf2-8981cef616df",
                                    Nome = "Não conseguiu ou não quis ler"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "49c26883-e717-44aa-9aab-1bd8aa870916",
                                    Nome = "Leu com muita dificuldade"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "0b38221a-9d50-4cdf-abbd-a9ac092dbe70",
                                    Nome = "Leu com alguma fluência"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = "18d148be-d83c-4f24-9d03-dc003a05b9e4",
                                    Nome = "Leu com fluência"
                                },
                                });
                    } else return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
                default:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
            }
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>> ObterLinhas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, IEnumerable<Aluno> alunos)
        {
            IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto> linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Bimestre = filtros.Bimestre,
                Proficiencia = filtros.ProficienciaId,
                Grupo = GrupoSondagemEnum.LeituraVozAlta
            });

            List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto> linhasPlanilha = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();
            foreach (Aluno aluno in alunos.OrderBy(a => a.NomeAluno).ToList())
            {
                var alunoDto = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                {
                    Codigo = aluno.CodigoAluno,
                    Nome = aluno.ObterNomeParaRelatorioSondagem(),
                    DataSituacao = aluno.DataSituacao.ToString("dd/MM/yyyy"),
                    SituacaoMatricula = aluno.SituacaoMatricula
                };

                var respostasDto = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>();

                var perguntas = await ObterPerguntas(filtros);
                foreach (var pergunta in perguntas)
                {
                    var resposta = linhasSondagem.FirstOrDefault(a => a.AlunoEolCode == aluno.CodigoAluno.ToString() && a.PerguntaId == pergunta.Id);
                    respostasDto.Add(new RelatorioSondagemPortuguesPorTurmaRespostaDto()
                    {
                        PerguntaId = pergunta.Id,
                        Resposta = resposta?.Resposta
                    });

                }


                linhasPlanilha.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = alunoDto,
                    Respostas = respostasDto
                });
            }
            return await Task.FromResult(linhasPlanilha);
        }
    }
}
