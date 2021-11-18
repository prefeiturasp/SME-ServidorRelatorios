using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioOcorrenciaCommandHandler : IRequestHandler<ObterDadosRelatorioOcorrenciaCommand, RelatorioRegistroOcorrenciasDto>
    {
        private readonly IMediator mediator;

        private readonly IAlunoRepository alunoRepository;

        private readonly IUeEolRepository ueEolRepository;

        private readonly OcorrenciaRepository ocorrenciaRepository;

        public ObterDadosRelatorioOcorrenciaCommandHandler(IMediator mediator, IAlunoRepository alunoRepository, IUeEolRepository ueEolRepository, OcorrenciaRepository ocorrenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.ueEolRepository = ueEolRepository ?? throw new ArgumentNullException(nameof(ueEolRepository));
            this.ocorrenciaRepository = ocorrenciaRepository ?? throw new ArgumentException(nameof(ocorrenciaRepository));
        }

        public async Task<RelatorioRegistroOcorrenciasDto> Handle(ObterDadosRelatorioOcorrenciaCommand request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioRegistroOcorrenciasDto();

            var alunosSelecionados = new List<AlunoNomeDto>();

            await MapearCabecalho(relatorio, request.FiltroOcorrencia);
        }

        private async Task MapearCabecalho(RelatorioRegistroOcorrenciasDto relatorio, FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            var dadosDreUe = await ObterNomeDreUe(filtroOcorrencia.TurmaCodigo);

            var ueCodigoConvertido = Convert.ToInt64(dadosDreUe.UeCodigo);

            var enderecoUe = await ueEolRepository.ObterEnderecoUePorCodigo(ueCodigoConvertido);

            relatorio.DreNome = dadosDreUe.UeNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.Endereco = $"{enderecoUe.Logradouro}, {enderecoUe.Numero} - {enderecoUe.Bairro}";
            relatorio.Contato = enderecoUe.TelefoneFormatado;
            relatorio.UsuarioNome = filtroOcorrencia.UsuarioNome;
            relatorio.UsuarioRF = filtroOcorrencia.UsuarioRf;            
            relatorio.Ocorrencias = new List<RelatorioOcorrenciasDto>();
        }

        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
        {
            return await mediator.Send(new ObterDreUePorTurmaQuery(turmaCodigo));
        }
    }
}