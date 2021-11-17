using System;

namespace SME.SR.Infra.Dtos
{
    public class AusenciaBimestreDto
    {
        public string CodigoAluno { get; set; }
        public DateTime DataAusencia { get; set; }
        public string MotivoAusencia { get; set; }
        public int Bimestre { get; set; }
    }
}
