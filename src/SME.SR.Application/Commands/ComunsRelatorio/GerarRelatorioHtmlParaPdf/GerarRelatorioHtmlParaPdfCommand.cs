using MediatR;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Collections.Generic;
using System.Text;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, List<ConselhoClasseAtaFinalPaginaDto> paginas, Guid codigoCorrelacao)
        {
            NomeTemplate = nomeTemplate;
            Paginas = paginas;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }

        public List<ConselhoClasseAtaFinalPaginaDto> Paginas { get; set; }
    }
}
