using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTiposNotaRelatorioBoletimQuery : IRequest<IDictionary<string, string>>
    {
        public long DreId { get; set; }
        public long UeId { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public Modalidade Modalidade { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
    }
}
