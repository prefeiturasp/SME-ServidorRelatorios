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
        private readonly ISondagemAutoralRepository sondagemAutoralRepository;
        private readonly IMediator mediator;
        private const int ANO_LETIVO_DOIS_MIL_VINTE_DOIS = 2022;
        private const int TERCEIRO_ANO = 3;

        public ObterSondagemMatNumAutoralConsolidadoQueryHandler(IMathPoolNumbersRepository mathPoolNumbersRepository, ISondagemAutoralRepository sondagemAutoralRepository, IMediator mediator)
        {
            this.mathPoolNumbersRepository = mathPoolNumbersRepository ?? throw new ArgumentNullException(nameof(mathPoolNumbersRepository));
            this.sondagemAutoralRepository = sondagemAutoralRepository ?? throw new ArgumentNullException(nameof(sondagemAutoralRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto> Handle(ObterSondagemMatNumAutoralConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var consideraNovaOpcaoRespostaSemPreenchimento = await mediator.Send(new UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(request.Semestre,request.Bimestre,request.AnoLetivo), cancellationToken);
            var relatorio = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto();
            var perguntas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto>();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Bimestre, request.Usuario.CodigoRf, request.Usuario.Nome, request.QuantidadeTotalAlunos);

            if (request.TurmaAno > TERCEIRO_ANO || request.AnoLetivo >= ANO_LETIVO_DOIS_MIL_VINTE_DOIS)
            {
                var totalDeAlunos = request.QuantidadeTotalAlunos;
                var listaPeguntaResposta = await sondagemAutoralRepository.ObterSondagemPerguntaRespostaConsolidadoBimestre(request.Dre?.Codigo, request.Ue?.Codigo, (request.Bimestre > 0 ? request.Bimestre : request.Semestre), request.TurmaAno, request.AnoLetivo, ComponenteCurricularSondagemEnum.Matematica.Name());
                var relatorioAgrupado = listaPeguntaResposta.GroupBy(p => p.PerguntaId).ToList();

                relatorioAgrupado.ForEach(x =>
                {
                    var pergunta = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto();
                    var totalRespostas = x.Where(y => y.PerguntaId == x.Key).Sum(q => q.QtdRespostas);
                    totalDeAlunos = totalRespostas > totalDeAlunos ? totalRespostas : totalDeAlunos;

                    CalcularPercentualTotalPergunta(x.First(y => y.PerguntaId == x.Key).PerguntaDescricao, pergunta);

                    var listaPr = x.Where(y => y.PerguntaId == x.Key).ToList();
                    CalculaPercentualRespostas(totalDeAlunos, pergunta, listaPr, totalRespostas,consideraNovaOpcaoRespostaSemPreenchimento);

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

                AdicionarPergunta(familiaresAgrupados, grupo: "Familiares/Frequentes", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(opacosAgrupados, grupo: "Opacos", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(transparentesAgrupados, grupo: "Transparentes", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(terminamZeroAgrupados, grupo: "Terminam em zero", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(algarismosAgrupados, grupo: "Algarismos iguais", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(processoAgrupados, grupo: "Processo de generalização", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
                AdicionarPergunta(zeroIntercaladosAgrupados, grupo: "Zero intercalado", perguntas, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
            }


            if (perguntas.Any() && request.AnoLetivo < 2022)
            {
                perguntas.ForEach(pergunta => pergunta.Respostas = pergunta.Respostas.OrderBy(r => r.Resposta).ToList());
            }

            relatorio.PerguntasRespostas = perguntas;

            TrataAlunosQueNaoResponderam(relatorio, request.QuantidadeTotalAlunos,consideraNovaOpcaoRespostaSemPreenchimento);
            GerarGraficos(relatorio);

            return relatorio;
        }

        private static void GerarGraficos(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio)
        {
            relatorio.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            foreach (var pergunta in relatorio.PerguntasRespostas)
            {
                var chave = string.Empty;
                var chaveIndex = 0;
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
                var valorMaximoEixo = grafico.EixosX.Any() ? grafico.EixosX.Max(a => int.Parse(a.Valor.ToString())) : 0;

                grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(340, "Quantidade Alunos", valorMaximoEixo.ArredondaParaProximaDezena(), 10);
                grafico.Legendas = legendas;
                relatorio.GraficosBarras.Add(grafico);
            }
        }

        private static void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, int quantidadeTotalAlunos,
            bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var qntDeAlunosPreencheu = perguntaResposta.Respostas.Sum(a => a.AlunosQuantidade);
                var diferencaPreencheuNao = quantidadeTotalAlunos - qntDeAlunosPreencheu;

                var percentualNaoPreencheu = diferencaPreencheuNao == 0 && quantidadeTotalAlunos == 0 ? 0 : ((double)diferencaPreencheuNao / quantidadeTotalAlunos) * 100;

                var existePerguntasSemPreenchimento = perguntaResposta.Respostas.FirstOrDefault(p => p.Resposta == "Sem preenchimento");

                if (consideraNovaOpcaoRespostaSemPreenchimento) continue;
                if (existePerguntasSemPreenchimento == null)
                    perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        AlunosQuantidade = diferencaPreencheuNao,
                        AlunosPercentual = percentualNaoPreencheu

                    });
            }
        }

        private static void MontarCabecalho(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, int? bimestre, string rf, string usuario, int totalAlunos)
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
            relatorio.TotalDeAlunos = totalAlunos;
        }

        private static void AdicionarPergunta(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento, string grupo, List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> perguntas, int totalAlunosGeral
        ,bool consideraNovaOpcaoRespostaSemPreenchimento)
        {
            var respostas = ObterRespostas(agrupamento, totalAlunosGeral);

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
                if (!consideraNovaOpcaoRespostaSemPreenchimento)
                {
                    perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
                    {
                        Pergunta = grupo,
                        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>()
                        {
                            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                            {
                                AlunosPercentual = 100,
                                AlunosQuantidade = totalAlunosGeral,
                                Resposta = "Sem preenchimento"
                            }
                        }
                    });
                }
            }
        }

        private static List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento, int totalAlunosGeral)
        {
            var agrupamentosComValor = agrupamento.Where(a => !a.Key.Trim().Equals(""));

            var respostas = agrupamentosComValor.Select(item => 
                new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() 
                    { Resposta = item.Key.Equals("S", StringComparison.InvariantCultureIgnoreCase) ? 
                        "Escreve de forma convencional" : "Não escreve de forma convencional", AlunosQuantidade = item.Count(), AlunosPercentual = ((double)item.Count() / totalAlunosGeral) * 100 }).ToList();

            return respostas.Any() ? respostas : null;
        }

        private static void CalcularPercentualTotalPergunta(string descricaoPergunta, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto pergunta)
        {
            pergunta.Pergunta = descricaoPergunta;
            pergunta.Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();
        }

        private static void CalculaPercentualRespostas(int totalDeAlunos, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto pergunta, List<PerguntasRespostasDTO> listaPr, int totalRespostas,bool consideraNovaOpcaoRespostaSemPreenchimento)
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

            if (consideraNovaOpcaoRespostaSemPreenchimento) return;

            var respostaSempreenchimento = pergunta.Respostas.Find(resp => resp.Resposta == "Sem preenchimento");

            if (respostaSempreenchimento == null)
            {
                respostaSempreenchimento = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto();
                pergunta.Respostas.Add(respostaSempreenchimento);
            }

            CarregarRespostaSemPreenchimento(totalDeAlunos, totalRespostas, respostaSempreenchimento);
        }

        private static void CarregarRespostaSemPreenchimento(
                                                             int totalDeAlunos, 
                                                             int quantidadeTotalRespostasPergunta,
                                                             RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto respostaSemPreenchimento)
        {
            var quantidade = totalDeAlunos - quantidadeTotalRespostasPergunta;
            respostaSemPreenchimento.Resposta = "Sem preenchimento";
            respostaSemPreenchimento.AlunosQuantidade = quantidade >= 0 ? quantidade : 0;
            respostaSemPreenchimento.AlunosPercentual = (respostaSemPreenchimento.AlunosQuantidade > 0 ? (respostaSemPreenchimento.AlunosQuantidade * 100) / (double)totalDeAlunos : 0);
        }
    }

}
