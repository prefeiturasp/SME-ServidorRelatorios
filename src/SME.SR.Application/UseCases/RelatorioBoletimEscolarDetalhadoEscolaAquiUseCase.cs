using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioBoletimEscolarDetalhadoEscolaAquiUseCase : IRelatorioBoletimEscolarDetalhadoEscolaAquiUseCase
    {
        private readonly IMediator mediator;
        public RelatorioBoletimEscolarDetalhadoEscolaAquiUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroBoletimDetalhadoEscolaAqui;
            request.RotaProcessando = RotasRabbitSR.RotaRelatoriosSolicitadosBoletimDetalhadoEscolaAqui;
            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioBoletimEscolarDetalhadoEscolaAquiQuery>();
            relatorioQuery.Modalidade = ObterModalidade(relatorioQuery.ModalidadeCodigo);
            relatorioQuery.Usuario = await ObterUsuarioLogado(request.UsuarioLogadoRF);
            var relatorio = await mediator.Send(relatorioQuery);
            relatorioQuery.CodigoArquivo = request.CodigoCorrelacao;
            var mensagemdados = UtilJson.ConverterApenasCamposNaoNulos(relatorioQuery);
            await mediator.Send(new GerarRelatorioHtmlPDFBoletimDetalhadoAppCommand(relatorio, request.CodigoCorrelacao, relatorioQuery.Modalidade,mensagemDados: mensagemdados));
        }
        private Modalidade ObterModalidade(int modalidadeId)
        {
            return Enum.GetValues(typeof(Modalidade))
            .Cast<Modalidade>().Where(x => (int)x == modalidadeId).FirstOrDefault();
        }
        private async Task<Usuario> ObterUsuarioLogado(string usuarioLogadorf)
        {
            return await mediator.Send(new ObterUsuarioPorCodigoRfQuery(usuarioLogadorf));
        }
    }
}
