using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery : IRequest<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto>
    {
        public Dre Dre { get; set; }

        public Ue Ue { get; set; }

        public int TurmaAno { get; set; }

        public int TurmaCodigo { get; set; }

        public int Bimestre { get; set; }

        public int AnoLetivo { get; set; }

        public Usuario Usuario { get; set; }
        public int Semestre { get; set; }
    }
}
