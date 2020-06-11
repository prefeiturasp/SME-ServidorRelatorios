using MediatR;
using System;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommand : IRequest<bool>
    {
        public GerarRelatorioAssincronoCommand(string caminhoRelatorio, string dados, FormatoEnum formato, Guid codigoCorrelacao)
        {
            CaminhoRelatorio = caminhoRelatorio;
            Dados = dados;
            Formato = formato;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public Guid CodigoCorrelacao { get; set; }
        public string CaminhoRelatorio { get; set; }
        public string Dados { get; set; }
        public FormatoEnum Formato { get; set; }
    }
}
