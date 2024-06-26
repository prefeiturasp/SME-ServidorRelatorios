﻿namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaFiltroDto
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int AnoLetivo { get; set; }
        public int Ano { get; set; }
        public int Bimestre { get; set; }
        public ProficienciaSondagemEnum ProficienciaId { get; set; }
        public string UsuarioRF { get; set; }
        public string GrupoId { get; set; }
        public int Semestre { get; set; }
    }
}
