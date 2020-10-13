using MediatR;
using System;

namespace SME.SR.Application
{
    public class ObterTotalAlunosPorUeAnoSondagemQuery : IRequest<int>
    {
        public ObterTotalAlunosPorUeAnoSondagemQuery(string ano, long ueCodigo, int anoLetivo, DateTime dataReferencia)
        {
            Ano = ano;
            UeCodigo = ueCodigo;
            AnoLetivo = anoLetivo;
            DataReferencia = dataReferencia;
        }

        public string Ano { get; set; }
        public long UeCodigo { get; set; }
        public int AnoLetivo { get; internal set; }
        public DateTime DataReferencia { get; set; }
    }
}
