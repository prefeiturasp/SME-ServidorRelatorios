using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class ResumoPAPGraficoAnoDto
    {
        public ResumoPAPGraficoAnoDto(decimal larguraTotal, string titulo)
        {
            EixosX = new List<GraficoBarrasPAPVerticalEixoXDto>();
            EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(10, "", 10, 10);
            Titulo = titulo;
            LarguraTotal = larguraTotal;
            IdParaLastro = Guid.NewGuid().ToString().Replace("-", "");
        }

        public List<GraficoBarrasPAPVerticalEixoXDto> EixosX { get; set; }

        public GraficoBarrasPAPVerticalEixoYDto EixoYConfiguracao { get; set; }

        public string Titulo { get; set; }

        public List<GraficoBarrasLegendaDto> Legendas { get; set; }

        public string DescricaoLegenda
        {
            get
            {
                if (Legendas != null && Legendas.Any())
                    return string.Join(" || ", Legendas.Select(m => $"{m.Chave} - {m.Valor}").ToArray());
                else
                    return string.Empty;
            }
        }

        public decimal LarguraTotal { get; set; }

        public string IdParaLastro { get; set; }

    }
}
