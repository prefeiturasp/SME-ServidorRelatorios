﻿using MediatR;
using SME.SR.Application.Queries;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
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
        private const int SEGUNDO_BIMESTRE = 2;
        private const int PRIMEIRO_SEMESTRE = 1;
        private const int SEGUNDO_SEMESTRE = 2;
        public RelatorioSondagemPortuguesConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();
            var consideraNovaOpcaoRespostaSemPreenchimento = await mediator.Send(new UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(filtros.Semestre,filtros.Bimestre,filtros.AnoLetivo));

            if (filtros.GrupoId != GrupoSondagemEnum.CapacidadeLeitura.Name())
                throw new NegocioException($"{ filtros.GrupoId } fora do esperado.");

            var semestre = ObterSemestrePeriodo(filtros);
            var periodoCompleto = await ObterDatasPeriodoFixoAnual(0, semestre, filtros.AnoLetivo);

            var alunosPorAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(
                filtros.Ano.ToString(),
                filtros.UeCodigo,
                filtros.AnoLetivo,
                periodoCompleto.PeriodoFim,
                Convert.ToInt64(filtros.DreCodigo),
                filtros.Modalidades,
                true, 
                periodoCompleto.PeriodoInicio));

            var relatorio = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros),
                Planilhas = await ObterPlanilhas(filtros, alunosPorAno,consideraNovaOpcaoRespostaSemPreenchimento)
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            GerarGrafico(relatorio, 0,consideraNovaOpcaoRespostaSemPreenchimento);

            return await mediator
                .Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoCapacidadeLeitura", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private int ObterSemestrePeriodo(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            if (filtros.Bimestre == 0) return filtros.Semestre;
            return filtros.Bimestre <= SEGUNDO_BIMESTRE ? PRIMEIRO_SEMESTRE : SEGUNDO_SEMESTRE;
        }

        private async Task<PeriodoCompletoSondagemDto> ObterDatasPeriodoFixoAnual(int bimestre, int semestre, int anoLetivo)
        {
            if (bimestre != 0)
                return await mediator.Send(new ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery(bimestre, anoLetivo));
            return await mediator.Send(new ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery(semestre, anoLetivo));
        }

        private static void GerarGrafico(RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto relatorio, int qtdAlunos,bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();
            foreach (var ordem in relatorio.Planilhas)
            {
                foreach (var pergunta in ordem.Perguntas)
                {
                    var legendas = new List<GraficoBarrasLegendaDto>();
                    var grafico = new GraficoBarrasVerticalDto(420, $"{ordem.Ordem} - {pergunta.Pergunta}");

                    var chaveIndex = 0;
                    var chave = string.Empty;

                    foreach (var resposta in pergunta.Respostas)
                    {
                        chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                        legendas.Add(new GraficoBarrasLegendaDto()
                        {
                            Chave = chave,
                            Valor = resposta.Resposta
                        });

                        grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(resposta.Quantidade, chave));
                    }

                    var totalRespostas = (int)grafico.EixosX.Sum(e => e.Valor);

                    if (!consideraNovaOpcaoRespostaSemPreenchimento)
                    {
                        var qtdSemPreenchimento = qtdAlunos - totalRespostas;

                        if (qtdSemPreenchimento > 0)
                        {
                            chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();

                            legendas.Add(new GraficoBarrasLegendaDto()
                            {
                                Chave = chave,
                                Valor = "Sem preenchimento"
                            });

                            grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qtdSemPreenchimento, chave));
                        }
                    }

                    var valorMaximoEixo = grafico.EixosX.Any() ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                    grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                    grafico.Legendas = legendas;

                    relatorio.GraficosBarras.Add(grafico);
                }
            }
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            var dreAbreviacao = "Todas";
            if (filtros.DreCodigo != null && filtros.DreCodigo != "0")
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                dreAbreviacao = dre.Abreviacao;
            }

            var ueNomeComTipoEscola = "Todas";
            if (filtros.UeCodigo != null & filtros.UeCodigo != String.Empty)
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                ueNomeComTipoEscola = ue.NomeComTipoEscola;
            }

            var proficiencia = !string.IsNullOrEmpty(filtros.GrupoId) ? filtros.GrupoId : filtros.ProficienciaId.ToString();
            if (proficiencia == GrupoSondagemEnum.CapacidadeLeitura.Name())
            {
                proficiencia = GrupoSondagemEnum.CapacidadeLeitura.ShortName();
            }
            else if (proficiencia == GrupoSondagemEnum.LeituraVozAlta.Name())
            {
                proficiencia = GrupoSondagemEnum.LeituraVozAlta.ShortName();
            }
            else if (proficiencia == GrupoSondagemEnum.ProducaoTexto.Name())
            {
                proficiencia = GrupoSondagemEnum.ProducaoTexto.ShortName();
            }

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dreAbreviacao,
                Periodo = $"{Math.Max(filtros.Bimestre, filtros.Semestre)}° {(filtros.Bimestre != 0 ? "Bimestre" : "Semestre")}",
                Rf = filtros.UsuarioRF,
                Ue = ueNomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Turma = "Todas",
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.ShortName(),
                Proficiencia = proficiencia
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>> ObterPlanilhas(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros, int alunosPorAno
           ,bool consideraNovaOpcaoRespostaSemPreenchimento , GrupoSondagemEnum grupoSondagemEnum = GrupoSondagemEnum.CapacidadeLeitura)
        {
            var periodo = await mediator.Send(new ObterPeriodoPorTipoQuery(Math.Max(filtros.Semestre, filtros.Bimestre), filtros.Semestre != 0 ? TipoPeriodoSondagem.Semestre : TipoPeriodoSondagem.Bimestre));

            var linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Grupo = grupoSondagemEnum, 
                PeriodoId = periodo.Id
            });

            var planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();

            var ordens = linhasSondagem.GroupBy(o => o.Ordem).Select(x => x.FirstOrDefault()).ToList();
            if (ordens.Count == 0)
            {
                var respostasDto = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>();

                if (!consideraNovaOpcaoRespostaSemPreenchimento)
                {
                    respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = alunosPorAno,
                        Total = alunosPorAno,
                        Percentual = 1
                    });
                }

                var ordensSondagem = await mediator.Send(new ObterOrdensSondagemPorGrupoQuery() { Grupo = grupoSondagemEnum });
                foreach (var ordem in ordensSondagem)
                {
                    planilhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
                    {
                        Ordem = ordem.Descricao,
                        Perguntas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
                            {
                                Pergunta = string.Empty,
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

                    var respostas = linhasSondagem.Where(o => o.Ordem == ordem.Ordem && o.Pergunta == pergunta.Pergunta).ToList();
                    var totalRespostas = respostas.Sum(o => o.Quantidade);
                    var totalAlunos = alunosPorAno >= totalRespostas ? alunosPorAno : totalRespostas;

                    foreach (var resposta in respostas)
                    {
                        respostasDto.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                        {
                            Resposta = resposta.Resposta,
                            Quantidade = resposta.Quantidade,
                            Total = totalAlunos,
                            Percentual = decimal.Divide(resposta.Quantidade, totalAlunos)
                        });
                    }

                    if (!consideraNovaOpcaoRespostaSemPreenchimento)
                    {
                        var dtoSemPreenchimento = respostasDto.Find(resp => resp.Resposta == "Sem preenchimento"); 
                        
                        if (dtoSemPreenchimento == null)
                        {
                            dtoSemPreenchimento = new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto();
                            respostasDto.Add(dtoSemPreenchimento);
                        }  
   
                        dtoSemPreenchimento.Resposta = "Sem preenchimento";
                        dtoSemPreenchimento.Quantidade = totalAlunos - totalRespostas;
                        dtoSemPreenchimento.Total = alunosPorAno;
                        dtoSemPreenchimento.Percentual = decimal.Divide(totalAlunos - totalRespostas, totalAlunos);
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
