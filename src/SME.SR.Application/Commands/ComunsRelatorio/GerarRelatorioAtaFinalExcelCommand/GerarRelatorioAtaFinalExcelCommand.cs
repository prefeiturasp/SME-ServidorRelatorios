using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaFinalExcelCommand : IRequest
    {
        public IList<ConselhoClasseAtaFinalPaginaDto> ObjetoExportacao { get; }
        public string NomeWorkSheet { get; set; }
        public IEnumerable<DataTable> TabelasDados { get; set; }
        public string UsuarioRf { get; set; }

        public GerarRelatorioAtaFinalExcelCommand(IEnumerable<DataTable> tabelaDados, IList<ConselhoClasseAtaFinalPaginaDto> objetoExportacaoExcel, string nomeWorkSheet, string usuarioRf)
        {
            TabelasDados = tabelaDados;
            ObjetoExportacao = objetoExportacaoExcel;
            NomeWorkSheet = nomeWorkSheet;
            UsuarioRf = usuarioRf;
        }

    }
}
