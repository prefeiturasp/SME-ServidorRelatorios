using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Application.UseCases
{
    public class RelatorioFrequenciaControleMensalUseCase : IRelatorioFrequenciaControleMensalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFrequenciaControleMensalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var retorno = new List<ControleFrequenciaMensalDto>();

            retorno = await MapearDtoRetorno(request);
        }

        private async Task<List<ControleFrequenciaMensalDto>> MapearDtoRetorno(FiltroRelatorioDto request)
        {
            var retorno = new List<ControleFrequenciaMensalDto>();
            var filtro = request.ObterObjetoFiltro<FiltroRelatorioControleFrenquenciaMensalDto>();
            var ueComDre = await ObterUeComDrePorCodigo(filtro.CodigoUe);
            var dadosTurma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtro.CodigoTurma));
            

            var valorSemestre = filtro.Semestre != null ? int.Parse(filtro.Semestre) : 0;

            var frequencias = await mediator.Send(new ObterFrequenciaRealatorioControleMensalQuery(filtro.AnoLetivo, filtro.MesesReferencias.ToArray(), filtro.CodigoUe
                , filtro.CodigoDre, (int) filtro.Modalidade, valorSemestre, filtro.CodigoTurma,
                filtro.AlunosCodigo));

            var agrupadoPorAlunos = frequencias.GroupBy(x => x.CodigoAluno).Distinct().ToList();

            foreach (var alunoFrequencia in agrupadoPorAlunos)
            {
                var dadosAluno = await mediator.Send(new ObterNomeAlunoPorCodigoQuery(alunoFrequencia.Key));
                var controFrequenciaMensal = new ControleFrequenciaMensalDto
                {
                    Ano = filtro.AnoLetivo,
                    Usuario = $"{filtro.NomeUsuario}(${filtro.CodigoRf})",
                    Dre = ueComDre.Dre.Abreviacao,
                    Ue = ueComDre.NomeRelatorio,
                    Turma = dadosTurma.NomeRelatorio,
                    CodigoCriancaEstudante = dadosAluno.Codigo,
                    NomeCriancaEstudante = dadosAluno.Nome,
                    DataImpressao = DateTime.Now.ToString("dd/MM/yyyy"),
                };
                var agrupadoPorMes = alunoFrequencia.GroupBy(x => x.Mes);

                foreach (var mesAgrupado in agrupadoPorMes)
                {
                    double totalFrequenciaDoPeriodo = 0;
                    var mes = new ControleFrequenciaPorMesDto
                    {
                        Mes = mesAgrupado.Key,
                        MesDescricao = ObterNomeMes(mesAgrupado.Key),
                        FrequenciaGlobal = "%"
                    };
                    var componentesAgrupado = mesAgrupado.OrderBy(x =>x.OrdemExibicaoComponente).GroupBy(x => x.NomeComponente);
                    foreach (var componenteAgrupado in componentesAgrupado)
                    {
                        var frequenciaPeriodoComponente = PercentualFrequenciaComponente(componenteAgrupado);
                        totalFrequenciaDoPeriodo += frequenciaPeriodoComponente;
                        var componente = new ControleFrequenciaPorComponenteDto
                        {
                            NomeComponente = componenteAgrupado.Key,
                            FrequenciaDoPeriodo = $"{frequenciaPeriodoComponente}%"
                        };
                        MapearTipoPresencas(componenteAgrupado,componente);
                        MapearTipoCompensacoes(componenteAgrupado, componente);
                        MapearTipoAulas(componenteAgrupado, componente);
                        mes.FrequenciaComponente.Add(componente);
                    }

                    mes.FrequenciaGlobal = $"{(totalFrequenciaDoPeriodo / componentesAgrupado.Count())}%";
                    controFrequenciaMensal.FrequenciaMes.Add(mes);
                }

                retorno.Add(controFrequenciaMensal);
            }

            return retorno;
        }

        private static double PercentualFrequenciaComponente(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado)
        {
            var totalAulas = componenteAgrupado.Sum(x => x.TotalAula);
            var numeroFaltasNaoCompensadas = componenteAgrupado.Where(x => x.TotalTipoFrequencia == (int) TipoFrequencia.F && x.DataCompensacao == null).Sum(s => s.TotalTipoFrequencia);

            if (totalAulas == 0)
                return 0;
            
            var percentual = 100 - (((double) numeroFaltasNaoCompensadas / totalAulas) * 100);
            var percentualArredondado = Math.Round(percentual, 2);
            return percentualArredondado;
        }


        private static void MapearTipoPresencas(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado,ControleFrequenciaPorComponenteDto componente)
        {
            var tipoFrequenciaPrensenca = new List<int> {1, 3};
            var presencas = componenteAgrupado.Where(p => tipoFrequenciaPrensenca.Contains(p.TipoFrequencia)).ToList();
            var controleFrequenciaPorTipoDto = new ControleFrequenciaPorTipoDto
            {
                TipoFrequencia = "Presenças",
                TotalDoPeriodo = presencas.Select(t => t.TotalTipoFrequencia).Sum(),
            };

            foreach (var presenca in presencas)
            {
                var controleFrequenciaPorAulaDto = new ControleFrequenciaPorAulaDto
                {
                    DiaSemanaSigla = presenca.DiaSemana,
                    DiaSemanaNumero = presenca.Dia,
                    Valor = presenca.TotalTipoFrequencia
                };
                controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
            }

            componente.FrequenciaPorTipo.Add(controleFrequenciaPorTipoDto);
        }
        private static void MapearTipoCompensacoes(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado, ControleFrequenciaPorComponenteDto componente)
        {
            var compensacoes = componenteAgrupado.Where(p => p.DataCompensacao != null).ToList();
            var controleFrequenciaPorTipoDto = new ControleFrequenciaPorTipoDto
            {
                TipoFrequencia = "Compensações",
                TotalDoPeriodo = compensacoes.Select(t => t.TotalTipoFrequencia).Sum(),
            };

            foreach (var compensacao in compensacoes)
            {
                var controleFrequenciaPorAulaDto = new ControleFrequenciaPorAulaDto
                {
                    DiaSemanaSigla = compensacao.DiaSemana,
                    DiaSemanaNumero = compensacao.Dia,
                    Valor = compensacao.TotalTipoFrequencia
                };
                controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
            }

            componente.FrequenciaPorTipo.Add(controleFrequenciaPorTipoDto);
        }
        
        private static void MapearTipoAulas(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado, ControleFrequenciaPorComponenteDto componente)
        {
            var aulas = componenteAgrupado.ToList();
            var controleFrequenciaPorTipoDto = new ControleFrequenciaPorTipoDto
            {
                TipoFrequencia = "Aulas",
                TotalDoPeriodo = aulas.Select(t => t.TotalAula).Sum(),
            };

            foreach (var aula in aulas)
            {
                var controleFrequenciaPorAulaDto = new ControleFrequenciaPorAulaDto
                {
                    DiaSemanaSigla = aula.DiaSemana,
                    DiaSemanaNumero = aula.Dia,
                    Valor = aula.TotalTipoFrequencia
                };
                controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
            }

            componente.FrequenciaPorTipo.Add(controleFrequenciaPorTipoDto);
        }


        private async Task<Ue> ObterUeComDrePorCodigo(string codigoUe)
        {
            return await mediator.Send(new ObterUeComDrePorCodigoUeQuery(codigoUe));
        }

        private string ObterNomeMes(int mes)
        {
            switch (mes)
            {
                case 1:
                    return "Janeiro";
                case 2:
                    return "Fevereiro";
                case 3:
                    return "Março";
                case 4:
                    return "Abril";
                case 5:
                    return "Maio";
                case 6:
                    return "Junho";
                case 7:
                    return "Julho";
                case 8:
                    return "Agosto";
                case 9:
                    return "Setembro";
                case 10:
                    return "Outubro";
                case 11:
                    return "Novembro";
                case 12:
                    return "Dezembro";
                default:
                    return string.Empty;
            }
        }
    }
}