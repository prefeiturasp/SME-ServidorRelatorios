﻿using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum NotificacaoCategoria
    {
        [Display(Name = "Alerta")]
        Alerta = 1,

        [Display(Name = "Ação")]
        Workflow_Aprovacao = 2,

        [Display(Name = "Aviso")]
        Aviso = 3
    }
}
