﻿using MediatR;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarExcelGenericoCommand: IRequest
    {
        public IList<object> ObjetoExportacao { get; }
        public string NomeWorkSheet { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public bool PossuiNotaRodape { get; set; }
        public string NotaRodape { get; set; }

        public GerarExcelGenericoCommand(IList<object> objetoExportacaoExcel, string nomeWorkSheet, Guid codigoCorrelacao, bool possuiNotaRodape = false, string notaRodape = null)
        {
            ObjetoExportacao = objetoExportacaoExcel;
            NomeWorkSheet = nomeWorkSheet;
            CodigoCorrelacao = codigoCorrelacao;
            PossuiNotaRodape = possuiNotaRodape;
            NotaRodape = notaRodape;
        }
    }
}
