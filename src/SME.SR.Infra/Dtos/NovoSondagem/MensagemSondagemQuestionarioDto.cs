namespace SME.SR.Infra.Dtos.NovoSondagem
{
    /// <summary>
    /// Reflete o novo formato de mensagem da fila com FiltrosUsados aninhado.
    /// Usado apenas para deserializar a mensagem — é mapeado para
    /// FiltroRelatorioSondagemQuestionarioDto no UseCase.
    /// </summary>
    public class MensagemSondagemQuestionarioDto
    {
        public FiltrosUsadosSondagemDto FiltrosUsados { get; set; }
        public int SolicitacaoRelatorioId { get; set; }
        public int TipoRelatorio { get; set; }
        public int ExtensaoRelatorio { get; set; }
        public string UsuarioQueSolicitou { get; set; } = string.Empty;
        public int StatusSolicitacao { get; set; }
    }

    /// <summary>
    /// Campos de filtro aninhados dentro de FiltrosUsados na mensagem.
    /// </summary>
    public class FiltrosUsadosSondagemDto
    {
        public int ExtensaoRelatorio { get; set; }
        public int TurmaId { get; set; }
        public int ProficienciaId { get; set; }
        public int ComponenteCurricularId { get; set; }
        public int Modalidade { get; set; }
        public int Ano { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public string UeCodigo { get; set; } = string.Empty;
        public int? BimestreId { get; set; }
    }
}