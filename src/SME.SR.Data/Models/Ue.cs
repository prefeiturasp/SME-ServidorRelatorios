using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data
{
    public class Ue
    {
        public long Id { get; set; }
        public string Codigo { get; set; }
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
                // Verifica se é maior que 0 E se o número existe de fato no Enum
                if ((int)TipoEscola > 0 && Enum.IsDefined(typeof(TipoEscola), TipoEscola))
                {
                    var atributo = TipoEscola.GetAttribute<DisplayAttribute>();
                    return $"{Codigo} - {atributo?.ShortName ?? "Escola"} {Nome}";
                }

                return $"{Codigo ?? $"{Codigo} - "} {Nome}";
            }
        }

        public string TituloTipoEscolaNome
        {
            get
            {
                if ((int)TipoEscola > 0 && Enum.IsDefined(typeof(TipoEscola), TipoEscola))
                {
                    var atributo = TipoEscola.GetAttribute<DisplayAttribute>();
                    return $"{atributo?.ShortName ?? "Escola"} {Nome}";
                }

                return $"Escola {Nome}";
            }
        }

        public string NomeComTipoEscola { get { return $"{TipoEscola.ShortName()} - {Nome}"; } }

    public string NomeComTipoEscolaEDre { get { return $"{TipoEscola.ShortName()} - {Nome} ({Dre.Abreviacao})"; } }
}
}
