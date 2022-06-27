namespace SME.SR.Infra
{
    public class PaginaFrenquenciaMensal : PaginaComColuna
    {
        private const string TODAS = "Todas";
        public PaginaFrenquenciaMensal(string filtroDre, string filtroUe)
        {
            MostrarSecaoDre = filtroDre == TODAS;
            MostrarSecaoUe = filtroUe == TODAS;
        }

        public bool MostrarSecaoDre { get; private set; }

        public bool MostrarSecaoUe { get; private set; }
    }
}
