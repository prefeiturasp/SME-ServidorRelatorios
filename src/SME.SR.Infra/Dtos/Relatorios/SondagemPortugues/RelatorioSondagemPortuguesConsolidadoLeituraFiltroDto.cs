using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto
    {
        private string usuarioRF;
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int AnoLetivo { get; set; }
        public int Ano { get; set; }
        public int Bimestre { get; set; }
        public ProficienciaSondagemEnum ProficienciaId { get; set; }
        public string UsuarioRF
        {
            get => usuarioRF;
            set => usuarioRF = value?.Trim();
        }
        public string GrupoId { get; set; }
        public int[] Modalidades { get; set; }
    }
}
