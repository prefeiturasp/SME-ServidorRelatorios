using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Sondagem;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterSondagemMatNumAutoralConsolidadoQueryHandler : IRequestHandler<ObterSondagemMatNumAutoralConsolidadoQuery, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto>
    {
        private readonly IMathPoolNumbersRepository mathPoolNumbersRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;
        private readonly ISondagemAutoralRepository sondagemAutoralRepository;

        public ObterSondagemMatNumAutoralConsolidadoQueryHandler(IMathPoolNumbersRepository mathPoolNumbersRepository, IPerguntasAutoralRepository perguntasAutoralRepository, ISondagemAutoralRepository sondagemAutoralRepository)
        {
            this.mathPoolNumbersRepository = mathPoolNumbersRepository ?? throw new ArgumentNullException(nameof(mathPoolNumbersRepository));
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new ArgumentNullException(nameof(perguntasAutoralRepository));
            this.sondagemAutoralRepository = sondagemAutoralRepository ?? throw new ArgumentNullException(nameof(sondagemAutoralRepository));
        }

        public async Task<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto> Handle(ObterSondagemMatNumAutoralConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto();
            var perguntas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto>();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Bimestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            if (request.TurmaAno > 3 || request.AnoLetivo >= 2022)
            {
                var totalDeAlunos = request.QuantidadeTotalAlunos;
                var listaPeguntaResposta = await sondagemAutoralRepository.ObterSondagemPerguntaRespostaConsolidadoBimestre(request.Dre?.Codigo, request.Ue?.Codigo, (request.Bimestre > 0 ? request.Bimestre : request.Semestre), request.TurmaAno, request.AnoLetivo, ComponenteCurricularSondagemEnum.Matematica.Name());
                var relatorioAgrupado = listaPeguntaResposta.GroupBy(p => p.PerguntaId).ToList();

                relatorioAgrupado.ForEach(x =>
                {
                    var pergunta = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto();
                    var totalRespostas = x.Where(y => y.PerguntaId == x.Key).Sum(q => q.QtdRespostas);
                    totalDeAlunos = totalRespostas > totalDeAlunos ? totalRespostas : totalDeAlunos;

                    CalculaPercentualTotalPergunta(totalDeAlunos, x.Where(y => y.PerguntaId == x.Key).First().PerguntaDescricao, pergunta);

                    var listaPr = x.Where(y => y.PerguntaId == x.Key).ToList();
                    CalculaPercentualRespostas(totalDeAlunos, pergunta, listaPr, totalRespostas);

                    perguntas.Add(pergunta);
                });
            }
            else
            {
                var listaAlunos = await mathPoolNumbersRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                var familiaresAgrupados = listaAlunos.GroupBy(fu => fu.Familiares);

                var opacosAgrupados = listaAlunos.GroupBy(fu => fu.Opacos);

                var transparentesAgrupados = listaAlunos.GroupBy(fu => fu.Transparentes);

                var terminamZeroAgrupados = listaAlunos.GroupBy(fu => fu.TerminamZero);

                var algarismosAgrupados = listaAlunos.GroupBy(fu => fu.Algarismos);

                var processoAgrupados = listaAlunos.GroupBy(fu => fu.Processo);

                var zeroIntercaladosAgrupados = listaAlunos.GroupBy(fu => fu.ZeroIntercalados);

                AdicionarPergunta(familiaresAgrupados, grupo: "Familiares/Frequentes", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(opacosAgrupados, grupo: "Opacos", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(transparentesAgrupados, grupo: "Transparentes", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(terminamZeroAgrupados, grupo: "Terminam em zero", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(algarismosAgrupados, grupo: "Algarismos iguais", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(processoAgrupados, grupo: "Processo de generalização", perguntas, request.QuantidadeTotalAlunos);
                AdicionarPergunta(zeroIntercaladosAgrupados, grupo: "Zero intercalado", perguntas, request.QuantidadeTotalAlunos);
            }


            if (perguntas.Any() && request.AnoLetivo < 2022)
            {
                perguntas.ForEach(pergunta => pergunta.Respostas = pergunta.Respostas.OrderBy(r => r.Resposta).ToList());
            }

            relatorio.PerguntasRespostas = perguntas;

            TrataAlunosQueNaoResponderam(relatorio, request.QuantidadeTotalAlunos);
            GerarGraficos(relatorio);

            return relatorio;
        }

        private void GerarGraficos(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio)
        {
            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            foreach (var pergunta in relatorio.PerguntasRespostas)
            {
                string chave = String.Empty;
                int chaveIndex = 0;
                var grafico = new GraficoBarrasVerticalDto(420, $"{pergunta.Pergunta}");
                var legendas = new List<GraficoBarrasLegendaDto>();

                foreach (var resposta in pergunta.Respostas)
                {
                    chave = Constantes.ListaChavesGraficos[chaveIndex++].ToString();
                    legendas.Add(new GraficoBarrasLegendaDto()
                    {
                        Chave = chave,
                        Valor = resposta.Resposta
                    });

                    grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(decimal.Parse(resposta.AlunosQuantidade.ToString()), chave));
                }
                var valorMaximoEixo = grafico.EixosX.Count() > 0 ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(340, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                grafico.Legendas = legendas;
                relatorio.GraficosBarras.Add(grafico);
            }
        }

        private void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, int quantidadeTotalAlunos)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var qntDeAlunosPreencheu = perguntaResposta.Respostas.Sum(a => a.AlunosQuantidade);
                var diferencaPreencheuNao = quantidadeTotalAlunos - qntDeAlunosPreencheu;

                var percentualNaoPreencheu = diferencaPreencheuNao == 0 && quantidadeTotalAlunos == 0 ? 0 : ((double)diferencaPreencheuNao / quantidadeTotalAlunos) * 100;

                var existePerguntasSemPreenchimento = perguntaResposta.Respostas.FirstOrDefault(p => p.Resposta == "Sem preenchimento");

                if (existePerguntasSemPreenchimento == null)
                {
                    perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        AlunosQuantidade = diferencaPreencheuNao,
                        AlunosPercentual = percentualNaoPreencheu

                    });
                }
            }
        }

        private void MontarCabecalho(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, int? bimestre, string rf, string usuario)
        {
            relatorio.Ano = anoTurma;
            relatorio.AnoLetivo = anoLetivo;
            relatorio.ComponenteCurricular = "Matemática";
            relatorio.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.Dre = dre != null ? dre.Abreviacao : "Todas";
            relatorio.Periodo = anoLetivo >= 2022 && bimestre > 0 ? $"{bimestre}º Bimestre" : $"{semestre}º Semestre";
            relatorio.Proficiencia = int.Parse(anoTurma) > 3 ? "" : "Números";
            relatorio.RF = rf;
            relatorio.Turma = "Todas";
            relatorio.Ue = ue != null ? ue.NomeComTipoEscola : "Todas";
            relatorio.Usuario = usuario;
        }

        private void AdicionarPergunta(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento, string grupo, List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> perguntas, int TotalAlunosGeral)
        {
            var respostas = ObterRespostas(agrupamento, TotalAlunosGeral);

            if (respostas != null && respostas.Any())
            {
                perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
                {
                    Pergunta = grupo,
                    Respostas = respostas
                });
            }
            else
            {
                perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
                {
                    Pergunta = grupo,
                    Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>()
                    {
                        new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                        {
                            AlunosPercentual = 100,
                            AlunosQuantidade = TotalAlunosGeral,
                            Resposta = "Sem preenchimento"
                        }
                    }
                });
            }
        }

        private void AdicionarPergunta(IGrouping<string, PerguntasAutoralDto> pergunta, string grupo, IGrouping<string, SondagemAutoralDto> respostasAlunos, List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> perguntas, int totalAlunosGeral)
        {
            var respostas = ObterRespostas(pergunta, respostasAlunos, totalAlunosGeral);

            if (respostas != null && respostas.Any())
            {
                perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
                {
                    Pergunta = pergunta.FirstOrDefault(a => a.PerguntaId == grupo).Pergunta,
                    Respostas = respostas
                });
            }
        }

        private List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento, int TotalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento.Where(a => !a.Key.Trim().Equals(""));

            //var totalAlunos = agrupamentosComValor.SelectMany(g => g).Count();

            foreach (var item in agrupamentosComValor)
            {
                respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                {
                    Resposta = item.Key.Equals("S", StringComparison.InvariantCultureIgnoreCase) ? "Escreve de forma convencional" : "Não escreve de forma convencional",
                    AlunosQuantidade = item.Count(),
                    AlunosPercentual = ((double)item.Count() / TotalAlunosGeral) * 100
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> ObterRespostas(IGrouping<string, PerguntasAutoralDto> pergunta, IGrouping<string, SondagemAutoralDto> agrupamento, int TotalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento?.Where(a => !string.IsNullOrEmpty(a.RespostaId));

            var totalAlunos = agrupamentosComValor?.Count() ?? 0;

            var agrupamentosComValorAgrupado = agrupamentosComValor?.GroupBy(g => g.RespostaId);

            foreach (var item in pergunta)
            {
                var totalAlunosResposta = agrupamentosComValorAgrupado?.FirstOrDefault(a => a.Key == item.RespostaId)?.Count() ?? 0;

                respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                {
                    Resposta = item.Resposta,
                    AlunosQuantidade = totalAlunosResposta,
                    AlunosPercentual = totalAlunosResposta > 0 ? ((double)totalAlunosResposta / TotalAlunosGeral) * 100 : 0
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private void CalculaPercentualTotalPergunta(int totalDeAlunos, string descricaoPergunta, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto pergunta)
        {
            pergunta.Pergunta = descricaoPergunta;
            pergunta.Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();
        }

        private void CalculaPercentualRespostas(int totalDeAlunos, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto pergunta, List<PerguntasRespostasDTO> listaPr, int totalRespostas)
        {
            foreach (var item in listaPr)
            {
                pergunta.Respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                {
                    Resposta = item.RespostaDescricao,
                    AlunosQuantidade = item.QtdRespostas,
                    AlunosPercentual = item.QtdRespostas > 0 ? ((double)item.QtdRespostas / totalDeAlunos) * 100 : 0
                });
            }

            var respostaSempreenchimento = CriaRespostaSemPreenchimento(totalDeAlunos, totalRespostas);
            pergunta.Respostas.Add(respostaSempreenchimento);
        }

        private RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto CriaRespostaSemPreenchimento(int totalDeAlunos, int quantidadeTotalRespostasPergunta)
        {
            var respostaSemPreenchimento = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto();
            var quantidade = totalDeAlunos - quantidadeTotalRespostasPergunta;
            respostaSemPreenchimento.Resposta = "Sem preenchimento";
            respostaSemPreenchimento.AlunosQuantidade = quantidade >= 0 ? quantidade : 0;
            respostaSemPreenchimento.AlunosPercentual = (respostaSemPreenchimento.AlunosQuantidade > 0 ? (respostaSemPreenchimento.AlunosQuantidade * 100) / (Double)totalDeAlunos : 0);
            return respostaSemPreenchimento;
        }
    }

}
