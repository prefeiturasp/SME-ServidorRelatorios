using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SME.SR.Infra
{
    public enum Sintese
    {
        [Display(Name = "Frequente")]
        Frequente = 1,

        [Display(Name = "Não frequente")]
        NaoFrequente = 2
    }
}
