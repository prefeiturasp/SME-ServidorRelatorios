namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoDto(int naoIniciado, int processadoComPendencia, int processadoComSucesso)
        {
            NaoIniciado = naoIniciado;
            ProcessadoComPendencia = processadoComPendencia;
            ProcessadoComSucesso = processadoComSucesso;
        }

        public int NaoIniciado { get; set; }
        public int ProcessadoComPendencia { get; set; }
        public int ProcessadoComSucesso { get; set; }
    }
}
