using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioOcorrenciaQueryHandler : IRequestHandler<ObterDadosRelatorioOcorrenciaQuery, RelatorioRegistroOcorrenciasDto>
    {
        private readonly IMediator mediator;

        public ObterDadosRelatorioOcorrenciaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioRegistroOcorrenciasDto> Handle(ObterDadosRelatorioOcorrenciaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var relatorio = new RelatorioRegistroOcorrenciasDto();

                await MapearCabecalho(relatorio, request.FiltroOcorrencia);

                await MapearOcorrencias(relatorio, request.FiltroOcorrencia);

                return relatorio;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task MapearCabecalho(RelatorioRegistroOcorrenciasDto relatorio, FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            var dadosDreUe = await ObterNomeDreUe(filtroOcorrencia.TurmaCodigo);

            var ueCodigoConvertido = Convert.ToInt64(dadosDreUe.UeCodigo);

            var enderecoUe = await mediator.Send(new ObterEnderecoUeEolPorCodigoQuery(ueCodigoConvertido));

            relatorio.DreNome = dadosDreUe.UeNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.Endereco = $"{enderecoUe.Logradouro}, {enderecoUe.Numero} - {enderecoUe.Bairro}";
            relatorio.Contato = enderecoUe.TelefoneFormatado;
            relatorio.UsuarioNome = filtroOcorrencia.UsuarioNome;
            relatorio.UsuarioRF = filtroOcorrencia.UsuarioRf;
            relatorio.Ocorrencias = new List<RelatorioOcorrenciasDto>();
        }

        private async Task MapearOcorrencias(RelatorioRegistroOcorrenciasDto relatorio, FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            var ocorrencias = new List<RelatorioOcorrenciasDto>();

            var ocorrenciasList = await mediator.Send(new ObterOcorrenciasPorCodigoETurmaQuery(filtroOcorrencia.TurmaCodigo, filtroOcorrencia.OcorrenciasIds.ToArray()));

            foreach (var item in ocorrenciasList)
            {
                var ocorrencia = new RelatorioOcorrenciasDto()
                {
                    CriancaNome = await ObterNomeAluno(item.CodigoAluno.ToString()),
                    Turma = await ObterNomeTurma(item.TurmaId),
                    DataOcorrencia = item.OcorrenciaDataFormatada(),
                    TipoOcorrencia = item.OcorrenciaTipo,
                    TituloOcorrencia = item.OcorrenciaTitulo,
                    DescricaoOcorrencia = item.OcorrenciaDescricao
                };
                ocorrencias.Add(ocorrencia);
            }

            relatorio.Ocorrencias = ocorrencias;
        }

        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
        {
            return await mediator.Send(new ObterDreUePorTurmaIdQuery(turmaCodigo));
        }

        private async Task<string> ObterNomeAluno(string codigoAluno)
        {
            var aluno = await mediator.Send(new ObterNomeAlunoPorCodigoQuery(codigoAluno));
            
            return aluno.Nome;
        }

        private async Task<string> ObterNomeTurma(long turmaId)
        {
            var consultaTurma = await mediator.Send(new ObterTurmaPorIdQuery(turmaId));

            return consultaTurma.NomePorFiltroModalidade(null);

        }
    }
}