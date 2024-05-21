using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class UePorDresIdResultDto
    {
        public long Id { get; set; }
        public long DreId { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string NomeRelatorio =>
            $"{Codigo} - {TipoEscola.GetAttribute<DisplayAttribute>().ShortName ?? "Escola"} {Nome}";
        public string TituloTipoEscolaNome =>
            $"{TipoEscola.GetAttribute<DisplayAttribute>().ShortName ?? "Escola"} {Nome}";
    }
}
