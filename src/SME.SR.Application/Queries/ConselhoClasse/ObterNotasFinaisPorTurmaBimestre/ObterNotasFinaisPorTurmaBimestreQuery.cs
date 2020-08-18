using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmaBimestreQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public ObterNotasFinaisPorTurmaBimestreQuery(string turmaCodigo, int[] bimestres, CondicoesRelatorioNotasEConceitosFinais? condicao, int? valorCondicao)
        {
            TurmaCodigo = turmaCodigo;
            Bimestres = bimestres;
            Condicao = condicao;
            ValorCondicao = valorCondicao;
        }

        public string TurmaCodigo { get; set; }
        public int[] Bimestres { get; set; }
        public CondicoesRelatorioNotasEConceitosFinais? Condicao { get; set; }
        public int? ValorCondicao { get; set; }
    }
}
