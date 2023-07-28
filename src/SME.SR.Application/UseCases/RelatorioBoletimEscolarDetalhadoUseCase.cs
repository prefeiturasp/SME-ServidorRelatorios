using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioBoletimEscolarDetalhadoUseCase : IRelatorioBoletimEscolarDetalhadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioBoletimEscolarDetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            if (request.Action == "relatorios/acompanhamento-aprendizagem-escolaaqui" ||
                request.Action == "relatorios/boletimescolardetalhadoescolaaqui")
                request.RelatorioEscolaAqui = true;

            if (request.RelatorioEscolaAqui)
            {
                request.RotaErro = RotasRabbitSGP.RotaRelatorioComErro;
                request.RotaProcessando = RotasRabbitSR.RotaRelatoriosSolicitadosBoletimDetalhadoEscolaAqui;
                var relatorioQuery = request.ObterObjetoFiltro<ObterDadosMensagemEscolaAquiQuery>();
                relatorioQuery.Modalidade = ObterModalidade(relatorioQuery.ModalidadeCodigo);
                if (relatorioQuery.Usuario == null)
                    relatorioQuery.Usuario = await ObterUsuarioLogado(request.UsuarioLogadoRF);
                var relatorio = await mediator.Send(relatorioQuery);
                relatorioQuery.CodigoArquivo = request.CodigoCorrelacao;
                var mensagemdados = UtilJson.ConverterApenasCamposNaoNulos(relatorioQuery);
                await mediator.Send(new GerarRelatorioHtmlPDFBoletimDetalhadoCommand(relatorio, request.CodigoCorrelacao, relatorioQuery.Modalidade, mensagemDados: mensagemdados));
            }
            else
            {
                request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroBoletimDetalhado;
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarDetalhadoQuery>();
                var relatorio = await mediator.Send(relatorioQuery);
                await mediator.Send(new GerarRelatorioHtmlPDFBoletimDetalhadoCommand(relatorio, request.CodigoCorrelacao, relatorioQuery.Modalidade));
            }
        }

        private Modalidade ObterModalidade(int modalidadeId)
        {
            return Enum.GetValues(typeof(Modalidade))
            .Cast<Modalidade>().FirstOrDefault(x => (int)x == modalidadeId);
        }
        private async Task<Usuario> ObterUsuarioLogado(string usuarioLogadorf)
        {
            return await mediator.Send(new ObterUsuarioPorCodigoRfQuery(usuarioLogadorf));
        }
    }
}