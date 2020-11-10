using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
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

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            if (request.TurmaAno > 3)
            {
                var listaPerguntas = await perguntasAutoralRepository.ObterPerguntasPorComponenteAnoTurma(request.TurmaAno, ComponenteCurricularSondagemEnum.Matematica);
                var listaAlunos = await sondagemAutoralRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, string.Empty, string.Empty, request.TurmaAno, request.AnoLetivo, ComponenteCurricularSondagemEnum.Matematica);

                if (listaPerguntas != null && listaPerguntas.Any())
                {
                    var perguntasAgrupado = listaPerguntas.GroupBy(g => g.PerguntaId);
                    var respostasAgrupado = listaAlunos.GroupBy(g => g.PerguntaId);

                    foreach (var pergunta in perguntasAgrupado)
                    {
                        var respostas = respostasAgrupado.FirstOrDefault(a => a.Key == pergunta.Key);

                        AdicionarPergunta(pergunta, pergunta.Key, respostas, perguntas, request.QuantidadeTotalAlunos);
                    }
                }
            }
            else
            {
                var listaAlunos = await mathPoolNumbersRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                if (listaAlunos != null && listaAlunos.Any())
                {
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
            }

            if (perguntas.Any())
                relatorio.PerguntasRespostas = perguntas;


            TrataAlunosQueNaoResponderam(relatorio, request.QuantidadeTotalAlunos);

            return relatorio;
        }

        private void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, int quantidadeTotalAlunos)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var qntDeAlunosPreencheu = perguntaResposta.Respostas.Sum(a => a.AlunosQuantidade);
                var diferencaPreencheuNao = quantidadeTotalAlunos - qntDeAlunosPreencheu;
                
                var percentualNaoPreencheu = (diferencaPreencheuNao / quantidadeTotalAlunos) * 100;

                perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                {
                    Resposta = "Sem preenchimento", 
                    AlunosQuantidade = diferencaPreencheuNao,
                    AlunosPercentual = percentualNaoPreencheu

                });
            }
        }

        private void MontarCabecalho(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, string rf, string usuario)
        {
            relatorio.Ano = anoTurma;
            relatorio.AnoLetivo = anoLetivo;
            relatorio.ComponenteCurricular = "Matemática";
            relatorio.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.Dre = dre != null ? dre.Abreviacao : "Todas";
            relatorio.Periodo = $"{semestre}º Semestre";
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

            var totalAlunos =  agrupamentosComValor?.Count() ?? 0;

            var agrupamentosComValorAgrupado = agrupamentosComValor?.GroupBy(g => g.RespostaId );

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
    }

}
