using MediatR;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterRelatorioBoletimEscolarQuery : IRequest<BoletimEscolarDto>
    {
        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public long? CicloId { get; set; }

        public long? PeriodoEscolarId { get; set; }

        public string TurmaCodigo { get; set; }

        public int? AnoLetivo { get; set; }

        public Modalidade? Modalidade { get; set; }

        public string[] AlunosCodigo { get; set; }
    }
}
