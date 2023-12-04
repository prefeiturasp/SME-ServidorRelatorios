using MediatR;
using System;

namespace SME.SR.Application
{
    public class ObterTotalAlunosPorUeAnoSondagemQuery : IRequest<int>
    {
        public ObterTotalAlunosPorUeAnoSondagemQuery(string ano, string ueCodigo, int anoLetivo, DateTime dataReferenciaFim, long dreCodigo, int[] modalidades, bool consideraHistorico = false, DateTime? dataReferenciaInicio = null)
        {
            Ano = ano;
            UeCodigo = ueCodigo;
            AnoLetivo = anoLetivo;
            DataReferenciaFim = dataReferenciaFim;
            DataReferenciaInicio = dataReferenciaInicio; 
            DreCodigo = dreCodigo;
            Modalidades = modalidades;
            ConsideraHistorico = consideraHistorico;
        }

        public string Ano { get; set; }
        public string UeCodigo { get; set; }
        public long DreCodigo { get; set; }
        public int AnoLetivo { get; internal set; }
        public DateTime DataReferenciaFim { get; set; }
        public DateTime? DataReferenciaInicio { get; set; }
        public int[] Modalidades { get; set; }
        public bool ConsideraHistorico { get; set; }
    }
}
