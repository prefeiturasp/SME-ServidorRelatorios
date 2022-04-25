using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Data;

namespace SME.SR.Application
{
    public class GerarRelatorioFrequenciaGlobalExcelCommand : IRequest
    {
        public string NomeWorkSheet { get; set; }
        public IEnumerable<DataTable> TabelasDados { get; set; }
        public string UsuarioRf { get; set; }

        public GerarRelatorioFrequenciaGlobalExcelCommand(IEnumerable<DataTable> tabelaDados, string usuarioRf)
        {
            TabelasDados = tabelaDados;
            NomeWorkSheet = "RelatorioFrequenciaGlobal";
            UsuarioRf = usuarioRf;
        }
    }
}
