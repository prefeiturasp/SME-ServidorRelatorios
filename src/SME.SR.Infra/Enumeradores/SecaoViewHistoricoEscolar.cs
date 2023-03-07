namespace SME.SR.Infra
{
    public class SecaoViewHistoricoEscolar
    {
        public static readonly SecaoViewHistoricoEscolar TabelaHistoricoTodosAnosFundamental = new SecaoViewHistoricoEscolar("DadosHistoricoFundamental.cshtml");

        public static readonly SecaoViewHistoricoEscolar EstudosRealizados = new SecaoViewHistoricoEscolar("EstudosRealizados.cshtml");

        public static readonly SecaoViewHistoricoEscolar TabelaAnoAtualFundamental = new SecaoViewHistoricoEscolar("DadosTransferenciaFundamental.cshtml");

        public static readonly SecaoViewHistoricoEscolar Observacoes = new SecaoViewHistoricoEscolar("Observacoes.cshtml");

        public static readonly SecaoViewHistoricoEscolar Assinaturas = new SecaoViewHistoricoEscolar("Assinaturas.cshtml");

        public readonly string NomeView; 

        private SecaoViewHistoricoEscolar(string nomeView)
        {
            NomeView = nomeView;
        }
    }
}
