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
        public DataTable TabelaDados { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatorioAtaFinalExcelCommand(DataTable tabelaDados, IList<ConselhoClasseAtaFinalPaginaDto> objetoExportacaoExcel, string nomeWorkSheet, Guid codigoCorrelacao)
        {
            TabelaDados = tabelaDados;
            ObjetoExportacao = objetoExportacaoExcel;
            NomeWorkSheet = nomeWorkSheet;
            CodigoCorrelacao = codigoCorrelacao;
        }

    }
}
