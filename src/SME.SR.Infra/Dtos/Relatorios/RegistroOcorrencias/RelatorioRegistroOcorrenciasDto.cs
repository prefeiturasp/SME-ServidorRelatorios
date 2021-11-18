using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRegistroOcorrenciasDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Endereco { get; set; }
        public string Contato { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string DataImpressao => DateTime.Now.ToString("dd/MM/yyyy");
        public List<RelatorioOcorrenciasDto> Ocorrencias { get; set; }
    }
}
