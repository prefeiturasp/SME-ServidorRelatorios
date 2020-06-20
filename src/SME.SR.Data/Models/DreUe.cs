namespace SME.SR.Data
{
    public class DreUe
    {
        public long DreId { get; set; }
        public string DreCodigo { get; set; }
        public string DreNome { get; set; }
        public long UeId { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }

        public string UeNomeRelatorio =>
            $"UE - {UeNome}";
    }
}
