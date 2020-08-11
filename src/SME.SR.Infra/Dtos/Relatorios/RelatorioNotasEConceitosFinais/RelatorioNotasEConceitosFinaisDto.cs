using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDto
    {

        public RelatorioNotasEConceitosFinaisDto(string dreNome, string ueNome, string turmaNome, string bimestre, string componenteCurricular, string usuarioNome, string usuarioRF, string data, int anoLetivo, int semestre)
        {
            DreNome = dreNome;
            UeNome = ueNome;
            TurmaNome = turmaNome;
            Bimestre = bimestre;
            ComponenteCurricular = componenteCurricular;
            UsuarioNome = usuarioNome;
            UsuarioRF = usuarioRF;
            Data = DateTime.Now.ToString("dd/MM/yyyy");
            Dres = new List<RelatorioNotasEConceitosFinaisDreDto>();
        }

        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string TurmaNome { get; set; }
        public string Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data { get; set; }

        public List<RelatorioNotasEConceitosFinaisDreDto> Dres { get; set; }
    }
}
