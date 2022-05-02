using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioGraficoPAPUseCase : IRelatorioGraficoPAPUseCase
    {
        private readonly IMediator mediator;
        private readonly char[] lstChaves = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public RelatorioGraficoPAPUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioResumoPAPDto>();

            // Obter dados de dre e ue
            var dreUe = await ObterDadosDreUe(filtros);

            // Obter dados de dre e ue
            var turma = await ObterDadosTurma(filtros);

            var ciclo = await ObterCiclo(filtros);

            var periodo = await ObterPeriodo(filtros);

            var totalEstudantes = await ObterTotalEstudantes(filtros);

            var totalFrequencia = await ObterTotalFrequencia(filtros);

            var encaminhamento = await ObterEncaminhamento(filtros);

            var resultado = await ObterResultados(filtros);

            var graficos = ObterGraficos(totalEstudantes, totalFrequencia, encaminhamento, resultado);

            var relatorioGraficoPAPDto = new GraficoPAPDto()
            {
                Ano = filtros.Ano != "0" ? filtros.Ano : "Todos",
                AnoLetivo = filtros.AnoLetivo,
                Ciclo = ciclo.Descricao,
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                DreNome = dreUe.DreNome,
                EhEncaminhamento = (filtros.Periodo == (int)PeriodoRecuperacaoParalela.Encaminhamento),
                Periodo = periodo.Nome,
                GraficosDto = graficos,
                Turma = turma.Nome,
                UeNome = dreUe.UeNome,
                UsuarioNome = filtros.UsuarioNome,
                UsuarioRF = filtros.UsuarioRf
            };

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioGraficosPAP", relatorioGraficoPAPDto, request.CodigoCorrelacao));
        }

        private List<ResumoPAPGraficoDto> ObterGraficos(ResumoPAPTotalEstudantesDto totalEstudantes, ResumoPAPTotalEstudantePorFrequenciaDto totalFrequencia, IEnumerable<ResumoPAPTotalResultadoDto> encaminhamento, IEnumerable<ResumoPAPTotalResultadoDto> resultado)
        {
            var graficos = new List<ResumoPAPGraficoDto>();

            if (totalEstudantes != null)
            {
                AdicionarTotalEstudantes(totalEstudantes, graficos);
            }

            if (totalFrequencia != null)
            {
                AdicionarTotalFrequencia(totalFrequencia, graficos);
            }

            if (encaminhamento != null && encaminhamento.Any())
            {
                AdicionarEncaminhamento(encaminhamento, graficos);
            }

            if (resultado != null && resultado.Any())
            {
                AdicionarResultado(resultado, graficos);
            }

            return graficos;
        }

        private void AdicionarResultado(IEnumerable<ResumoPAPTotalResultadoDto> resultados, List<ResumoPAPGraficoDto> graficos)
        {
            foreach (var resultado in resultados)
            {
                foreach (var objetivo in resultado.Objetivos)
                {
                    var grafico = new ResumoPAPGraficoDto();

                    grafico.Titulo = $"{resultado.EixoDescricao} - {objetivo.ObjetivoDescricao}";

                    foreach (var ano in objetivo.Anos)
                    {
                        var graficoAno = new ResumoPAPGraficoAnoDto(420, $"{ano.AnoDescricao} ANO");
                        var legendas = new List<GraficoBarrasLegendaDto>();
                        int chaveIndex = 0;

                        foreach (var resposta in ano.Respostas.Where(r => r.Quantidade > 0)?.OrderBy(o => o.Ordem))
                        {
                            string chave = lstChaves[chaveIndex++].ToString();

                            legendas.Add(new GraficoBarrasLegendaDto()
                            {
                                Chave = chave,
                                Valor = $"{resposta.RespostaNome}"
                            });

                            graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(resposta.Quantidade, (decimal)Math.Round(resposta.Porcentagem, 0), chave));
                        }

                        if (graficoAno.EixosX.Any())
                        {
                            var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                            graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                            graficoAno.Legendas = legendas;

                            grafico.Graficos.Add(graficoAno);
                        }
                    }

                    graficos.Add(grafico);
                }
            }
        }

        private void AdicionarEncaminhamento(IEnumerable<ResumoPAPTotalResultadoDto> encaminhamentos, List<ResumoPAPGraficoDto> graficos)
        {
            foreach (var encaminhamento in encaminhamentos)
            {
                foreach (var objetivo in encaminhamento.Objetivos)
                {
                    var grafico = new ResumoPAPGraficoDto();

                    grafico.Titulo = $"{encaminhamento.EixoDescricao} - {objetivo.ObjetivoDescricao}";

                    foreach (var ano in objetivo.Anos)
                    {
                        var legendas = new List<GraficoBarrasLegendaDto>();
                        var graficoAno = new ResumoPAPGraficoAnoDto(420, $"{ano.AnoDescricao} ANO");
                        int chaveIndex = 0;

                        foreach (var resposta in ano.Respostas.Where(r => r.Quantidade > 0)?.OrderBy(o => o.Ordem))
                        {
                            string chave = lstChaves[chaveIndex++].ToString();

                            legendas.Add(new GraficoBarrasLegendaDto()
                            {
                                Chave = chave,
                                Valor = $"{resposta.RespostaNome}"
                            });

                            graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(resposta.Quantidade, (decimal)Math.Round(resposta.Porcentagem, 0), chave));
                        }

                        if (graficoAno.EixosX.Any())
                        {

                            var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                            graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                            graficoAno.Legendas = legendas;

                            grafico.Graficos.Add(graficoAno);
                        }
                    }

                    graficos.Add(grafico);
                }
            }
        }

        private void AdicionarTotalFrequencia(ResumoPAPTotalEstudantePorFrequenciaDto totalFrequencia, List<ResumoPAPGraficoDto> graficos)
        {
            var grafico = new ResumoPAPGraficoDto();
            grafico.Titulo = "Frequência";

            var anos = totalFrequencia.Frequencia.SelectMany(f => f.Linhas)
                         .SelectMany(f => f.Anos).Select(f => new { f.CodigoAno, f.DescricaoAno })
                         .DistinctBy(f => f.CodigoAno);

            foreach (var ano in anos)
            {
                var graficoAno = new ResumoPAPGraficoAnoDto(420, $"{ano.DescricaoAno} ANO");
                var legendas = new List<GraficoBarrasLegendaDto>();
                int chaveIndex = 0;

                foreach (var frequencia in totalFrequencia.Frequencia)
                {
                    if (frequencia.FrequenciaDescricao.ToLower() != "total")
                    {
                        string chave = lstChaves[chaveIndex++].ToString();

                        legendas.Add(new GraficoBarrasLegendaDto()
                        {
                            Chave = chave,
                            Valor = $"{frequencia.FrequenciaDescricao}"
                        });

                        var itemFrequencia = frequencia.Linhas.FirstOrDefault().Anos.FirstOrDefault(d => d.CodigoAno == ano.CodigoAno);

                        if (itemFrequencia == null)
                            itemFrequencia = new ResumoPAPTotalFrequenciaAnoDto() { Quantidade = 0, Porcentagem = 0 };

                        graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(itemFrequencia.Quantidade, (decimal)Math.Round(itemFrequencia.Porcentagem, 0), chave));
                    }
                
                }

                var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                graficoAno.Legendas = legendas;

                grafico.Graficos.Add(graficoAno);
            }

            graficos.Add(grafico);
        }

        private void AdicionarTotalEstudantes(ResumoPAPTotalEstudantesDto totalEstudantes, List<ResumoPAPGraficoDto> graficos)
        {
            var grafico = new ResumoPAPGraficoDto();
            var legendas = new List<GraficoBarrasLegendaDto>();
            int chaveIndex = 0;

            grafico.Titulo = "Total Estudantes";

            var graficoAno = new ResumoPAPGraficoAnoDto(420, "Total Estudante");

            foreach (var ano in totalEstudantes.Anos)
            {
                string chave = lstChaves[chaveIndex++].ToString();

                legendas.Add(new GraficoBarrasLegendaDto()
                {
                    Chave = chave,
                    Valor = $"{ano.AnoDescricao}"
                });

                graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(ano.Quantidade, (decimal)Math.Round(ano.Porcentagem, 0), chave));
            }

            var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

            graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
            graficoAno.Legendas = legendas;

            grafico.Graficos.Add(graficoAno);

            graficos.Add(grafico);
        }

        private async Task<IEnumerable<ResumoPAPTotalResultadoDto>> ObterResultados(FiltroRelatorioResumoPAPDto filtros)
        {
            return await mediator.Send(
               new ListarTotalResultadoQuery()
               {
                   Periodo = filtros.Periodo,
                   DreId = filtros.DreId,
                   UeId = filtros.UeId,
                   CicloId = filtros.CicloId,
                   TurmaId = filtros.TurmaId,
                   Ano = filtros.Ano,
                   AnoLetivo = filtros.AnoLetivo
               });
        }

        private async Task<IEnumerable<ResumoPAPTotalResultadoDto>> ObterEncaminhamento(FiltroRelatorioResumoPAPDto filtros)
        {
            if (filtros.Periodo.HasValue && filtros.Periodo.Value != (int)PeriodoRecuperacaoParalela.Encaminhamento) return null;

            return await mediator.Send(
                new ListarTotalResultadoEncaminhamentoQuery()
                {
                    Periodo = filtros.Periodo,
                    DreId = filtros.DreId,
                    UeId = filtros.UeId,
                    CicloId = filtros.CicloId,
                    TurmaId = filtros.TurmaId,
                    Ano = filtros.Ano,
                    AnoLetivo = filtros.AnoLetivo
                });
        }

        private async Task<ResumoPAPTotalEstudantePorFrequenciaDto> ObterTotalFrequencia(FiltroRelatorioResumoPAPDto filtros)
        {
            return await mediator.Send(
               new ListarTotalAlunosPorFrequenciaQuery()
               {
                   Periodo = filtros.Periodo,
                   DreId = filtros.DreId,
                   UeId = filtros.UeId,
                   CicloId = filtros.CicloId,
                   TurmaId = filtros.TurmaId,
                   Ano = filtros.Ano,
                   AnoLetivo = filtros.AnoLetivo
               });
        }

        private async Task<ResumoPAPTotalEstudantesDto> ObterTotalEstudantes(FiltroRelatorioResumoPAPDto filtros)
        {
            return await mediator.Send(
                 new ListarTotalAlunosSeriesQuery()
                 {
                     Periodo = filtros.Periodo,
                     DreId = filtros.DreId,
                     UeId = filtros.UeId,
                     CicloId = filtros.CicloId,
                     TurmaId = filtros.TurmaId,
                     Ano = filtros.Ano,
                     AnoLetivo = filtros.AnoLetivo
                 });
        }



        private async Task<RecuperacaoParalelaPeriodoDto> ObterPeriodo(FiltroRelatorioResumoPAPDto filtros)
        {
            if (filtros.Periodo.HasValue && filtros.Periodo.Value > 0)
            {
                var periodo = await mediator.Send(new ObterRecuperacaoParalelaPeriodoPorIdQuery() { RecuperacaoParalelaPeriodoId = (long)filtros.Periodo.Value });

                if (periodo == null)
                    throw new NegocioException($"Não foi possível localizar dados do período");
                return periodo;
            }
            else
            {
                return new RecuperacaoParalelaPeriodoDto() { Nome = "Todos" };
            }
        }

        private async Task<TipoCiclo> ObterCiclo(FiltroRelatorioResumoPAPDto filtros)
        {
            if (filtros.CicloId.HasValue && filtros.CicloId.Value > 0)
            {
                var ciclo = await mediator.Send(new ObterCicloPorIdQuery() { CicloId = (long)filtros.CicloId.Value });

                if (ciclo == null)
                    throw new NegocioException($"Não foi possível localizar dados do ciclo");
                return ciclo;
            }
            else
            {
                return new TipoCiclo() { Descricao = "Todos" };
            }
        }

        private async Task<Turma> ObterDadosTurma(FiltroRelatorioResumoPAPDto filtros)
        {
            if (!string.IsNullOrEmpty(filtros.TurmaId) && filtros.TurmaId != "0")
            {
                var turma = await mediator.Send(new ObterTurmaQuery() { CodigoTurma = filtros.TurmaId });

                if (turma == null)
                    throw new NegocioException($"Não foi possível localizar dados da turma");
                return turma;
            }
            else
            {
                return new Turma() { Nome = "Todas" };
            }
        }

        private async Task<DreUe> ObterDadosDreUe(FiltroRelatorioResumoPAPDto filtros)
        {
            DreUe dreUe = new DreUe();

            if (!string.IsNullOrEmpty(filtros.DreId) && filtros.DreId != "0")
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreId });

                if (dre != null)
                {
                    dreUe.DreCodigo = dre.Codigo;
                    dreUe.DreId = dre.Id;
                    dreUe.DreNome = dre.Abreviacao;
                }
            }
            else
            {
                dreUe.DreNome = "Todas";
            }

            if (!string.IsNullOrEmpty(filtros.UeId) && filtros.UeId != "0")
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeId));

                if (ue != null)
                {
                    dreUe.UeCodigo = ue.Codigo;
                    dreUe.UeId = ue.Id;
                    dreUe.UeNome = ue.NomeComTipoEscola;
                }
            }
            else
            {
                dreUe.UeNome = "Todas";
            }

            if (dreUe == null)
                throw new NegocioException($"Não foi possível localizar dados do Dre e Ue");
            return dreUe;
        }
    }
}
