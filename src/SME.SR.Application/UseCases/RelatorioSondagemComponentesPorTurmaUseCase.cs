using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemComponentesPorTurmaUseCase : IRelatorioSondagemComponentesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemComponentesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(RelatorioSondagemComponentesPorTurmaFiltroDto request)
        {
            var relatorioSondagemComponentesPorTurmaRelatorioDto = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                #region Mock
                Cabecalho = new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
                {
                    Ano = 2020,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now,
                    Dre = "DRE - BT",
                    Periodo = "1º Semestre",
                    Proficiencia = "Campo Aditivo",
                    Rf = "987987",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "master",
                    Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 1,
                            Nome = "ORDEM 1 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 2,
                            Nome = "ORDEM 2 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 3,
                            Nome = "ORDEM 3 - COMPOSIÇÃO"
                        },
                    },
                },
                Planilha = new RelatorioSondagemComponentesPorTurmaPlanilhaDto()
                {
                    Linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6197654,
                                Nome = "ALEXIA FERNANDES LIMA",
                                SituacaoMatricula = SituacaoMatriculaAluno.Ativo,
                            },
                            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Ideia = "Errou",
                                    Resultado = "Errou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Ideia = "Acertou",
                                    Resultado = "Errou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 3,
                                    Ideia = "Errou",
                                    Resultado = "Acertou"
                                },
                            },
                        },
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6195479,
                                Nome = "ALICE SILVA RIBEIRO",
                                SituacaoMatricula = SituacaoMatriculaAluno.Desistente,
                            },
                            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Ideia = "Acertou",
                                    Resultado = "Acertou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Ideia = "Acertou",
                                    Resultado = "Errou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 3,
                                    Ideia = "Errou",
                                    Resultado = "Acertou"
                                },
                            },
                        },
                        new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto() {
                            Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                            {
                                Codigo = 6197654,
                                Nome = "AMANDA ALBUQUERQUE",
                                SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu,
                            },
                            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 1,
                                    Ideia = "Acertou",
                                    Resultado = "Errou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 2,
                                    Ideia = "Acertou",
                                    Resultado = "Acertou"
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 3,
                                    Ideia = "Errou",
                                    Resultado = "Acertou"
                                },
                            },
                        },
                    }
                }
                #endregion
            };

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesPorTurma", relatorioSondagemComponentesPorTurmaRelatorioDto, request.CodigoCorrelacao));
        }
    }
}
