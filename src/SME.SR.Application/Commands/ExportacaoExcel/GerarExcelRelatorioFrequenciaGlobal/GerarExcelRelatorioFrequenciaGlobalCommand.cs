using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application.Commands.ExportacaoExcel.GerarExcelRelatorioFrequenciaGlobal
{
    public class GerarExcelRelatorioFrequenciaGlobalCommand : IRequest<Unit>
    {
        public IList<FrequenciaGlobalDto> ObjetoExportacao { get; }
        public string NomeWorkSheet { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public bool PossuiNotaRodape { get; set; }
        public string NotaRodape { get; set; }
        public bool RelatorioFrequenciaGlobal { get; set; }
        public string MensagemTitulo { get; set; }

        public GerarExcelRelatorioFrequenciaGlobalCommand(IList<FrequenciaGlobalDto> objetoExportacaoExcel, string nomeWorkSheet, Guid codigoCorrelacao, bool possuiNotaRodape = false, string notaRodape = null, bool relatorioFrequenciaGlobal = false, string mensagemTitulo = "")
        {
            ObjetoExportacao = objetoExportacaoExcel;
            NomeWorkSheet = nomeWorkSheet;
            CodigoCorrelacao = codigoCorrelacao;
            PossuiNotaRodape = possuiNotaRodape;
            NotaRodape = notaRodape;
            RelatorioFrequenciaGlobal = relatorioFrequenciaGlobal;
            MensagemTitulo = mensagemTitulo;
        }
    }
}
