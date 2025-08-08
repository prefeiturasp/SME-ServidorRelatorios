using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP
{
    public abstract class GerarRelatorioControleLivrosEmprestadosBase
    {
        protected static string ObterCabecalhoHtml(string usuario, string rf)
        {
            return $@"
                        <div style='display: flex; justify-content: space-between; align-items: center; padding: 10px;'>
                            <div style='text-align: center;'>
                                <p style='font-size: 14px; font-weight: bold; margin-bottom: 5px;'>SGP - SISTEMA DE GESTÃO PEDAGÓGICA</p>
                                <h3 style='margin-top: 0;'>Relatório de Controle de Livros Emprestados</h3>
                            </div>
                        </div>
                        <table border='1' cellpadding='5' cellspacing='0' style='width: 100%; margin-bottom: 20px; border-collapse: collapse;'>
                            <tr>
                                <td><strong>{usuario}</td>
                                <td><strong>RF: {rf}</strong></td>
                                <td><strong>DATA:</strong> {DateTime.Now.ToString("dd-MM-yyyy")}</td>
                            </tr>
                        </table>
                    ";
        }

        protected static async Task SaveMemoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            memoryStream.Position = 0;

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await memoryStream.CopyToAsync(fileStream);
            }
        }
    }
}
