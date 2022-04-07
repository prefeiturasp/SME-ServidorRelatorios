namespace SME.SR.Infra
{
    public class ParametroRelatorioPaginadoPorColuna<T> : ParametroRelatorioPaginado<T> where T : class
    {
        public TipoPapel TipoDePapel { get; set; }

        public int AlturaDaLinha { get; set; }

        public EnumUnidadeDeTamanho UnidadeDeTamanho { get; set; }
    }
}
