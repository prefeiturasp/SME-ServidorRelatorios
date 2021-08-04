using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommand : IRequest<bool>
    {
        public GerarRelatorioAssincronoCommand(string caminhoRelatorio, string dados, TipoFormatoRelatorio formato, Guid codigoCorrelacao, string rotaProcessando)
        {
            CaminhoRelatorio = caminhoRelatorio;
            Dados = dados;
            Formato = formato;
            CodigoCorrelacao = codigoCorrelacao;
            RotaProcessando = rotaProcessando;
        }

        public Guid CodigoCorrelacao { get; set; }
        public string CaminhoRelatorio { get; set; }
        public string Dados { get; set; }
        public string RotaProcessando { get; set; }
        public TipoFormatoRelatorio Formato { get; set; }
    }
}
