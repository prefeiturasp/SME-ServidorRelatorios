using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroRelatorioUsuariosDto
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string UsuarioRf { get; set; }
        public string[] Perfis { get; set; }
        public int[] Situacoes { get; set; }
        public int DiasSemAcesso { get; set; }
        public bool ExibirHistorico { get; set; }
        public string NomeUsuario { get; set; }
    }
}
