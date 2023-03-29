using MediatR;
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
    public class RelatorioSondagemPtConsolidadoLeitEscProdUseCase : IRelatorioSondagemPtConsolidadoLeitEscProdUseCase
    {
        private readonly IMediator mediator;

        private const int ANO_ESCOLAR_PROFICIENCIA_NIVEL = 3;

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

            var tipoRelatorio = !string.IsNullOrEmpty(filtros.GrupoId) ?
                (filtros.GrupoId == GrupoSondagemEnum.ProducaoTexto.Name() ? "Produção de texto" : "Leitura em voz alta") :
                (filtros.ProficienciaId == ProficienciaSondagemEnum.Leitura ? "Leitura" : "Escrita");
            GerarGrafico(relatorio, tipoRelatorio);

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidado", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private void GerarGrafico(RelatorioSondagemPortuguesConsolidadoRelatorioDto relatorio, string tipoRelatorio)
        {
            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();
            var grafico = new GraficoBarrasVerticalDto(800, $"Língua Portuguesa - {tipoRelatorio}");
            int chaveIndex = 0;
            var legendas = new List<GraficoBarrasLegendaDto>();

            foreach (var resposta in relatorio.Respostas)
            {
                var chave = Constantes.ListaChavesGraficos[chaveIndex].ToString();
                legendas.Add(new GraficoBarrasLegendaDto()
                {
                    Chave = chave,
                    Valor = resposta.Resposta
                });
                chaveIndex++;
                grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(resposta.Quantidade, chave));
            }
            var valorMaximoEixo = grafico.EixosX.Max(a => int.Parse(a.Valor.ToString()));
            grafico.Legendas = legendas;
            grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
            relatorio.GraficosBarras.Add(grafico);
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
                proficiencia = ProficienciaSondagemEnum.Escrita.Name();
            }
            else if (proficiencia == ProficienciaSondagemEnum.Leitura.Name())
            {
                proficiencia = ProficienciaSondagemEnum.Leitura.Name();
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
                EhProducaoTexto = filtros.GrupoId == GrupoSondagemEnum.ProducaoTexto.Name()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoRespostaDto>> ObterRespostasGrupo(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            GrupoSondagemEnum grupoSondagemEnum = filtros.GrupoId == GrupoSondagemEnum.LeituraVozAlta.Name() ?
                GrupoSondagemEnum.LeituraVozAlta : GrupoSondagemEnum.ProducaoTexto;

            var semestre = (filtros.Bimestre <= 2) ? 1 : 2;

            var dataReferencia = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, filtros.AnoLetivo));

            int alunosPorAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(
                filtros.Ano.ToString(),
                filtros.UeCodigo,
                filtros.AnoLetivo,
                dataReferencia,
                Convert.ToInt64(filtros.DreCodigo),
                filtros.Modalidades
                ));

            var periodo = await mediator.Send(new ObterPeriodoPorTipoQuery(filtros.Bimestre, TipoPeriodoSondagem.Bimestre));
            var perguntas = await mediator.Send(new ObterPerguntasPorGrupoQuery(grupoSondagemEnum, ComponenteCurricularSondagemEnum.Portugues));
            var dados = await mediator.Send(new ObterRespostasPorFiltrosQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                TurmaAno = filtros.Ano,
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                GrupoId = filtros.GrupoId,
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues,
                PeriodoId = periodo.Id
            });

            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>();

            if (dados == null || !dados.Any())
            {
                PreencherPerguntasForaLista(respostas, perguntas);

                ObterSemPreenchimento(dados, alunosPorAno, respostas);

                return respostas;
            }

            PopularListaRetorno(dados, alunosPorAno, perguntas, respostas);

            return respostas;
        }

        private void PopularListaRetorno(IEnumerable<SondagemAutoralDto> dados, int alunosPorAno, IEnumerable<PerguntasOrdemGrupoAutoralDto> perguntas, List<RelatorioSondagemPortuguesConsolidadoRespostaDto> respostas)
        {
            foreach (var pergunta in perguntas)
            {
                AdicionarPerguntaSeNaoExistir(respostas, pergunta, dados, alunosPorAno);
            }

            ObterSemPreenchimento(dados, alunosPorAno, respostas);
            PreencherPerguntasForaLista(respostas, perguntas);
        }

        private void AdicionarPerguntaSeNaoExistir(List<RelatorioSondagemPortuguesConsolidadoRespostaDto> respostas, PerguntasOrdemGrupoAutoralDto pergunta, IEnumerable<SondagemAutoralDto> dados, int alunosPorAno)
        {
            if (respostas.Any(x => x.Id.Equals(pergunta.PerguntaId)))
                return;

            var alunosPergunta = dados.Where(x => x.PerguntaId == pergunta.PerguntaId)
                                                .Select(x => x.CodigoAluno)
                                                .GroupBy(x => x).OrderBy(x => x.Key);

            var quantidadeAlunosPergunta = alunosPergunta.Select(x => x.Key).Count();

            respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostaDto
            {
                Id = pergunta.PerguntaId,
                Resposta = pergunta.Pergunta,
                Total = alunosPorAno,
                Quantidade = quantidadeAlunosPergunta,
                Percentual = Math.Round(((decimal)quantidadeAlunosPergunta / (decimal)alunosPorAno) * 100, 2)
            });
        }

        private void ObterSemPreenchimento(IEnumerable<SondagemAutoralDto> dados, int alunosPorAno, List<RelatorioSondagemPortuguesConsolidadoRespostaDto> respostas)
        {
            var alunosUnicos = dados.GroupBy(x => x.CodigoAluno);

            var quantidadeAlunosUnicos = alunosUnicos.Select(x => x.Key).Count();

            var diferenca = alunosPorAno - quantidadeAlunosUnicos;

            if (diferenca <= 0) return;

            respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostaDto
            {
                Resposta = "Sem preenchimento",
                Total = alunosPorAno,
                Quantidade = diferenca,
                Percentual = Math.Round(((decimal)diferenca / (decimal)alunosPorAno) * 100, 2)
            });
        }

        private void PreencherPerguntasForaLista(List<RelatorioSondagemPortuguesConsolidadoRespostaDto> respostas, IEnumerable<PerguntasOrdemGrupoAutoralDto> perguntas)
        {
            foreach (var pergunta in perguntas)
            {
                if (respostas.Any(x => x.Id?.Equals(pergunta.PerguntaId) ?? false))
                    return;

                respostas.Add(new RelatorioSondagemPortuguesConsolidadoRespostaDto
                {
                    Id = pergunta.PerguntaId,
                    Resposta = pergunta.Pergunta,
                    Percentual = 0,
                    Quantidade = 0,
                    Total = 0
                });
            }
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoRespostaDto>> ObterRespostasProficiencia(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto> linhasSondagem = null;

            linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Bimestre = filtros.Bimestre,
                Proficiencia = filtros.ProficienciaId
            });

            var dataReferencia = await mediator
                .Send(new ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery(filtros.Bimestre, filtros.AnoLetivo));

            int alunosPorAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(
                filtros.Ano.ToString(),
                filtros.UeCodigo,
                filtros.AnoLetivo,
                dataReferencia,
                Convert.ToInt64(filtros.DreCodigo),
                filtros.Modalidades));

            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>();

            var respAgrupado = linhasSondagem
                .GroupBy(o => o.Resposta).Select(g => new { Label = g.Key, Value = g.Count() }).OrderBy(r => r.Label).ToList();

            int totalRespostas = 0;

            if(respAgrupado.Any())
                respAgrupado.Where(r => !string.IsNullOrWhiteSpace(r.Label)).Sum(r => r.Value);

            foreach (var item in respAgrupado)
            {
                RelatorioSondagemPortuguesConsolidadoRespostaDto itemRetorno = new RelatorioSondagemPortuguesConsolidadoRespostaDto();

                int quantidadeRespostas = item.Value;

                if (string.IsNullOrWhiteSpace(item.Label))
                    quantidadeRespostas = alunosPorAno - respAgrupado.Where(r => !string.IsNullOrWhiteSpace(r.Label)).Sum(r => r.Value);

                itemRetorno.Resposta = MontarTextoProficiencia(item.Label);
                itemRetorno.Quantidade = quantidadeRespostas;
                itemRetorno.Percentual = Math.Round(((decimal)quantidadeRespostas / (decimal)alunosPorAno) * 100, 2);
                itemRetorno.Total = alunosPorAno;
                respostas.Add(itemRetorno);                
            }            

            if (alunosPorAno > totalRespostas && !respostas.Any(x => x.Percentual == 100))
            {
                var totalSemPreenchimento = alunosPorAno - totalRespostas;

                RelatorioSondagemPortuguesConsolidadoRespostaDto itemRetorno = new RelatorioSondagemPortuguesConsolidadoRespostaDto();

                itemRetorno.Resposta = MontarTextoProficiencia(string.Empty);
                itemRetorno.Quantidade = totalSemPreenchimento;
                itemRetorno.Percentual = Math.Round(((decimal)totalSemPreenchimento / (decimal)alunosPorAno) * 100, 2);
                itemRetorno.Total = alunosPorAno;
                respostas.Add(itemRetorno);
            }

            if (filtros.ProficienciaId == ProficienciaSondagemEnum.Escrita && filtros.Ano != ANO_ESCOLAR_PROFICIENCIA_NIVEL)
                respostas = OrdenarRespostasEscrita(respostas);

            return respostas;
        }

        private List<RelatorioSondagemPortuguesConsolidadoRespostaDto> OrdenarRespostasEscrita(List<RelatorioSondagemPortuguesConsolidadoRespostaDto> listaRespostas)
        {
            var listaOrdenadaRespostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>();

            var opcao1 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Pré-Silábico");
            if (opcao1 != null)
                listaOrdenadaRespostas.Add(opcao1);

            var opcao2 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Silábico sem valor");
            if (opcao2 != null)
                listaOrdenadaRespostas.Add(opcao2);

            var opcao3 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Silábico com valor");
            if (opcao3 != null)
                listaOrdenadaRespostas.Add(opcao3);

            var opcao4 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Silábico alfabético");
            if (opcao4 != null)
                listaOrdenadaRespostas.Add(opcao4);

            var opcao5 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Alfabético");
            if (opcao5 != null)
                listaOrdenadaRespostas.Add(opcao5);

            var opcao6 = listaRespostas.FirstOrDefault(lr => lr.Resposta == "Sem Preenchimento");
            if (opcao6 != null)
                listaOrdenadaRespostas.Add(opcao6);

            return listaOrdenadaRespostas;
        }

        private string MontarTextoProficiencia(string proficiencia)
        {
            switch (proficiencia)
            {
                case "":
                    return "Sem Preenchimento";
                case "PS":
                    return "Pré-Silábico";
                case "SSV":
                    return "Silábico sem valor";
                case "SCV":
                    return "Silábico com valor";
                case "SA":
                    return "Silábico alfabético";
                case "A":
                    return "Alfabético";
                default:
                    return proficiencia;
            }
        }
    }
}
