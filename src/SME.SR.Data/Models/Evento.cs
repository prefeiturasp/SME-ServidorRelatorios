using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class Evento
    {
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Descricao { get; set; }
        public TipoEvento TipoEvento { get; set; }
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
        public bool NaoEhEventoLetivo()
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
    }
}
