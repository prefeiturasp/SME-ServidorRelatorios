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
    public class ObterSondagemMatAditMultiConsolidadoQueryHandler : IRequestHandler<ObterSondagemMatAditMultiConsolidadoQuery, RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto>
    {
        private readonly IMathPoolCARepository mathPoolCARepository;
        private readonly IMathPoolCMRepository mathPoolCMRepository;

        public ObterSondagemMatAditMultiConsolidadoQueryHandler(IMathPoolCARepository mathPoolCARepository, IMathPoolCMRepository mathPoolCMRepository)
        {
            this.mathPoolCARepository = mathPoolCARepository ?? throw new ArgumentNullException(nameof(mathPoolCARepository)); ;
            this.mathPoolCMRepository = mathPoolCMRepository ?? throw new ArgumentNullException(nameof(mathPoolCMRepository)); ;
        }

        public async Task<RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto> Handle(ObterSondagemMatAditMultiConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto();
            var perguntas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto>();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, request.Semestre, request.Usuario.CodigoRf, request.Usuario.Nome);

            if (request.Proficiencia == ProficienciaSondagemEnum.CampoAditivo)
            {
                var listaAlunos = await mathPoolCARepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                if (listaAlunos != null && listaAlunos.Any())
                {
                    var ordem1Ideia = listaAlunos.GroupBy(fu => fu.Ordem1Ideia);

                    var ordem1Resultado = listaAlunos.GroupBy(fu => fu.Ordem1Resultado);

                    var ordem2Ideia = listaAlunos.GroupBy(fu => fu.Ordem2Ideia);

                    var ordem2Resultado = listaAlunos.GroupBy(fu => fu.Ordem2Resultado);

                    if (request.TurmaAno != 2)
                    {
                        var ordem3Ideia = listaAlunos.GroupBy(fu => fu.Ordem3Ideia);

                        var ordem3Resultado = listaAlunos.GroupBy(fu => fu.Ordem3Resultado);

                        AdicionarOrdem(ordem3Ideia, ordem: "3", perguntas, request.QuantidadeTotalAlunos);
                        AdicionarOrdem(ordem3Resultado, ordem: "3", perguntas, request.QuantidadeTotalAlunos);

                        if (request.TurmaAno != 1 && request.TurmaAno != 3)
                        {
                            var ordem4Ideia = listaAlunos.GroupBy(fu => fu.Ordem4Ideia);

                            var ordem4Resultado = listaAlunos.GroupBy(fu => fu.Ordem4Resultado);

                            AdicionarOrdem(ordem4Ideia, ordem: "4", perguntas, request.QuantidadeTotalAlunos);
                            AdicionarOrdem(ordem4Resultado, ordem: "4", perguntas, request.QuantidadeTotalAlunos);

                        }
                    }

                    AdicionarOrdem(ordem1Ideia, ordem: "1", perguntas, request.QuantidadeTotalAlunos);
                    AdicionarOrdem(ordem1Resultado, ordem: "1", perguntas, request.QuantidadeTotalAlunos);
                    AdicionarOrdem(ordem2Ideia, ordem: "2", perguntas, request.QuantidadeTotalAlunos);
                    AdicionarOrdem(ordem2Resultado, ordem: "2", perguntas, request.QuantidadeTotalAlunos);
                }
            }
            else
            {
                var listaAlunos = await mathPoolCMRepository.ObterPorFiltros(request.Dre?.Codigo, request.Ue?.Codigo, request.TurmaAno, request.AnoLetivo, request.Semestre);

                if (listaAlunos != null && listaAlunos.Any())
                {
                    if (request.TurmaAno == 2)
                    {
                        var ordem3Ideia = listaAlunos.GroupBy(fu => fu.Ordem3Ideia);

                        var ordem3Resultado = listaAlunos.GroupBy(fu => fu.Ordem3Resultado);

                        AdicionarOrdem(ordem3Ideia, ordem: "3", perguntas, request.QuantidadeTotalAlunos);
                        AdicionarOrdem(ordem3Resultado, ordem: "3", perguntas, request.QuantidadeTotalAlunos);
                    }
                    else
                    {
                        if (request.TurmaAno == 3)
                        {
                            var ordem3Ideia = listaAlunos.GroupBy(fu => fu.Ordem3Ideia);

                            var ordem3Resultado = listaAlunos.GroupBy(fu => fu.Ordem3Resultado);

                            AdicionarOrdem(ordem3Ideia, ordem: "3", perguntas, request.QuantidadeTotalAlunos);
                            AdicionarOrdem(ordem3Resultado, ordem: "3", perguntas, request.QuantidadeTotalAlunos);
                        }


                        var ordem5Ideia = listaAlunos.GroupBy(fu => fu.Ordem5Ideia);

                        var ordem5Resultado = listaAlunos.GroupBy(fu => fu.Ordem5Resultado);

                        AdicionarOrdem(ordem5Ideia, ordem: "5", perguntas, request.QuantidadeTotalAlunos);
                        AdicionarOrdem(ordem5Resultado, ordem: "5", perguntas, request.QuantidadeTotalAlunos);

                        if (request.TurmaAno != 3)
                        {
                            var ordem6Ideia = listaAlunos.GroupBy(fu => fu.Ordem6Ideia);

                            var ordem6Resultado = listaAlunos.GroupBy(fu => fu.Ordem6Resultado);

                            AdicionarOrdem(ordem6Ideia, ordem: "6", perguntas, request.QuantidadeTotalAlunos);
                            AdicionarOrdem(ordem6Resultado, ordem: "6", perguntas, request.QuantidadeTotalAlunos);

                            var ordem7Ideia = listaAlunos.GroupBy(fu => fu.Ordem7Ideia);

                            var ordem7Resultado = listaAlunos.GroupBy(fu => fu.Ordem6Resultado);

                            AdicionarOrdem(ordem7Ideia, ordem: "7", perguntas, request.QuantidadeTotalAlunos);
                            AdicionarOrdem(ordem7Resultado, ordem: "7", perguntas, request.QuantidadeTotalAlunos);
                        }

                        if (request.TurmaAno != 3 && request.TurmaAno != 4)
                        {
                            var ordem8Ideia = listaAlunos.GroupBy(fu => fu.Ordem8Ideia);

                            var ordem8Resultado = listaAlunos.GroupBy(fu => fu.Ordem8Resultado);

                            AdicionarOrdem(ordem8Ideia, ordem: "8", perguntas, request.QuantidadeTotalAlunos);
                            AdicionarOrdem(ordem8Resultado, ordem: "8", perguntas, request.QuantidadeTotalAlunos);
                        }

                    }
                }
            }

            if (perguntas.Any())
                relatorio.PerguntasRespostas = perguntas;

            TrataAlunosQueNaoResponderam(relatorio, request.QuantidadeTotalAlunos);

            return relatorio;
        }

        private void TrataAlunosQueNaoResponderam(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, int quantidadeTotalAlunos)
        {
            foreach (var perguntaResposta in relatorio.PerguntasRespostas)
            {
                var qntDeAlunosPreencheu = perguntaResposta.Respostas.Sum(a => a.AlunosQuantidade);
                var diferencaPreencheuNao = quantidadeTotalAlunos - qntDeAlunosPreencheu;

                var percentualNaoPreencheu = (diferencaPreencheuNao / quantidadeTotalAlunos) * 100;

                perguntaResposta.Respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    Resposta = "Sem preenchimento",
                    AlunosQuantidade = diferencaPreencheuNao,
                    AlunosPercentual = percentualNaoPreencheu
                });
            }
        }

        private void MontarCabecalho(RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int semestre, string rf, string usuario)
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

        private void AdicionarOrdem(IEnumerable<IGrouping<string, MathPoolCA>> agrupamento, string ordem, List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> perguntas, int TotalAlunosGeral)
        {
            var respostas = ObterRespostas(agrupamento, TotalAlunosGeral);

            if (respostas != null && respostas.Any())
            {
                perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
                {
                    Ordem = ordem,
                    Respostas = respostas
                });
            }
        }

        private void AdicionarOrdem(IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, string ordem, List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> perguntas, int TotalAlunosGeral)
        {
            var respostas = ObterRespostas(agrupamento, TotalAlunosGeral);

            if (respostas != null && respostas.Any())
            {
                perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
                {
                    Ordem = ordem,
                    Respostas = respostas
                });
            }
        }

        private List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCA>> agrupamento, int TotalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento.Where(a => !a.Key.Trim().Equals(""));

            foreach (var item in agrupamentosComValor)
            {
                respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    Resposta = ConverteTextoPollMatematica(item.Key),
                    AlunosQuantidade = item.Count(),
                    AlunosPercentual = ((double)item.Count() / TotalAlunosGeral) * 100
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> ObterRespostas(IEnumerable<IGrouping<string, MathPoolCM>> agrupamento, int TotalAlunosGeral)
        {
            var respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();

            var agrupamentosComValor = agrupamento.Where(a => !a.Key.Trim().Equals(""));

            foreach (var item in agrupamentosComValor)
            {
                respostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto()
                {
                    Resposta = ConverteTextoPollMatematica(item.Key),
                    AlunosQuantidade = item.Count(),
                    AlunosPercentual = ((double)item.Count() / TotalAlunosGeral) * 100
                });
            }

            if (respostas.Any())
                return respostas;
            else
                return null;
        }

        private string ConverteTextoPollMatematica(string texto)
        {
            switch (texto)
            {
                case "A":
                    return "Acertou";
                case "E":
                    return "Errou";
                case "NR":
                    return "Não Resolveu";
                default:
                    return "";
            }
        }
    }
}
