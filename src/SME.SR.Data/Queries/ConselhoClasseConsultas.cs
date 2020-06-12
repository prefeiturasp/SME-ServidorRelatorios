namespace SME.SR.Data
{
    public class ConselhoClasseConsultas
    {
        internal static string ConselhoPorFechamentoId = @"select c.id from conselho_classe c where c.fechamento_turma_id = @fechamentoTurmaId";
	}
}
