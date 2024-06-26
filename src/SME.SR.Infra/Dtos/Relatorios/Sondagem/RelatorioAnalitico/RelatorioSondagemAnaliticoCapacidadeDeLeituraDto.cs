﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoCapacidadeDeLeituraDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public RelatorioSondagemAnaliticoCapacidadeDeLeituraDto()
        {
            Respostas = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();
        }
        public List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto> Respostas { get; set; }
        protected override TipoSondagem TipoDaSondagem => TipoSondagem.LP_CapacidadeLeitura;
    }
}
