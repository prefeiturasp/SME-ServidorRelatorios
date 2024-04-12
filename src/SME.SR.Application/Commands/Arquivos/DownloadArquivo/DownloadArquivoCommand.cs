using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class DownloadArquivoCommand : IRequest<byte[]>
    {
        public DownloadArquivoCommand(Guid codigoArquivo, string nome, TipoArquivo tipo)
        {
            Codigo = codigoArquivo;
            Nome = nome;
            Tipo = tipo;
        }

        public Guid Codigo;
        public string Nome;
        public TipoArquivo Tipo;
    }
}
