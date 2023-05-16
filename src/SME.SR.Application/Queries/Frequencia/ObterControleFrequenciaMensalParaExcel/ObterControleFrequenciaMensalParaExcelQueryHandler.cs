using MediatR;
using Sentry;
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
                    relatorios.Add(ObterFrequenciaDto(frenquenciaAluno));
                }

                return await Task.FromResult(relatorios);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
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

            foreach(var frequenciaMes in dto.FrequenciaMes)
            {
                var data = new DataTable();
                var diasSemanas = ObterDiaSemanaAulas(frequenciaMes.FrequenciaComponente);

                AdicionarColunas(diasSemanas, data, frequenciaMes.Mes);

                AdicionarTituloColunas(diasSemanas, data, frequenciaMes.Mes);

                AdicionarLinhas(frequenciaMes.FrequenciaComponente, data, frequenciaMes.Mes);

                frequenciasMeses.Add(new FrequenciaPorMesExcelDto() { Mes = frequenciaMes.MesDescricao, FrequenciaGlobal = frequenciaMes.FrequenciaGlobal, TabelaDeDado = data });
            }
            
            return frequenciasMeses;
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
                linhaNumeroDia[$"{mes}_{dias.Key}"] = dias.Key;
                linhaSiglaDia[$"{mes}_{dias.Key}"] = dias.Value;
            }

            linhaSiglaDia[$"{mes}_TotalDoPeriodo"] = "Total do Período";
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

        private Dictionary<int, string> ObterDiaSemanaAulas(IEnumerable<ControleFrequenciaPorComponenteDto> frequenciasPorComponente)
        {
            var diasSemanas = new Dictionary<int, string>();

            foreach (var frequenciaComponente in frequenciasPorComponente)
            {
                foreach (var FrequenciaTipo in frequenciaComponente.FrequenciaPorTipo)
                {
                    foreach (var FrequenciaAula in FrequenciaTipo.FrequenciaPorAula)
                    {
                        if (!diasSemanas.ContainsKey(FrequenciaAula.DiaSemanaNumero))
                            diasSemanas.Add(FrequenciaAula.DiaSemanaNumero, FrequenciaAula.DiaSemanaSigla);
                    }
                }
            }

            return diasSemanas; 
        }
    }
}
