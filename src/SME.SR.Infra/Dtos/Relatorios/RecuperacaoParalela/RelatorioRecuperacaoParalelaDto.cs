using System;
using System.Collections.Generic;
using System.Globalization;

namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaDto
    {

        public RelatorioRecuperacaoParalelaDto(string dreNome, string ueNome, string usuarioNome, string usuarioRF, string data, int anoLetivo, int semestre)
        {
            DreNome = dreNome;
            UeNome = ueNome;
            UsuarioNome = usuarioNome;
            UsuarioRF = usuarioRF;
            Data = DateTime.Now.ToString("dd/MM/yyyy");
            AnoLetivo = anoLetivo;
            Alunos = new List<RelatorioRecuperacaoParalelaAlunoDto>();
            Semestre = semestre;
        }

        public RelatorioRecuperacaoParalelaDto(string dreNome, string ueNome)
        {
            DreNome = dreNome;
            UeNome = ueNome;
            Data = DateTime.Now.ToString("dd/MM/yyyy");
            Alunos = new List<RelatorioRecuperacaoParalelaAlunoDto>();
        }

        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public List<RelatorioRecuperacaoParalelaAlunoDto> Alunos { get; set; }
    }
}
