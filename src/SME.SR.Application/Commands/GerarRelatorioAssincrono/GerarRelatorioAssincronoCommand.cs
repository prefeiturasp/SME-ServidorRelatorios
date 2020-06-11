using MediatR;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommand : IRequest<bool>
    {
        public GerarRelatorioAssincronoCommand(string caminhoRelatorio, string dados, FormatoEnum formato)
        {
            CaminhoRelatorio = caminhoRelatorio;
            Dados = dados;
            Formato = formato;
        }

        public string CaminhoRelatorio { get; set; }
        public string Dados { get; set; }
        public FormatoEnum Formato { get; set; }
    }
}
