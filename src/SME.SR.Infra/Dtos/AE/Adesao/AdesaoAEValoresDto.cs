using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAEValoresDto
    {
        public string Nome { get; set; }
        public int SemCpfOuCpfInvalido { get; set; }
        public int NaoRealizaram { get; set; }
        public int PrimeiroAcessoIncompleto { get; set; }
        public int Validos { get; set; }
        public int Total
        {
            get
            {
                return (SemCpfOuCpfInvalido + NaoRealizaram + PrimeiroAcessoIncompleto + Validos);
            }
        }
     
    }
}