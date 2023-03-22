using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public abstract class RelatorioSondagemAnaliticoPorDreDto
    {
        public string Dre { get; set; }
        public string DreSigla { get; set; }
        public string DescricaoTipoSondagem => TipoDaSondagem.GetAttribute<DisplayAttribute>().Name;
        protected abstract TipoSondagem TipoDaSondagem { get; }
    }
}
