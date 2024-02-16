using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class Evento
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Descricao { get; set; }
        public TipoEvento TipoEvento { get; set; }
        public long TipoEventoId { get; set; }
        public EventoTipo EventoTipo { get; set; }
        public EventoLetivo Letivo { get; set; }
        public string UeId { get; set; }
        public string DreId { get; set; }

        public bool ExisteTipoEvento()
        {
            return Enum.IsDefined(typeof(TipoEvento), TipoEvento);
        }

        public bool EhEventoLetivo()
        {
            return Letivo == EventoLetivo.Sim;
        }
        public bool EhEventoNaoLetivo()
        {
            return Letivo == EventoLetivo.Nao;
        }

        public IEnumerable<DateTime> ObterIntervaloDatas()
        {
            var datas = new List<DateTime>();
            for (var dia = DataInicio.Date; dia <= DataFim.Date; dia = dia.AddDays(1))
                datas.Add(dia);
            return datas;
        }

        public bool EhEventoSME() =>
            string.IsNullOrWhiteSpace(DreId) && string.IsNullOrWhiteSpace(UeId);

        public bool EhEventoDRE() =>
            !string.IsNullOrWhiteSpace(DreId) && string.IsNullOrWhiteSpace(UeId);

        public bool EhEventoUE() =>
            !string.IsNullOrWhiteSpace(DreId) && !string.IsNullOrWhiteSpace(UeId);

    }
}
