using MediatR;
using SME.SR.Infra.Dtos.NovoSondagem;
using System;
using System.Collections.Generic;

namespace SME.SR.Application.Commands.NovoSondagem.GerarRelatorioSondagemQuestionarioHtmlParaPdf
{
    public class GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand(
            string nomeTemplate,
            List<QuestionarioSondagemRelatorioDto> paginas,
            Guid codigoCorrelacao,
            string mensagemUsuario)
        {
            NomeTemplate = nomeTemplate;
            Paginas = paginas;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
        }

        public Guid CodigoCorrelacao { get; set; }
        public string NomeTemplate { get; set; }
        public List<QuestionarioSondagemRelatorioDto> Paginas { get; set; }
        public string MensagemUsuario { get; set; }
    }
}