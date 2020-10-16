using MediatR;
using System;

namespace SME.SR.Application
{
    public class ObterTotalAlunosPorUeAnoSondagemQuery : IRequest<int>
    {
        public ObterTotalAlunosPorUeAnoSondagemQuery(string ano, string ueCodigo, int anoLetivo, DateTime dataReferencia, long dreCodigo)
        {
            Ano = ano;
            UeCodigo = ueCodigo;
            AnoLetivo = anoLetivo;
            DataReferencia = dataReferencia;
            DreCodigo = dreCodigo;
        }

        public string Ano { get; set; }
        public string UeCodigo { get; set; }
        public long DreCodigo { get; set; }
        public int AnoLetivo { get; internal set; }
        public DateTime DataReferencia { get; set; }
    }
}
