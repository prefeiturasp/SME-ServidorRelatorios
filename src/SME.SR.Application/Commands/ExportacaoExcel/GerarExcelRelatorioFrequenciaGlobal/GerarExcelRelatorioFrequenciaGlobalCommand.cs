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
        public bool PossuiNotaRodape { get; set; }
        public string NotaRodape { get; set; }
        public bool RelatorioFrequenciaGlobal { get; set; }
        public string MensagemTitulo { get; set; }
        public string UsuarioRf { get; internal set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; internal set; }

        public GerarExcelRelatorioFrequenciaGlobalCommand(
            IList<FrequenciaGlobalDto> objetoExportacaoExcel,
            string nomeWorkSheet,
            TipoFormatoRelatorio tipoFormatoRelatorio,
            bool possuiNotaRodape = false,
            string notaRodape = null,
            bool relatorioFrequenciaGlobal = false,
            string mensagemTitulo = "",
            string usuarioRf = null,
            TipoFormatoRelatorio TipoFormatoRelatorio = default)
        {
            ObjetoExportacao = objetoExportacaoExcel;
            NomeWorkSheet = nomeWorkSheet;
            PossuiNotaRodape = possuiNotaRodape;
            NotaRodape = notaRodape;
            RelatorioFrequenciaGlobal = relatorioFrequenciaGlobal;
            MensagemTitulo = mensagemTitulo;
            this.UsuarioRf = usuarioRf;
            this.TipoFormatoRelatorio = tipoFormatoRelatorio;
        }
    }
}
