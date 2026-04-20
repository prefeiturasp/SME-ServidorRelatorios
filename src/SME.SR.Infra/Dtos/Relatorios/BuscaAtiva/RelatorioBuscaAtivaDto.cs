using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioBuscaAtivaDto
    {
        public RelatorioBuscaAtivaDto()
        {
            RegistrosAcaoDreUe = new List<AgrupamentoBuscaAtivaDreUeDto>();
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UsuarioNome { get; set; }
        public int AnoLetivo { get; set; }
        public int? Semestre { get; set; }
        public List<string> UesNome { get; set; } = new List<string>();
        public string Turma { get; set; }
        public Modalidade Modalidade { get; set; }
        public List<AgrupamentoBuscaAtivaDreUeDto> RegistrosAcaoDreUe { get; set; }
    }
}
