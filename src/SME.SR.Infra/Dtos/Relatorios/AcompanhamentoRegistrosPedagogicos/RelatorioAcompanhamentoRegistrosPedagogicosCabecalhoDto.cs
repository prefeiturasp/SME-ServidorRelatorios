using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string Bimestre { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");

        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto()
        {

        }

        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto(string dre, string ue, string turma, string bimestre, string usuarioNome, string usuarioRF)
        {
            Dre = dre;
            Ue = ue;
            Turma = turma;
            Bimestre = bimestre;
            UsuarioNome = usuarioNome;
            UsuarioRF = usuarioRF;
        }
    }
}
