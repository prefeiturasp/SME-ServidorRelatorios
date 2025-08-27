using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleEditora
    {
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public List<int> EditoraId { get; set; }
    }
}
