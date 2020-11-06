using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPtConsolidadoLeitEscProdUseCase : IRelatorioSondagemPtConsolidadoLeitEscProdUseCase
    {
        private readonly IMediator mediator;
        public RelatorioSondagemPtConsolidadoLeitEscProdUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();

            var respostas = !String.IsNullOrEmpty(filtros.GrupoId) ? await ObterRespostasGrupo(filtros) : await ObterRespostasProficiencia(filtros);

            RelatorioSondagemPortuguesConsolidadoRelatorioDto relatorio = new RelatorioSondagemPortuguesConsolidadoRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros),
                Respostas = respostas
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoCapacidadeLeitura", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            string dreAbreviacao = "Todas";
            if (filtros.DreCodigo != null && filtros.DreCodigo != "0")
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                dreAbreviacao = dre.Abreviacao;
            }

            string ueNomeComTipoEscola = "Todas";
            if (filtros.UeCodigo != null & filtros.UeCodigo != String.Empty)
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                ueNomeComTipoEscola = ue.NomeComTipoEscola;
            }

            var proficiencia = !String.IsNullOrEmpty(filtros.GrupoId) ? filtros.GrupoId : filtros.ProficienciaId.ToString();
            if (proficiencia == GrupoSondagemEnum.LeituraVozAlta.Name())
            {
                proficiencia = GrupoSondagemEnum.LeituraVozAlta.ShortName();
            }
            else if (proficiencia == GrupoSondagemEnum.ProducaoTexto.Name())
            {
                proficiencia = GrupoSondagemEnum.ProducaoTexto.ShortName();
            }
            else if (proficiencia == ProficienciaSondagemEnum.Escrita.Name())
            {
                proficiencia = ProficienciaSondagemEnum.Escrita.ShortName();
            }
            else if (proficiencia == ProficienciaSondagemEnum.Leitura.Name())
            {
                proficiencia = ProficienciaSondagemEnum.Leitura.ShortName();
            }

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dreAbreviacao,
                Periodo = $"{ filtros.Bimestre }° Bimestre",
                Rf = filtros.UsuarioRF,
                Ue = ueNomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Turma = "Todas",
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.ShortName(),
                Proficiencia = proficiencia,
                EhProducaoTexto = proficiencia == GrupoSondagemEnum.ProducaoTexto.Name()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoRespostasDto>> ObterRespostasGrupo(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto> linhasSondagem = null;

            GrupoSondagemEnum grupoSondagemEnum = filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name() ?
                GrupoSondagemEnum.LeituraVozAlta : GrupoSondagemEnum.LeituraVozAlta;

            linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Bimestre = filtros.Bimestre,
                Grupo = grupoSondagemEnum
            });

            int alunosPorAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(
                filtros.Ano.ToString(),
                filtros.UeCodigo,
                filtros.AnoLetivo,
                DateTime.Now,
                Convert.ToInt64(filtros.DreCodigo)
                ));

            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostasDto>();

            var ordens = linhasSondagem.GroupBy(o => o.Ordem).Select(x => x.FirstOrDefault()).ToList();
            if (ordens.Count == 0)
            {
                var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();
                respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                {
                    Resposta = "Sem preenchimento",
                    Quantidade = alunosPorAno,
                    Total = alunosPorAno,
                    Percentual = 1
                });

                var ordensSondagem = await mediator.Send(new ObterOrdensSondagemPorGrupoQuery() { Grupo = grupoSondagemEnum });
                foreach (var ordem in ordensSondagem)
                {
                    respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostasDto()
                    {
                        Respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>()
                        {
                            new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                            {
                                 Pergunta = String.Empty,
                                Respostas = respostasDto
                            }
                        }
                    });
                }
            }

            foreach (var ordem in ordens)
            {
                var perguntasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>();

                var perguntas = linhasSondagem.Where(o => o.Ordem == ordem.Ordem).GroupBy(p => p.Pergunta).Select(x => x.FirstOrDefault()).ToList();
                foreach (var pergunta in perguntas)
                {
                    var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();

                    var respostasSondagem = linhasSondagem.Where(o => o.Ordem == ordem.Ordem && o.Pergunta == pergunta.Pergunta).ToList();
                    var totalRespostas = respostasSondagem.Sum(o => o.Quantidade);
                    foreach (var resposta in respostasSondagem)
                    {
                        respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                        {
                            Resposta = resposta.Resposta,
                            Quantidade = resposta.Quantidade,
                            Total = alunosPorAno,
                            Percentual = Decimal.Divide(resposta.Quantidade, alunosPorAno)
                        });
                    }

                    respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = alunosPorAno - totalRespostas,
                        Total = alunosPorAno,
                        Percentual = Decimal.Divide(alunosPorAno - totalRespostas, alunosPorAno)
                    });

                    perguntasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
                    {
                        Pergunta = pergunta.Pergunta,
                        Respostas = respostasDto
                    });
                }

                respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostasDto()
                {
                    Ordem = ordem.Ordem,
                    Perguntas = perguntasDto
                });
            }

            return await Task.FromResult(respostas);
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoRespostasDto>> ObterRespostasProficiencia(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto> linhasSondagem = null;

            linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Bimestre = filtros.Bimestre,
                Proficiencia = filtros.ProficienciaId
            });

            int alunosPorAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(
                filtros.Ano.ToString(),
                filtros.UeCodigo,
                filtros.AnoLetivo,
                DateTime.Now,
                Convert.ToInt64(filtros.DreCodigo)
                ));

            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostasDto>();

            var ordens = linhasSondagem.GroupBy(o => o.Ordem).Select(x => x.FirstOrDefault()).ToList();
            if (ordens.Count == 0)
            {
                var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();
                respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                {
                    Resposta = "Sem preenchimento",
                    Quantidade = alunosPorAno,
                    Total = alunosPorAno,
                    Percentual = 1
                });

                var ordensSondagem = await mediator.Send(new ObterOrdensSondagemPorGrupoQuery() { Grupo = grupoSondagemEnum });
                foreach (var ordem in ordensSondagem)
                {
                    respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostasDto()
                    {
                        Respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>()
                        {
                            new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                            {
                                 Pergunta = String.Empty,
                                Respostas = respostasDto
                            }
                        }
                    });
                }
            }

            foreach (var ordem in ordens)
            {
                var perguntasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>();

                var perguntas = linhasSondagem.Where(o => o.Ordem == ordem.Ordem).GroupBy(p => p.Pergunta).Select(x => x.FirstOrDefault()).ToList();
                foreach (var pergunta in perguntas)
                {
                    var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();

                    var respostasSondagem = linhasSondagem.Where(o => o.Ordem == ordem.Ordem && o.Pergunta == pergunta.Pergunta).ToList();
                    var totalRespostas = respostasSondagem.Sum(o => o.Quantidade);
                    foreach (var resposta in respostasSondagem)
                    {
                        respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                        {
                            Resposta = resposta.Resposta,
                            Quantidade = resposta.Quantidade,
                            Total = alunosPorAno,
                            Percentual = Decimal.Divide(resposta.Quantidade, alunosPorAno)
                        });
                    }

                    respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = alunosPorAno - totalRespostas,
                        Total = alunosPorAno,
                        Percentual = Decimal.Divide(alunosPorAno - totalRespostas, alunosPorAno)
                    });

                    perguntasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
                    {
                        Pergunta = pergunta.Pergunta,
                        Respostas = respostasDto
                    });
                }

                respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostasDto()
                {
                    Ordem = ordem.Ordem,
                    Perguntas = perguntasDto
                });
            }

            return await Task.FromResult(respostas);
        }
    }
}
