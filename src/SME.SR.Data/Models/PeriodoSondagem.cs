namespace SME.SR.Data
{
    public class PeriodoSondagem
    {
        public string Id { get; set; }

        public string Descricao { get; set; }

        public int Periodo
        {
            get
            {
                if (!string.IsNullOrEmpty(Descricao))
                    return int.Parse(Descricao.Substring(0, 1));
                else
                    return 0;
            }
        }
    }
}
