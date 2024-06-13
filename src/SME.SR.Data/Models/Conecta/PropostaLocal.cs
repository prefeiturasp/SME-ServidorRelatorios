using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Data.Models.Conecta
{
    public class PropostaLocal
    {
        public int TotalTurmas { get; set; }
        public string Local { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string Turma { get; set; }
        public TipoEncontro TipoEncontro { get; set; }

        public string ObterLocalDetalhado()
        {
            return $@"{Turma.ToUpper()} - {ObterData()} - {HoraInicio} ATÉ {HoraFim} - {TipoEncontro.Name().ToUpper()} - {Local?.ToUpper()}";
        }

        public string ObterData()
        {
            var datafim = DataFim.HasValue ? $" ATÉ {DataFim.GetValueOrDefault().ToString("dd/MM/yyyy")}" : string.Empty;

            return $"{ DataInicio.ToString("dd/MM/yyyy")}{ datafim}";
        }
    }
}
