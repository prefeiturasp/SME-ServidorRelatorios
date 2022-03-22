using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarDetalhadoQuery : IRequest<BoletimEscolarDetalhadoEscolaAquiDto>
    {
        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public int Semestre { get; set; }

        public string TurmaCodigo { get; set; }

        public int AnoLetivo { get; set; }

        public Modalidade Modalidade { get; set; }

        public string[] AlunosCodigo { get; set; }

        public Usuario Usuario { get; set; }

        public bool ConsideraHistorico { get; set; }
    }
}
