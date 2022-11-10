using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class BoletimEscolarDetalhadoDto
    {
        public List<BoletimEscolarDetalhadoAlunoDto> Boletins { get; set; }
        public bool ExibirRecomendacao { get; set; }
        public Modalidade Modalidade { get; set; }

        public BoletimEscolarDetalhadoDto()
        {
            Boletins = new List<BoletimEscolarDetalhadoAlunoDto>();
        }

        public BoletimEscolarDetalhadoDto(List<BoletimEscolarDetalhadoAlunoDto> boletins, bool exibirRecomendacao)
        {
            if (boletins != null && boletins.Any())
                this.Boletins = boletins;

            this.ExibirRecomendacao = exibirRecomendacao;
        }
    }
}
