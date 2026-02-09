using ClosedXML.Excel;

namespace SME.SR.Infra.Excel.Codaf.Gerador.Interfaces
{
    public interface IBlocoGerador<in T>
    {
        int Processar(IXLWorksheet sheet, int linhaInicial, T dados);
    }
}