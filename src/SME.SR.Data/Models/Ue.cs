using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data
{
    public class Ue
    {
        public long Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string DreId { get; set; }
        public Dre Dre { get; set; }

        public string NomeRelatorio =>
            $"{Codigo} - {TipoEscola.GetAttribute<DisplayAttribute>().ShortName ?? "Escola"} {Nome}";

        public string NomeComTipoEscola { get { return $"{TipoEscola.ShortName()} - {Nome}"; } }

        public string NomeComTipoEscolaEDre { get { return $"{TipoEscola.ShortName()} - {Nome} ({Dre.Abreviacao})"; } }
    }
}
