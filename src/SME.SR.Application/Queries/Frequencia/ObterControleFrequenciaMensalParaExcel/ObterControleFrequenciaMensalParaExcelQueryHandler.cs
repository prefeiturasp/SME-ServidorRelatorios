using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Sentry;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace SME.SR.Application
{
    public class ObterControleFrequenciaMensalParaExcelQueryHandler : IRequestHandler<ObterControleFrequenciaMensalParaExcelQuery, IEnumerable<RelatorioControleFrequenciaMensalExcelDto>>
    {
        public async Task<IEnumerable<RelatorioControleFrequenciaMensalExcelDto>> Handle(ObterControleFrequenciaMensalParaExcelQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var relatorios = new List<RelatorioControleFrequenciaMensalExcelDto>();

                foreach (var frenquenciaAluno in request.ControlesFrequenciasMensais)
                {
                    relatorios.Add( ObterFrequenciaDto(frenquenciaAluno));
                }

                return await Task.FromResult(relatorios);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RelatorioControleFrequenciaMensalExcelDto ObterFrequenciaDto(ControleFrequenciaMensalDto dto)
        {
            return new RelatorioControleFrequenciaMensalExcelDto()
            {
                Ano = dto.Ano,
                CodigoCriancaEstudante = dto.CodigoCriancaEstudante,
                NomeCriancaEstudante = dto.NomeCriancaEstudante,
                Dre = dto.Dre,
                Ue = dto.Ue,
                DataImpressao = DateTime.Now.ToString("dd/MM/yyyy"),
                Turma = dto.Turma,
                Usuario = dto.Usuario,
                FrequenciasMeses = ObterFrequenciasPorMeses(dto)
            };
        }

        private List<FrequenciaPorMesExcelDto> ObterFrequenciasPorMeses(ControleFrequenciaMensalDto dto)
        {
            var frequenciasMeses = new List<FrequenciaPorMesExcelDto>();
            (int mes, int quantidadeDias) = ObterMaiorQuantidadeDias(dto.Ano, dto.FrequenciaMes.Select(fm => fm.Mes).ToArray());

            foreach (var frequenciaMes in dto.FrequenciaMes)
            {
                var data = new DataTable();
                var diasSemanas = ObterDiaSemanaAulas(frequenciaMes.FrequenciaComponente, dto.Ano, frequenciaMes.Mes, quantidadeDias);

                AdicionarColunas(diasSemanas, data, frequenciaMes.Mes);

                AdicionarTituloColunas(diasSemanas, data, frequenciaMes.Mes);

                AdicionarLinhas(frequenciaMes.FrequenciaComponente, data, frequenciaMes.Mes);

                frequenciasMeses.Add(new FrequenciaPorMesExcelDto() 
                    { Mes = frequenciaMes.MesDescricao, 
                      FrequenciaGlobal = frequenciaMes.FrequenciaGlobal, 
                      TabelaDeDado = data,
                      ColunasDiasNaoLetivosFinaisSemana = ObterColunasDiasNaoLetivosFinaisSemana(diasSemanas.Where(ds => !string.IsNullOrEmpty(ds.Value))
                                                                                                                  .Select(ds => ds.Key).ToArray(),
                                                                                                       frequenciaMes.Mes,
                                                                                                       dto.Ano,
                                                                                                       frequenciaMes.DiasNaoLetivosMes)
                    });
            }
            
            return frequenciasMeses;
        }

        private List<string> ObterColunasDiasNaoLetivosFinaisSemana(int[] dias, int mes, int ano, List<DiaLetivoDto> diasNaoLetivosMes)
        {
            var retorno = new List<string>();
            foreach (var dia in dias)
            {
                DateTime dataBase = new DateTime(ano, mes, dia);
                if (diasNaoLetivosMes.Any(d => d.Data.Equals(dataBase)))
                    retorno.Add($"{mes}_{dia}");
            }
            return retorno;
        }

        private static string ObterDiaDaSemanaSigla(DateTime data)
        {
            // Obtém o dia da semana como um enum
            DayOfWeek diaDaSemana = data.DayOfWeek;

            // Converte o enum para a sigla correspondente
            switch (diaDaSemana)
            {
                case DayOfWeek.Sunday:
                    return "DOM";
                case DayOfWeek.Monday:
                    return "SEG";
                case DayOfWeek.Tuesday:
                    return "TER";
                case DayOfWeek.Wednesday:
                    return "QUA";
                case DayOfWeek.Thursday:
                    return "QUI";
                case DayOfWeek.Friday:
                    return "SEX";
                case DayOfWeek.Saturday:
                    return "SAB";
                default:
                    return "";
            }
        }

        private static (int, int) ObterMaiorQuantidadeDias(int ano, int[] meses)
        {
            int maiorQuantidadeDias = 0;
            int mesComMaiorQuantidadeDias = 0;

            foreach (int mes in meses)
            {
                int quantidadeDias = DateTime.DaysInMonth(ano, mes);
                if (quantidadeDias > maiorQuantidadeDias)
                {
                    maiorQuantidadeDias = quantidadeDias;
                    mesComMaiorQuantidadeDias = mes;
                }
            }

            return (mesComMaiorQuantidadeDias, maiorQuantidadeDias);
        }

        private void AdicionarColunas(Dictionary<int, string> diasSemanas, DataTable data, int mes)
        {
            data.Columns.Add($"{mes}_Componentes");
            data.Columns.Add($"{mes}_TipoFrequencia");

            foreach (var dias in diasSemanas.OrderBy(dia => dia.Key))
            {
                data.Columns.Add($"{mes}_{dias.Key}");
            }

            data.Columns.Add($"{mes}_TotalDoPeriodo");
            data.Columns.Add($"{mes}_FrequenciaDoPeriodo");
        }

        private void AdicionarTituloColunas(Dictionary<int, string> diasSemanas, DataTable data, int mes)
        {
            DataRow linhaNumeroDia = data.NewRow();
            DataRow linhaSiglaDia = data.NewRow();

            foreach (var dias in diasSemanas.OrderBy(dia => dia.Key))
            {
                if (string.IsNullOrEmpty(dias.Value))
                    continue;
                linhaNumeroDia[$"{mes}_{dias.Key}"] = dias.Key;
                linhaSiglaDia[$"{mes}_{dias.Key}"] = dias.Value;
            }

            linhaSiglaDia[$"{mes}_TotalDoPeriodo"] = "Total";
            linhaSiglaDia[$"{mes}_FrequenciaDoPeriodo"] = "Frequência do Período";

            data.Rows.Add(linhaSiglaDia);
            data.Rows.Add(linhaNumeroDia);
        }

        private void AdicionarLinhas(IEnumerable<ControleFrequenciaPorComponenteDto> frequenciasPorComponente, DataTable data, int mes)
        {
            var linha = data.NewRow();

            foreach (var frequenciaComponente in frequenciasPorComponente)
            {
                linha[$"{mes}_Componentes"] = frequenciaComponente.NomeComponente;
                linha[$"{mes}_FrequenciaDoPeriodo"] = frequenciaComponente.FrequenciaDoPeriodo;

                foreach (var FrequenciaTipo in frequenciaComponente.FrequenciaPorTipo)
                {
                    linha[$"{mes}_TipoFrequencia"] = FrequenciaTipo.TipoFrequencia;
                    linha[$"{mes}_TotalDoPeriodo"] = FrequenciaTipo.TotalDoPeriodo;

                    foreach (var FrequenciaAula in FrequenciaTipo.FrequenciaPorAula)
                    {
                        linha[$"{mes}_{FrequenciaAula.DiaSemanaNumero}"] = FrequenciaAula.Valor;
                    }

                    data.Rows.Add(linha);
                    linha = data.NewRow();
                }
            }
        }

        private Dictionary<int, string> ObterDiaSemanaAulas(IEnumerable<ControleFrequenciaPorComponenteDto> frequenciasPorComponente, int ano, int mes, int qdadeDiasColunas)
        {
            var diasSemanas = new Dictionary<int, string>();
            for (int dia = 1; dia <= qdadeDiasColunas; dia++)
            {
                int quantidadeDiasMes = DateTime.DaysInMonth(ano, mes);
                if (dia <= quantidadeDiasMes)
                    diasSemanas.Add(dia, ObterDiaDaSemanaSigla(new DateTime(ano, mes, dia)));
                else
                    diasSemanas.Add(dia, "");
            }
            return diasSemanas; 
        }
    }
}
