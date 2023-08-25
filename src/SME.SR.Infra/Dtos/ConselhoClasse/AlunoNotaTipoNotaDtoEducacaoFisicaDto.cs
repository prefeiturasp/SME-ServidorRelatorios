using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AlunoNotaTipoNotaDtoEducacaoFisicaDto
    {
        public AlunoNotaTipoNotaDtoEducacaoFisicaDto(bool ehConceito, string alunoCodigo)
        {
            EhConceito = ehConceito;
            AlunoCodigo = alunoCodigo;
        }

        public string AlunoCodigo { get; set; }
        public bool EhConceito { get; set; }
    }
}
