using ClosedXML.Excel;
using MediatR;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Excel.Codaf.Gerador;
using SME.SR.Infra.Extensions.Codaf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf
{
    public class GerarPlanilhaCodafCommand : IRequest<byte[]>
    {
        public RelatorioCodafDto DadosRelatorio { get; set; }

        public GerarPlanilhaCodafCommand(RelatorioCodafDto dadosRelatorio)
        {
            DadosRelatorio = dadosRelatorio;
        }
    }

    public class GerarPlanilhaCodafCommandHandler : IRequestHandler<GerarPlanilhaCodafCommand, byte[]>
    {
        private readonly IBlocoTituloGerador _blocoTituloGerador;
        private readonly IBlocoCabecalhoGerador _blocoCabecalhoGerador;

        public GerarPlanilhaCodafCommandHandler(IBlocoTituloGerador blocoTituloGerador, IBlocoCabecalhoGerador blocoCabecalhoGerador)
        {
            _blocoTituloGerador = blocoTituloGerador;
            _blocoCabecalhoGerador = blocoCabecalhoGerador;
        }
        public async Task<byte[]> Handle(GerarPlanilhaCodafCommand request, CancellationToken cancellationToken)
        {
            //new PocCodaf().ExecutarPoc();
            var stream = GerarRelatorio(request.DadosRelatorio);
            var nomeArquivo = $"RelatorioCodaf_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            using var fileStream = File.Create(nomeArquivo);
            await stream.CopyToAsync(fileStream);
            var fileBytes = stream.ToArray();
            stream.Close();

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = nomeArquivo,
                UseShellExecute = true,
                Verb = "open"
            });
            
            return fileBytes;
        }

        private MemoryStream GerarRelatorio(RelatorioCodafDto dadosRelatorio)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            workbook.ConfigurarEstiloPadrao();

            foreach (var turma in dadosRelatorio.Turmas)
            {
                var nomeAba = turma.NomeTurma.Length > 31 ? turma.NomeTurma.Substring(0, 31) : turma.NomeTurma;
                var sheet = workbook.Worksheets.Add(nomeAba);
                ConfigurarLarguraColunas(sheet);

                var linhaAtual = 1;
                
                // 1º Bloco: Título (Brasão)
                linhaAtual = _blocoTituloGerador.Processar(sheet, linhaAtual, null);

                // 2º Bloco: Cabeçalho
                linhaAtual = _blocoCabecalhoGerador.Processar(sheet, linhaAtual, turma.Cabecalho);
            }

            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        private static void ConfigurarLarguraColunas(IXLWorksheet sheet)
        {
            sheet.Column("B").DefinirLarguraCm(3.57);
            sheet.Column("K").DefinirLarguraCm(4.09);
            sheet.Column("L").DefinirLarguraCm(2.29);
            sheet.Column("M").DefinirLarguraCm(2.90);
            sheet.Column("N").DefinirLarguraCm(2.18);
            sheet.Column("O").DefinirLarguraCm(2.75);
            sheet.Column("S").DefinirLarguraCm(2.35);
            sheet.Column("T").DefinirLarguraCm(2.35);
            sheet.Rows(2, 5).Height = 25;
        }

    }
}