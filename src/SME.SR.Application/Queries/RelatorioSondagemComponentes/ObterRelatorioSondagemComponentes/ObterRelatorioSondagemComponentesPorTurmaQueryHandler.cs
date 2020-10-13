using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, RelatorioSondagemComponentesPorTurmaRelatorioDto>
    {
        private readonly IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(
            IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository,
            IUsuarioRepository usuarioRepository,
            IMediator mediator)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            RelatorioSondagemComponentesPorTurmaCabecalhoDto cabecalho = await ObterCabecalho(request);
            RelatorioSondagemComponentesPorTurmaPlanilhaDto planilha = (Int32.Parse(request.Ano) >= 7)? await ObterPlanilhaAutoral(request, cabecalho.Perguntas) : await ObterPlanilha(request);

            return new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                Cabecalho = cabecalho,
                Planilha = planilha
            };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaCabecalhoDto> ObterCabecalho(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var componenteCurricular = request.ComponenteCurricular.Name();
            var ordens = await ObterOrdens(request.Ano, request.Proficiencia);
            var ue = await ObterUe(request.UeCodigo);
            var usuario = await usuarioRepository.ObterDados(request.UsuarioRF);
            var perguntas = await ObterPerguntas(request.Proficiencia, request.Ano);
            var dre = await ObterDre(request.DreCodigo);
            var proficiencia = request.Proficiencia.Name();
            var turma = await mediator.Send(new ObterTurmaSondagemEolPorCodigoQuery(request.TurmaCodigo));

            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = request.Ano,
                AnoLetivo = request.AnoLetivo,
                ComponenteCurricular = componenteCurricular,
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = $"{request.Semestre.ToString()}° Semestre",
                Proficiencia = proficiencia,
                Turma = turma.NomeRelatorio,
                Ue = ue.NomeComTipoEscola,
                Rf = request.UsuarioRF,
                Usuario = usuario.Nome,
                Ordens = ordens.ToList(),
                Perguntas = perguntas
            };
        }

        private async Task<Dre> ObterDre(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = dreCodigo });
        }

        private async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdens(string ano, ProficienciaSondagemEnum proficiencia)
        {
            return await mediator.Send(new ObterOrdensSondagemPorAnoProficienciaQuery(ano, proficiencia));

        }

        private async Task<Ue> ObterUe(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }

        public async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntas(ProficienciaSondagemEnum proficiencia, string ano)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo || proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Ideia"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Resultado"
                    }
                });
            }

            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Familiares ou Frequentes"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Opacos"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 3,
                        Nome = "Transparentes"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 4,
                        Nome = "Terminam em Zero"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 5,
                        Nome = "Algarismos Iguais"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 6,
                        Nome = "Processo de Generalização"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 7,
                        Nome = "Zero Intercalado"
                    },
                });
            }

            if (proficiencia == ProficienciaSondagemEnum.Escrita || proficiencia == ProficienciaSondagemEnum.Leitura)
            {
                return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Problema de lógica"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Área e perímetro"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 3,
                        Nome = (ano == "8")?"Triângulos e quadriláteros":"Sólidos geométricos"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 4,
                        Nome = (ano == "9")?"Regularidade e generalização":"Relações entre grandezas e porcentagem"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 5,
                        Nome = (ano == "7")?"Média, moda e mediana":"Probabilidade"
                    },
                });
            }

            return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>());
        }

        public async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilhaAutoral(ObterRelatorioSondagemComponentesPorTurmaQuery request, List<RelatorioSondagemComponentesPorTurmaPerguntaDto> perguntas)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo.ToString(), request.AnoLetivo, request.Semestre, request.Proficiencia);

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var listaRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                foreach (RelatorioSondagemComponentesPorTurmaPerguntaDto pergunta in perguntas)
                {
                    RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto resposta = listaSondagem.FirstOrDefault(r => r.AlunoEolCode == aluno.CodigoAluno.ToString() && r.PerguntaId == pergunta.Id);
                    if (resposta != null)
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 1,
                            PerguntaId = pergunta.Id,
                            Resposta = resposta.Resposta,
                        });
                    } else
                    {
                        listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                        {
                            OrdemId = 1,
                            PerguntaId = pergunta.Id,
                            Resposta = String.Empty,
                        });
                    }
                }

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = listaRespostas
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        public async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilha(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var listaSondagem = await relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo.ToString(), request.AnoLetivo, request.Semestre, request.Proficiencia);

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var aluno in request.alunos.OrderBy(a => a.ObterNomeFinal()))
            {
                var respostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

                var respostaDoAluno = listaSondagem.FirstOrDefault(a => a.AlunoEolCode == aluno.CodigoAluno.ToString());

                if (respostaDoAluno != null)
                    respostas = await ObterOrdemRespostas(respostaDoAluno, request.Ano, request.Proficiencia);

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = TransformarAlunoDto(aluno),
                    OrdensRespostas = respostas
                });

            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private RelatorioSondagemComponentesPorTurmaAlunoDto TransformarAlunoDto(Aluno aluno)
        {
            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = aluno.CodigoAluno,
                DataSituacao = aluno.DataSituacao.ToString("dd/MM/yyyy"),
                Nome = aluno.ObterNomeParaRelatorioSondagem(),
                SituacaoMatricula = aluno.SituacaoMatricula
            };

        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>> ObterOrdemRespostas(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, string ano, ProficienciaSondagemEnum proficiencia)
        {

            var listaRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

            switch (ano)
            {
                case "1":
                    ObterRespostasAno1(linha, listaRespostas, proficiencia);
                    break;
                case "2":
                    ObterRespostasAno2(linha, listaRespostas, proficiencia);
                    break;
                case "3":
                    ObterRespostasAno3(linha, listaRespostas, proficiencia);
                    break;
                case "4":
                    ObterRespostasAno4(linha, listaRespostas, proficiencia);
                    break;
                case "5":
                    ObterRespostasAno5(linha, listaRespostas, proficiencia);
                    break;
                case "6":
                    ObterRespostasAno6(linha, listaRespostas, proficiencia);
                    break;
                default:
                    break;
            }

            return await Task.FromResult(listaRespostas);
        }
        private static void ObterRespostasNumeros(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas)
        {
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 1,
                Resposta = linha.Familiares,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 2,
                Resposta = linha.Opacos,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 3,
                Resposta = linha.Transparentes,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 4,
                Resposta = linha.TerminamZero,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 5,
                Resposta = linha.Algarismos,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 6,
                Resposta = linha.Processo,
            });
            listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
            {
                OrdemId = 1,
                PerguntaId = 7,
                Resposta = linha.ZeroIntercalados,
            });
        }

        private static void ObterRespostasAno1(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
            }
        }

        private static void ObterRespostasAno2(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
            } else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
            }
        }

        private static void ObterRespostasAno3(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                ObterRespostasNumeros(linha, listaRespostas);
            }
            else if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
            } else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha.Ordem4Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha.Ordem4Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha.Ordem5Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha.Ordem5Resultado,
                });
            }
        }

        private static void ObterRespostasAno4(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha.Ordem4Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha.Ordem4Resultado,
                });
            } else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha.Ordem5Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha.Ordem5Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha.Ordem6Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha.Ordem6Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha.Ordem7Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha.Ordem7Resultado,
                });
            }
        }

        private static void ObterRespostasAno5(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha.Ordem4Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha.Ordem4Resultado,
                });
            } else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha.Ordem5Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha.Ordem5Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha.Ordem6Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha.Ordem6Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha.Ordem7Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha.Ordem7Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 1,
                    Resposta = linha.Ordem8Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 2,
                    Resposta = linha.Ordem8Resultado,
                });
            }
        }
        
        private static void ObterRespostasAno6(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha, List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> listaRespostas, ProficienciaSondagemEnum proficiencia)
        {
            if (proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 1,
                    Resposta = linha.Ordem1Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 1,
                    PerguntaId = 2,
                    Resposta = linha.Ordem1Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 1,
                    Resposta = linha.Ordem2Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 2,
                    PerguntaId = 2,
                    Resposta = linha.Ordem2Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 1,
                    Resposta = linha.Ordem3Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 3,
                    PerguntaId = 2,
                    Resposta = linha.Ordem3Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 1,
                    Resposta = linha.Ordem4Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 4,
                    PerguntaId = 2,
                    Resposta = linha.Ordem4Resultado,
                });
            } else if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
            {
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 1,
                    Resposta = linha.Ordem5Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 5,
                    PerguntaId = 2,
                    Resposta = linha.Ordem5Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 1,
                    Resposta = linha.Ordem6Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 6,
                    PerguntaId = 2,
                    Resposta = linha.Ordem6Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 1,
                    Resposta = linha.Ordem7Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 7,
                    PerguntaId = 2,
                    Resposta = linha.Ordem7Resultado,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 1,
                    Resposta = linha.Ordem8Ideia,
                });
                listaRespostas.Add(new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto()
                {
                    OrdemId = 8,
                    PerguntaId = 2,
                    Resposta = linha.Ordem8Resultado,
                });

            }
        }

    }
}
