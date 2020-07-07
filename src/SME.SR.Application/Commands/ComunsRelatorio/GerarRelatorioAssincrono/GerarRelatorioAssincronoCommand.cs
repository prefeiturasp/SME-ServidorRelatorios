using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommand : IRequest<bool>
    {
        public GerarRelatorioAssincronoCommand(string caminhoRelatorio, string dados, TipoFormatoRelatorio formato, Guid codigoCorrelacao)
        {
            CaminhoRelatorio = caminhoRelatorio;
            Dados = dados;
            Formato = formato;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public Guid CodigoCorrelacao { get; set; }
        public string CaminhoRelatorio { get; set; }
        public string Dados { get; set; }
        public TipoFormatoRelatorio Formato { get; set; }
    }
}
