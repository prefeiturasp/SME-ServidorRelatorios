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
                    FrequenciaGlobal = "0000%"
                };
                var agrupadoPorMes = alunoFrequencia.GroupBy(x => x.Mes);

                foreach (var mesAgrupado in agrupadoPorMes)
                {
                    var mes = new ControleFrequenciaPorMesDto
                    {
                        Mes = mesAgrupado.Key,
                        MesDescricao = ObterNomeMes(mesAgrupado.Key),
                    };
                    var componentesAgrupado = mesAgrupado.OrderBy(x =>x.OrdemExibicaoComponente).GroupBy(x => x.NomeComponente);
                    foreach (var componenteAgrupado in componentesAgrupado)
                    {
                        var componente = new ControleFrequenciaPorComponenteDto
                        {
                            NomeComponente = componenteAgrupado.Key,
                            FrequenciaDoPeriodo =  "0000%"
                        };
                        var tipoFrequenciaPrensenca = new List<int> {1, 3};


                        mes.FrequenciaComponente.Add(componente);
                    }
                    controFrequenciaMensal.FrequenciaMes.Add(mes);
                }


                retorno.Add(controFrequenciaMensal);
            }

            return retorno;
        }


        public async Task<Ue> ObterUeComDrePorCodigo(string codigoUe)
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