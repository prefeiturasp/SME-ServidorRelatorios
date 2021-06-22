using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterFrequenciasRelatorioHistoricoEscolarQuery : IRequest<IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        public ObterFrequenciasRelatorioHistoricoEscolarQuery(string[] codigosAluno, int anoLetivo, Modalidade modalidade, int semestre)
        {
            CodigosAluno = codigosAluno;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
        }

        public string[] CodigosAluno { get; set; }
        public int AnoLetivo { get; }
        public Modalidade Modalidade { get; }
        public int Semestre { get; }
    }
}
