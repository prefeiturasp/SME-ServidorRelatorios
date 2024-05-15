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
        public PropostaLocal LocalEncontro { get; set; }
        public double TotalDeVagas { get { return QuantidadeTurmas.GetValueOrDefault() * QuantidadeVagasTurmas.GetValueOrDefault(); } }
        public string TotalCargaHoraria { get { return ObterTotalCargaHoraria(); } }
        public string CargaHorariaSincronaDistancia { get { return ObterCargaHorariaSincronaDistancia(); } }
        public bool EhAreaPromotoraDireta { get {  return TipoAreaPromotora == AREA_PROMOTORA_DIRETA; } }
        public string CargaHorariaPresencialFormatada { get { return ObterHora(CargaHorariaPresencial).ToString(@"hh\:mm"); } }
        public string CargaHorariaSincronaFormatada { get { return ObterHora(CargaHorariaSincrona).ToString(@"hh\:mm"); } }
        public string CargaHorariaDistanciaFormatada { get { return ObterHora(CargaHorariaDistancia).ToString(@"hh\:mm"); } }

        public string ObterPeriodoRealizacao()
        {
            return $"{DataRealizacaoInicio.ToString("dd/MM/yyyy")} ATÉ {DataRealizacaoFim.ToString("dd/MM/yyyy")}";
        }

        public string ObterLocalUmaTurmaUmEncontro()
        {
            if (!(LocalEncontro is null) &&
                !string.IsNullOrEmpty(LocalEncontro.Local) &&
                LocalEncontro.TotalTurmas == 1)
                return $@"{LocalEncontro.ObterData()} - {LocalEncontro.Local.ToUpper()}";

            return string.Empty;
        }

        public string ObterLocalVariasTurmasUmEncontro()
        {
            if (!(LocalEncontro is null) && !string.IsNullOrEmpty(LocalEncontro.Local))
                return $@"{LocalEncontro.ObterData()} - {LocalEncontro.HoraInicio} ATÉ {LocalEncontro.HoraFim} - {LocalEncontro.Local.ToUpper()}";

            return string.Empty;
        }

        private string ObterTotalCargaHoraria()
        {
            var total = ObterHora(CargaHorariaPresencial) +
                        ObterHora(CargaHorariaSincrona) +
                        ObterHora(CargaHorariaDistancia);

            return total.ToString(@"hh\:mm");
        }

        private string ObterCargaHorariaSincronaDistancia()
        {
            var total = ObterHora(CargaHorariaSincrona) + ObterHora(CargaHorariaDistancia);

            if (total != TimeSpan.Zero)
                return total.ToString(@"hh\:mm");

            return string.Empty;
        }

        private TimeSpan ObterHora(string hora)
        {
            return string.IsNullOrEmpty(hora) ? TimeSpan.Zero : ObterTimeSpan(hora);
        }

        private TimeSpan ObterTimeSpan(string hora)
        {
            var horaSeparado = hora.Split(":");

            if (horaSeparado.Length > 0)
                return new TimeSpan(int.Parse(horaSeparado[0]), int.Parse(horaSeparado[1]), 0);

            return TimeSpan.Zero;
        }
    }
}
