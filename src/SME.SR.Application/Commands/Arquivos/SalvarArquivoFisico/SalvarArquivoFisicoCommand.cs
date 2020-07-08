using MediatR;

namespace SME.SR.Application
{
    public class SalvarArquivoFisicoCommand : IRequest<bool>
    {
        public byte[] Arquivo { get; internal set; }
        public string Pasta { get; internal set; }
        public string ArquivoNome { get; internal set; }

        public SalvarArquivoFisicoCommand(byte[] arquivo, string pasta, string arquivoNome)
        {
            Arquivo = arquivo;
            Pasta = pasta;
            ArquivoNome = arquivoNome;
        }
    }
}
