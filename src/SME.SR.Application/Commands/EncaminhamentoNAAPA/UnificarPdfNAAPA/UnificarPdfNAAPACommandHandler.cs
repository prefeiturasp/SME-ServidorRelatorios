using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands
{
    public class UnificarPdfNAAPACommandHandler : IRequestHandler<UnificarPdfNAAPACommand, bool>
    {
        private readonly IMediator mediator;
        private string nomePdfUnificado;
        private string diretorioPdfGerado;
        private List<ArquivoDto> anexos;
        private List<string> caminhoArquivoAnexo;

        public UnificarPdfNAAPACommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.caminhoArquivoAnexo = new List<string>();
        }

        public async Task<bool> Handle(UnificarPdfNAAPACommand request, CancellationToken cancellationToken)
        {
            this.nomePdfUnificado = request.NomePdfUnificado;
            this.anexos = request.Anexos;
            this.diretorioPdfGerado = ObterDiretorioPdfGerado(request.PdfGerado);
            this.caminhoArquivoAnexo.Add(diretorioPdfGerado);

            if (NaoPodeUnificarPdf())
                return false;

            await ExecuteDownloadDosAnexos();

            var unificador = new UnificadorPdf(nomePdfUnificado, this.caminhoArquivoAnexo);

            unificador.Processar();

            RemoveArquivosGerados();

            return true;
        }

        private bool NaoPodeUnificarPdf()
        {
            return !(anexos.Any()
                && !string.IsNullOrEmpty(nomePdfUnificado)
                && !string.IsNullOrEmpty(diretorioPdfGerado)
                && File.Exists(diretorioPdfGerado));
        }

        private void RemoveArquivosGerados()
        {
            foreach(var pdf in this.caminhoArquivoAnexo)
                File.Delete(pdf);
        }

        private string ObterDiretorioPdfGerado(string nomePdf)
        {
            var diretorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");

            return Path.Combine(diretorio, ObterNomeArquivo(nomePdf));
        }

        private async Task ExecuteDownloadDosAnexos()
        {
            foreach (var arquivo in anexos)
            {
                var arquivoFisico = await mediator.Send(new DownloadArquivoCommand(arquivo.Codigo, ObterNomeArquivo(arquivo.Codigo.ToString()), arquivo.Tipo));

                if (!(arquivoFisico is null))
                {
                    var caminho = ObterDiretorioPdfGerado(arquivo.Codigo.ToString());
                    File.WriteAllBytes(caminho, arquivoFisico);
                    this.caminhoArquivoAnexo.Add(caminho);
                }
            }
        }

        private string ObterNomeArquivo(string nomeArquivo)
        {
            return $"{nomeArquivo}.pdf";
        }
    }
}
