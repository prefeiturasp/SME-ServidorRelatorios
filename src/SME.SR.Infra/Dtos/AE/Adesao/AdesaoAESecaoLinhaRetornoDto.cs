using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAESecaoLinhaRetornoDto
    {
        public AdesaoAESecaoLinhaRetornoDto()
        {
            UEs = new List<AdesaoAESecaoLinhaRetornoDto>();
        }
        public string Nome { get; set; }
        public int SemCpfOuCpfInvalido { get; set; }
        public int NaoRealizaram { get; set; }
        public int PrimeiroAcessoIncompleto { get; set; }
        public int Validos { get; set; }
        public int Total { get; set; }
        public List<AdesaoAESecaoLinhaRetornoDto> UEs { get; set; }
    }
}