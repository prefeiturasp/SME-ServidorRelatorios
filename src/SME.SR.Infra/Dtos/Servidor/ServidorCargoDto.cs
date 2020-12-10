namespace SME.SR.Infra
{
    public class ServidorCargoDto
    {
        public string NomeServidor { get; set; }
        public string CodigoRF { get; set; }
        public string NomeCargo { get; set; }
        public string CodigoCargo { get; set; }
        public string CodigoTipoFuncao { get; set; }
        public string NomeCargoSobreposto { get; set; }
        public string CodigoCargoSobreposto { get; set; }
        public bool Sobreposto { get; set; }
        public string CodigoCargoBase { get; set; }
        public string CodigoComponenteCurricular { get; set; }
        public string NomeRelatorio => $"{NomeServidor} ({CodigoRF})";
        public string CargoRelatorio => $"{NomeCargo} ({CodigoCargo})";

        public bool PossuiCargoSobrepostoGestao()
        {
            return Sobreposto && (CodigoCargo == ((int)Cargo.AD).ToString() ||
                                  CodigoCargo == ((int)Cargo.CP).ToString() ||
                                  CodigoCargo == ((int)Cargo.Diretor).ToString());
        }
    }
}
