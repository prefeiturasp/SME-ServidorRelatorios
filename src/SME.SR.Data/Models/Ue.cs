using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data
{
    public class Ue
    {
        public long Id { get; set; }
        public string UeCodigo { get; set; }
        public string Nome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public long DreId { get; set; }
        public Dre Dre { get; set; }

        public void AdicionarDre(Dre dre)
        {
            if (dre != null)
            {
                Dre = dre;
                DreId = dre.Id;
            }
        }

        public string NomeRelatorio
        {
            get
            {
                if ((int)TipoEscola > 0)
                    return $"{UeCodigo} - {TipoEscola.GetAttribute<DisplayAttribute>().ShortName ?? "Escola"} {Nome}";


                return $"{UeCodigo ?? $"{UeCodigo} - "} {Nome}";
            }
        }

    public string TituloTipoEscolaNome =>
        $"{TipoEscola.GetAttribute<DisplayAttribute>().ShortName ?? "Escola"} {Nome}";

    public string NomeComTipoEscola { get { return $"{TipoEscola.ShortName()} - {Nome}"; } }

    public string NomeComTipoEscolaEDre { get { return $"{TipoEscola.ShortName()} - {Nome} ({Dre.Abreviacao})"; } }
}
}
