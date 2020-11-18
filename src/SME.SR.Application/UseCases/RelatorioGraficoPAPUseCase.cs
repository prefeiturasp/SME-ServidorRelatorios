using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioGraficoPAPUseCase : IRelatorioGraficoPAPUseCase
    {
        private readonly IMediator mediator;

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

            if ((resultado == null || !resultado.Any()) && (encaminhamento == null || !encaminhamento.Any()))
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

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
                    grafico.Titulo = $"{resultado.EixoDescricao} - {objetivo.ObjetivoDescricao}" ;

                    foreach (var ano in objetivo.Anos)
                    {
                        var graficoAno = new ResumoPAPGraficoAnoDto(ano.AnoDescricao);

                        foreach (var resposta in ano.Respostas)
                        {
                            graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(resposta.Quantidade, (decimal)resposta.Porcentagem, resposta.RespostaDescricao));
                        }

                        var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                        graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);

                        grafico.Graficos.Add(graficoAno);
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
                    grafico.Titulo = $"{encaminhamento.EixoDescricao} - {objetivo.ObjetivoDescricao}" ;

                    foreach (var ano in objetivo.Anos)
                    {
                        var graficoAno = new ResumoPAPGraficoAnoDto(ano.AnoDescricao);

                        foreach (var resposta in ano.Respostas)
                        {
                            graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(resposta.Quantidade, (decimal)resposta.Porcentagem, resposta.RespostaDescricao));
                        }

                        var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                        graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);

                        grafico.Graficos.Add(graficoAno);
                    }

                    graficos.Add(grafico);
                }
            }
        }

        private void AdicionarTotalFrequencia(ResumoPAPTotalEstudantePorFrequenciaDto totalFrequencia, List<ResumoPAPGraficoDto> graficos)
        {
            var grafico = new ResumoPAPGraficoDto();
            grafico.Titulo = "Frequência";

            foreach (var frequencia in totalFrequencia.Frequencia)
            {
                foreach (var linha in frequencia.Linhas)
                {
                    var graficoAno = new ResumoPAPGraficoAnoDto("Frequência");

                    foreach (var ano in linha.Anos)
                    {
                        graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(ano.Quantidade, (decimal)ano.Porcentagem, ano.DescricaoAno));
                    }

                    var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

                    graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);

                    grafico.Graficos.Add(graficoAno);
                }

                graficos.Add(grafico);
            }
        }

        private void AdicionarTotalEstudantes(ResumoPAPTotalEstudantesDto totalEstudantes, List<ResumoPAPGraficoDto> graficos)
        {
            var grafico = new ResumoPAPGraficoDto();
            grafico.Titulo = "Total Estudantes";

            var graficoAno = new ResumoPAPGraficoAnoDto("Total Estudante");

            foreach (var ano in totalEstudantes.Anos)
            {
                graficoAno.EixosX.Add(new GraficoBarrasPAPVerticalEixoXDto(ano.Quantidade, (decimal)ano.Porcentagem, ano.AnoDescricao));
            }

            var valorMaximoEixo = graficoAno.EixosX.Max(a => int.Parse(a.Valor.ToString()));

            graficoAno.EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);

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
