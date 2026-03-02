namespace SME.SR.Infra.Dtos.NovoSondagem
{
    /// <summary>
    /// Mapeamento do novo formato de mensagem da fila.
    /// Os dados de DRE, UE, Turma e Modalidade não vêm mais na mensagem —
    /// são buscados via queries no UseCase usando TurmaId.
    /// </summary>
    public class FiltroRelatorioSondagemQuestionarioDto
    {
        // ── Filtros de consulta (vêm dentro de "FiltrosUsados") ──
        public int TurmaId { get; set; }
        public int ProficienciaId { get; set; }
        public int ComponenteCurricularId { get; set; }
        public int Modalidade { get; set; }
        public int Ano { get; set; }
        public int AnoLetivo { get; set; }
        public int SemestreId { get; set; }
        public string UeCodigo { get; set; } = string.Empty;
        public int? BimestreId { get; set; }
        public int ExtensaoRelatorio { get; set; }

        // ── Campos da raiz de Mensagem ──
        public int SolicitacaoRelatorioId { get; set; }
        public int TipoRelatorio { get; set; }
        public int StatusSolicitacao { get; set; }

        // Recebidos mas sem uso direto por ora — mantidos para uso futuro
        public string UsuarioQueSolicitou { get; set; } = string.Empty;
    }
}