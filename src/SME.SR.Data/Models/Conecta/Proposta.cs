using System;
using System.Collections.Generic;

namespace SME.SR.Data.Models.Conecta
{
    public class Proposta
    {
        private const int AREA_PROMOTORA_DIRETA = 1;

        public long Id { get; set; }
        public string NomeAreaPromotora { get; set; }
        public int TipoAreaPromotora { get; set; }
        public string NomeFormacao { get; set; }
        public string CargaHorariaPresencial { get; set; }
        public string CargaHorariaSincrona { get; set; }
        public string CargaHorariaDistancia { get; set; }
        public DateTime DataRealizacaoInicio { get; set; }
        public DateTime DataRealizacaoFim { get; set; }
        public short? QuantidadeTurmas { get; set; }
        public short? QuantidadeVagasTurmas { get; set; }
        public DateTime DataInscricaoInicio { get; set; }
        public DateTime DataInscricaoFim { get; set; }
        public long NumeroHomologacao {  get; set; }
        public string LinkInscricaoExterna { get; set; }
        public IEnumerable<PropostaPublicoAlvo> PublicosAlvo { get; set; }
        public IEnumerable<PropostaCriterioDeValidacao> CriteriosValidacao { get; set; }
        public IEnumerable<PropostaCriterioCertificacao> CriteriosCertificacao { get; set; }
        public IEnumerable<PropostaRegente> Regentes { get; set; }
        public double TotalDeVagas { get { return QuantidadeTurmas.GetValueOrDefault() * QuantidadeVagasTurmas.GetValueOrDefault(); } }
        public string TotalCargaHoraria { get { return ObterTotalCargaHoraria(); } }
        public bool EhAreaPromotoraDireta { get {  return TipoAreaPromotora == AREA_PROMOTORA_DIRETA; } }

        private string ObterTotalCargaHoraria()
        {
            var total = ObterHora(CargaHorariaPresencial) +
                        ObterHora(CargaHorariaSincrona) +
                        ObterHora(CargaHorariaDistancia);

            return total.ToString("HH:MM");
        } 

        private TimeSpan ObterHora(string hora)
        {
            return string.IsNullOrEmpty(hora) ? TimeSpan.Zero : TimeSpan.Parse(hora);
        }
    }
}
