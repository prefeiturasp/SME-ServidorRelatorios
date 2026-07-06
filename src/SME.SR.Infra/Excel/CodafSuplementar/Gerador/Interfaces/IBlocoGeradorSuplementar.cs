using ClosedXML.Excel;

namespace SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces
{
    public interface IBlocoGeradorSuplementar<in T>
    {
        int Processar(IXLWorksheet sheet, int linhaInicial, T dados);
    }
}