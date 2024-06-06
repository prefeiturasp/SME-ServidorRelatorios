using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.Conecta
{
    public class RelatorioPaginaLaudaCompletaDto
    {
        public RelatorioPaginaLaudaCompletaDto()
        {
            Campos = new List<RelatorioCampoLaudaCompletaDto>();
        }

        public int Pagina { get; set; }
        public List<RelatorioCampoLaudaCompletaDto> Campos { get; set; }

        public void AdicionarCampo(RelatorioCampoLaudaCompletaDto campo)
        {
            Campos.Add(campo);
        }
    }
}
