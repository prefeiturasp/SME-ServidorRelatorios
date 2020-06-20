namespace SME.SR.Data
{
    public class ComponenteCurricularApiEol
    {
        public long IdComponenteCurricular { get; set; }
        public long IdComponenteCurricularPai { get; set; }
        public string Descricao { get; set; }
        public bool EhCompartilhada { get; set; }
        public bool EhRegencia { get; set; }
        public bool PermiteRegistroFrequencia { get; set; }
        public bool EhTerritorio { get; set; }
        public bool PermiteLancamentoDeNota { get; set; }
        public bool EhBaseNacional { get; set; }
        public long IdGrupoMatriz { get; set; }
    }
}
