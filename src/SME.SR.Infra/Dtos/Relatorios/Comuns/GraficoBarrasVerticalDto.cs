using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalDto
    {
        public GraficoBarrasVerticalDto(decimal larguraTotal, string titulo)
        {
            EixosX = new List<GraficoBarrasVerticalEixoXDto>();
            LarguraTotal = larguraTotal;
            EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(10, "", 10, 10);
            Titulo = titulo;
            IdParaLastro = Guid.NewGuid().ToString().Replace("-", "");
        }

        public List<GraficoBarrasVerticalEixoXDto> EixosX { get; set; }
        public GraficoBarrasVerticalEixoYDto EixoYConfiguracao { get; set; }
        public List<GraficoBarrasLegendaDto> Legendas { get; set; }

        public string DescricaoLegenda {
            get {
                if (Legendas != null && Legendas.Any())
                    return string.Join(" || ", Legendas.Select(m => $"{m.Chave} - {m.Valor}").ToArray());
                else
                    return string.Empty;
            }
        }

        public decimal LarguraTotal { get; set; }

        public string Titulo { get; set; }

        public string IdParaLastro { get; set; }
    }
}
