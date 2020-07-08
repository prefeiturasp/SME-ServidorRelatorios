using MediatR;
using System;

namespace SME.SR.Application
{
    public class DownloadArquivoLocalQuery : IRequest<byte[]>
    {
        public DownloadArquivoLocalQuery(string arquivoNome, string pastaFisicaCaminho)
        {
            ArquivoNome = arquivoNome;
            PastaFisicaCaminho = pastaFisicaCaminho;
        }

        public string ArquivoNome { get; set; }

        public string PastaFisicaCaminho { get; set; }
    }
}
