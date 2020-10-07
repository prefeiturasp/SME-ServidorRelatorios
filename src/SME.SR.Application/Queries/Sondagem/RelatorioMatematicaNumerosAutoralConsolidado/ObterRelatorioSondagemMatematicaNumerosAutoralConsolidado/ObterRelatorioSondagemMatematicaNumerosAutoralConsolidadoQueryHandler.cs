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
    public class ObterRelatorioSondagemMatematicaNumerosAutoralConsolidadoQueryHandler : IRequestHandler<ObterRelatorioSondagemMatematicaNumerosAutoralConsolidadoQuery, RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto>
    {
        private readonly IMathPoolNumbersRepository mathPoolNumbersRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;

        public ObterRelatorioSondagemMatematicaNumerosAutoralConsolidadoQueryHandler(IMathPoolNumbersRepository mathPoolNumbersRepository, IPerguntasAutoralRepository perguntasAutoralRepository)
        {
            this.mathPoolNumbersRepository = mathPoolNumbersRepository ?? throw new ArgumentNullException(nameof(mathPoolNumbersRepository));
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new ArgumentNullException(nameof(perguntasAutoralRepository));
        }

        public async Task<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto> Handle(ObterRelatorioSondagemMatematicaNumerosAutoralConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            if (request.TurmaAno < 7)
            {
                var listaAlunos = await perguntasAutoralRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo);

                var perguntas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto>();

                if (listaAlunos != null && listaAlunos.Any())
                {
                    var perguntasAgrupado = listaAlunos.GroupBy(g => g.PerguntaId);

                    foreach (var pergunta in perguntasAgrupado)
                    {
                        AdicionarPergunta(pergunta, pergunta.Key, perguntas);
                    }
                }
            }
            else
            {
                var listaAlunos = await mathPoolNumbersRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                var perguntas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto>();

                if (listaAlunos != null && listaAlunos.Any())
                {
                    var familiaresAgrupados = listaAlunos.GroupBy(fu => fu.Familiares);

                    var opacosAgrupados = listaAlunos.GroupBy(fu => fu.Opacos);

                    var transparentesAgrupados = listaAlunos.GroupBy(fu => fu.Transparentes);

                    var terminamZeroAgrupados = listaAlunos.GroupBy(fu => fu.TerminamZero);

                    var algarismosAgrupados = listaAlunos.GroupBy(fu => fu.Algarismos);

                    var processoAgrupados = listaAlunos.GroupBy(fu => fu.Processo);

                    var zeroIntercaladosAgrupados = listaAlunos.GroupBy(fu => fu.ZeroIntercalados);

                    AdicionarPergunta(familiaresAgrupados, grupo: "Familiares/Frequentes", perguntas);
                    AdicionarPergunta(opacosAgrupados, grupo: "Opacos", perguntas);
                    AdicionarPergunta(transparentesAgrupados, grupo: "Transparentes", perguntas);
                    AdicionarPergunta(terminamZeroAgrupados, grupo: "Terminam em zero", perguntas);
                    AdicionarPergunta(algarismosAgrupados, grupo: "Algarismos iguais", perguntas);
                    AdicionarPergunta(processoAgrupados, grupo: "Processo de generalização", perguntas);
                    AdicionarPergunta(zeroIntercaladosAgrupados, grupo: "Zero intercalado", perguntas);
                }
            }

            return relatorio;
        }

        private void MontarCabecalho(RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, string rf, string usuario)
        {
            relatorio.Ano = anoTurma;
            relatorio.AnoLetivo = anoLetivo;
            relatorio.ComponenteCurricular = "Matemática";
            relatorio.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.Dre = dre == null ? dre.Abreviacao : "Todas";
            relatorio.Periodo = $"{semestre}º Semestre";
            relatorio.Proficiencia = "Números";
            relatorio.RF = rf;
            relatorio.Ue = ue == null ? dre.Abreviacao : "Todas";
            relatorio.Usuario = usuario;
        }

        private void AdicionarPergunta(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento, string grupo, List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> perguntas)
        {
            perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
            {
                Pergunta = grupo,
                Respostas = ObterRespostas(agrupamento)
            });
        }

        private void AdicionarPergunta(IGrouping<string, PerguntasAutoralDto> agrupamento, string grupo, List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> perguntas)
        {
            perguntas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
            {
                Pergunta = grupo,
                Respostas = ObterRespostas(agrupamento)
            });
        }

        private List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolNumber>> agrupamento)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();

            var totalAlunos = agrupamento.SelectMany(g => g).Count();

            foreach (var item in agrupamento)
            {
                if (!item.Key.Trim().Equals(""))
                {
                    respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                    {
                        Resposta = item.Key.Equals("S", StringComparison.InvariantCultureIgnoreCase) ? "Escreve convencionalmente" : "Não escreve convencionalmente",
                        AlunosQuantidade = item.Count(),
                        AlunosPercentual = ((double)item.Count() / totalAlunos) * 100
                    });
                }
            }

            return respostas;
        }

        private List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> ObterRespostas(IGrouping<string, PerguntasAutoralDto> agrupamento)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();

            var totalAlunos = agrupamento.Count();

            foreach (var item in agrupamento.GroupBy(g => new { g.RespostaId, g.Resposta }))
            {
                if (!item.Key.Resposta.Trim().Equals(""))
                {
                    respostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto()
                    {
                        Resposta = item.Key.Resposta,
                        AlunosQuantidade = item.Count(),
                        AlunosPercentual = ((double)item.Count() / totalAlunos) * 100
                    });
                }
            }

            return respostas;
        }
    }

}
