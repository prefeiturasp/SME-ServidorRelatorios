namespace SME.SR.Infra
{
    public class AdesaoAEFiltroDto
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public FiltroRelatorioAEAdesaoEnum OpcaoListaUsuarios { get; set; }
        public string NomeUsuario { get; set; }
    }
    public enum FiltroRelatorioAEAdesaoEnum
    {
        ListarUsuariosNao = 1,
        ListarUsuariosValidos = 2,
        ListarUsuariosCPFIrregular = 3,
        ListarUsuariosCPFTodos = 4

    }
}
