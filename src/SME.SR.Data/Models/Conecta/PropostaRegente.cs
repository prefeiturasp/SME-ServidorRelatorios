namespace SME.SR.Data.Models.Conecta
{
    public class PropostaRegente
    {
        public long PropostaId { get; set; }
        public string Nome { get; set; }
        public string Rf { get; set; }
        public string MiniBio { get; set; }
        public bool ProfissionalDaRede { get; set; }

        public string ObterDescricaoCompleta()
        {
            var rf = ProfissionalDaRede ? $" - RF: {Rf} " : string.Empty;
            var miniBio = string.IsNullOrEmpty(MiniBio) ? string.Empty : $" - {MiniBio}";

            return $"{Nome}{rf}{miniBio}";
        }
    }
}
