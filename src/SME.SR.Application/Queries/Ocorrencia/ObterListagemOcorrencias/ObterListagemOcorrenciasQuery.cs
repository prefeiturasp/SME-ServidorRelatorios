using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterListagemOcorrenciasQuery : IRequest<RelatorioListagemOcorrenciasDto>
    {
        public ObterListagemOcorrenciasQuery(bool exibirHistorico, int anoLetivo, string codigoDre, string codigoUe, int modalidade, int semestre, string[] codigosTurma, DateTime? dataInicio, DateTime? dataFim, long[] ocorrenciaTipoIds, bool imprimirDescricaoOcorrencia)
        {
            ExibirHistorico = exibirHistorico;
            AnoLetivo = anoLetivo;
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
            Modalidade = modalidade;
            Semestre = semestre;
            CodigosTurma = codigosTurma;
            DataInicio = dataInicio;
            DataFim = dataFim;
            OcorrenciaTipoIds = ocorrenciaTipoIds;
            ImprimirDescricaoOcorrencia = imprimirDescricaoOcorrencia;
        }

        public bool ExibirHistorico { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public int Modalidade { get; set; }
        public int Semestre { get; set; }
        public string[] CodigosTurma { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long[] OcorrenciaTipoIds { get; set; }
        public bool ImprimirDescricaoOcorrencia { get; set; }
    }
}
