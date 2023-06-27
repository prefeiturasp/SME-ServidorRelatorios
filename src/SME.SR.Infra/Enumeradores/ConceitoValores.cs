using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace SME.SR.Infra
{
    public enum ConceitoValores
    {
        [Display(Name = "Plenamente Satisfatório")]
        P = 1,

        [Display(Name = "Satisfatório")]
        S = 2,

        [Display(Name = "Não Satisfatório")]
        NS = 3,
    }
}
