using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public static class BimestreConstants
    {
        public static string ObterCondicaoBimestre(int bimestre, bool ehModalidadeInfantil)
        {
            return ehModalidadeInfantil ? (bimestre <= 2 ? " <=2 " : " > 2") : " = @bimestre";
        }
    }
}
