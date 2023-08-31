using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var controles = await MapearDtoRetorno(request);

            if (controles == null || !controles.Any())
                throw new NegocioException("Não há dados para o Relatório de controle de frequência mensal");

            await mediator.Send(new GerarRelatoricoControleDeFrequenciaMensalExcelCommand(controles, request.CodigoCorrelacao));
        }

        private async Task<List<ControleFrequenciaMensalDto>> MapearDtoRetorno(FiltroRelatorioDto request)
        {
            var retorno = new List<ControleFrequenciaMensalDto>();
            var filtro = request.ObterObjetoFiltro<FiltroRelatorioControleFrenquenciaMensalDto>();

            if (filtro.TipoFormatoRelatorio != TipoFormatoRelatorio.Xlsx)
                throw new NegocioException("Relatório disponível somente no formata Excel");

            var ueComDre = await ObterUeComDrePorCodigo(filtro.CodigoUe);
            var dadosTurma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtro.CodigoTurma));

            var valorSemestre = filtro.Semestre != null ? int.Parse(filtro.Semestre) : 0;

            var frequencias = await mediator.Send(new ObterFrequenciaRealatorioControleMensalQuery(filtro.AnoLetivo, filtro.MesesReferencias.ToArray(), filtro.CodigoUe
                , filtro.CodigoDre, (int)filtro.Modalidade, valorSemestre, filtro.CodigoTurma,
                filtro.AlunosCodigo));

            var agrupadoPorAlunos = frequencias.GroupBy(x => x.CodigoAluno).Distinct().ToList();

            foreach (var alunoFrequencia in agrupadoPorAlunos)
            {
                var dadosAluno = await mediator.Send(new ObterNomeAlunoPorCodigoQuery(alunoFrequencia.Key));
                var controFrequenciaMensal = new ControleFrequenciaMensalDto
                {
                    Ano = filtro.AnoLetivo,
                    Usuario = $"{filtro.NomeUsuario}({filtro.CodigoRf})",
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
                        MesDescricao = ObterNomeMes(mesAgrupado.Key)
                    };

                    var componentesAgrupado = mesAgrupado.Where(w => !string.IsNullOrEmpty(w.NomeComponente)).OrderBy(x => x.NomeGrupo).ThenBy(t => t.NomeComponente).GroupBy(x => x.NomeComponente);
                    foreach (var componenteAgrupado in componentesAgrupado)
                    {
                        var frequenciaPeriodoComponente = PercentualFrequenciaComponente(componenteAgrupado);
                        totalFrequenciaDoPeriodo += frequenciaPeriodoComponente;
                        var componente = new ControleFrequenciaPorComponenteDto
                        {
                            NomeComponente = componenteAgrupado.Key,
                            FrequenciaDoPeriodo = $"{frequenciaPeriodoComponente}%"
                        };
                        var aulas = componenteAgrupado.Where(x => x.CodigoAluno == controFrequenciaMensal.CodigoCriancaEstudante).ToList();

                        MapearTipoAulas(componenteAgrupado, componente, controFrequenciaMensal.CodigoCriancaEstudante, aulas);
                        MapearTipoPresencas(componenteAgrupado, componente, controFrequenciaMensal.CodigoCriancaEstudante, aulas);
                        MapearTipoCompensacoes(componenteAgrupado, componente, controFrequenciaMensal.CodigoCriancaEstudante, aulas);
                        mes.FrequenciaComponente.Add(componente);
                    }

                    mes.FrequenciaGlobal = CalcularObterFrequenciaGlobal(alunoFrequencia);
                    controFrequenciaMensal.FrequenciaMes.Add(mes);
                }

                retorno.Add(controFrequenciaMensal);
            }

            return retorno.OrderBy(controle => controle.NomeCriancaEstudante).ToList();
        }

        private string CalcularObterFrequenciaGlobal(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> alunoFrequencia)
        {
            var frequenciaAlunoCalculo = new FrequenciaAluno()
            {
                TotalAulas = alunoFrequencia.Sum(af => af.TotalAula),
                TotalAusencias = alunoFrequencia.Where(af => af.TipoFrequencia == (int)TipoFrequencia.F).Sum(af => af.TotalAula),
                TotalCompensacoes = alunoFrequencia.Sum(af => af.TotalCompensacao ?? 0)
            };

            return FrequenciaAluno.FormatarPercentual(FrequenciaAluno.ArredondarPercentual(frequenciaAlunoCalculo.PercentualFrequencia));
        }

        private double PercentualFrequenciaComponente(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado)
        {
            var totalAulas = componenteAgrupado.Sum(aula => aula.TotalAula);                        
            var numeroCompensacao = componenteAgrupado.Where(x => x.TotalCompensacao.HasValue).Sum(x => x.TotalCompensacao.Value);            
            var totalAusencias = componenteAgrupado.Where(af => af.TipoFrequencia == (int)TipoFrequencia.F).Sum(af => af.TotalAula);

            if (totalAulas == 0)
                return 0;

            var frequenciaAluno = new FrequenciaAluno()
            {
                TotalAulas = totalAulas,
                TotalAusencias = totalAusencias,
                TotalCompensacoes = numeroCompensacao
            };
           
            return FrequenciaAluno.ArredondarPercentual(frequenciaAluno.PercentualFrequencia);
        }


        private void MapearTipoPresencas(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado, ControleFrequenciaPorComponenteDto componente, string codAluno, List<ConsultaRelatorioFrequenciaControleMensalDto> aulas)
        {
            var tipoFrequenciaPrensenca = new List<int> { (int)TipoFrequencia.R, (int)TipoFrequencia.C };
            var presencas = componenteAgrupado.Where(p => tipoFrequenciaPrensenca.Contains(p.TipoFrequencia) && p.CodigoAluno == codAluno).ToList();
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
            var aulasSemPresencas = aulas.Except(presencas);
            if (aulasSemPresencas.Any())
            {
                foreach (var aula in aulasSemPresencas)
                {
                    var controleFrequenciaPorAulaDto = new ControleFrequenciaPorAulaDto
                    {
                        DiaSemanaSigla = aula.DiaSemana,
                        DiaSemanaNumero = aula.Dia,
                        Valor = 0
                    };
                    controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
                }
            }

            componente.FrequenciaPorTipo.Add(controleFrequenciaPorTipoDto);
        }
        private void MapearTipoCompensacoes(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado, ControleFrequenciaPorComponenteDto componente, string codAluno, List<ConsultaRelatorioFrequenciaControleMensalDto> aulas)
        {
            var compensacoes = componenteAgrupado.Where(p => p.DataCompensacao != null && p.TipoFrequencia == (int)TipoFrequencia.F && p.CodigoAluno == codAluno).ToList();
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
                    Valor = compensacao.TotalCompensacao ?? 0,
                };
                controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
            }

            var aulasSemCompensacoes = aulas.Where(a => !compensacoes.Any(c => c.DataAula == a.DataAula)).ToList();
            foreach (var aula in aulasSemCompensacoes)
            {
                var controleFrequenciaPorAulaDto = new ControleFrequenciaPorAulaDto
                {
                    DiaSemanaSigla = aula.DiaSemana,
                    DiaSemanaNumero = aula.Dia,
                    Valor = 0
                };
                controleFrequenciaPorTipoDto.FrequenciaPorAula.Add(controleFrequenciaPorAulaDto);
            }

            componente.FrequenciaPorTipo.Add(controleFrequenciaPorTipoDto);
        }

        private void MapearTipoAulas(IGrouping<string, ConsultaRelatorioFrequenciaControleMensalDto> componenteAgrupado, ControleFrequenciaPorComponenteDto componente, string codAlunos, List<ConsultaRelatorioFrequenciaControleMensalDto> aulas)
        {
            var controleFrequenciaPorTipoDto = new ControleFrequenciaPorTipoDto
            {
                TipoFrequencia = "Aulas",
                TotalDoPeriodo = aulas.Sum(aula => aula.TotalAula),
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