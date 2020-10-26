using MediatR;
using SME.SR.Infra;
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
        public RelatorioSondagemPortuguesConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();

            if (filtros.GrupoId != GrupoSondagemEnum.CapacidadeLeitura.Name())
                throw new NegocioException($"{ filtros.GrupoId } fora do esperado.");

            RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto relatorio = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros),
                Planilhas = await ObterPlanilhas(filtros)
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoLeitura", relatorio, Guid.NewGuid(), "", "", false));
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });
            var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre.Abreviacao,
                Periodo = $"{ filtros.Bimestre }° Bimestre",
                Rf = filtros.UsuarioRF,
                Ue = ue.NomeComTipoEscola,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.Ano,
                Turma = "Todas",
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.ShortName(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>> ObterPlanilhas(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros, GrupoSondagemEnum grupoSondagemEnum = GrupoSondagemEnum.CapacidadeLeitura)
        {
            IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto> linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery()
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

            var planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();

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

                var ordensSondagem = await mediator.Send(new ObterOrdensSondagemPorGrupoQuery() { Grupo = grupoSondagemEnum } );
                foreach (var ordem in ordensSondagem)
                {
                    planilhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
                    {
                        Ordem = ordem.Descricao,
                        Perguntas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
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

                    var respostas = linhasSondagem.Where(o => o.Ordem == ordem.Ordem && o.Pergunta == pergunta.Pergunta).ToList();
                    var totalRespostas = respostas.Sum(o => o.Quantidade);
                    foreach (var resposta in respostas)
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
