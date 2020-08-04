using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaDto
    {
        public RelatorioRecuperacaoParalelaDto(string dreNome, string ueNome, string turmaNome, string usuarioNome, string usuarioRF, string data, int anoLetivo, int semestre)
        {
            DreNome = dreNome;
            UeNome = ueNome;
            UsuarioNome = usuarioNome;
            UsuarioRF = usuarioRF;
            Data = data;
            AnoLetivo = anoLetivo;
            Alunos = new List<RelatorioRecuperacaoParalelaAlunoDto>();
            Semestre = semestre;
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
