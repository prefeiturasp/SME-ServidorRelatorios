using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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
            DreUe dadosDreUe;
            if (filtroOcorrencia.TurmaId != 0)
                dadosDreUe = await ObterNomeDreUe(filtroOcorrencia.TurmaId);
            else
                dadosDreUe = await mediator.Send(new ObterDreUePorDreCodigoQuery(filtroOcorrencia.DreCodigo, filtroOcorrencia.UeCodigo));

            var ueCodigoConvertido = Convert.ToInt64(dadosDreUe.UeCodigo);

            var enderecoUe = await mediator.Send(new ObterEnderecoUeEolPorCodigoQuery(ueCodigoConvertido));

            relatorio.DreNome = dadosDreUe.DreNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.Endereco = $"{enderecoUe.Logradouro}, {enderecoUe.Numero} - {enderecoUe.Bairro}";
            relatorio.Contato = enderecoUe.TelefoneFormatado;
            relatorio.UsuarioNome = filtroOcorrencia.UsuarioNome;
            relatorio.UsuarioRF = filtroOcorrencia.UsuarioRf;
            relatorio.Ocorrencias = new List<RelatorioOcorrenciasDto>();
        }

        private async Task MapearOcorrencias(RelatorioRegistroOcorrenciasDto relatorio, FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            var ocorrenciasRelatorio = new List<RelatorioOcorrenciasDto>();

            var ocorrencias = await mediator.Send(new ObterOcorrenciasPorCodigoETurmaQuery(filtroOcorrencia.TurmaId, filtroOcorrencia.OcorrenciasIds.ToArray()));

            var codigosAlunos = ocorrencias.Select(a => a.CodigoAluno.ToString());
            var alunos = await ObterAlunos(codigosAlunos.ToArray());

            foreach (var item in ocorrencias)
            {
                var aluno = alunos.FirstOrDefault(a => a.Codigo == item.CodigoAluno.ToString());

                var ocorrencia = new RelatorioOcorrenciasDto()
                {
                    CriancaNome = $"{aluno.Nome}({aluno.Codigo})",
                    Turma = await ObterNomeTurma(item.TurmaId),
                    DataOcorrencia = item.OcorrenciaData,
                    TipoOcorrencia = item.OcorrenciaTipo,
                    TituloOcorrencia = item.OcorrenciaTitulo,
                    DescricaoOcorrencia = item.OcorrenciaDescricao
                };
                ocorrenciasRelatorio.Add(ocorrencia);
            }

            relatorio.Ocorrencias = ocorrenciasRelatorio;
        }

        private async Task<DreUe> ObterNomeDreUe(long turmaId)
        {
            return await mediator.Send(new ObterDreUePorTurmaIdQuery(turmaId));
        }

        private async Task<IEnumerable<AlunoNomeDto>> ObterAlunos(string[] codigosAlunos)
        {
            return await mediator.Send(new ObterNomesAlunosPorCodigosQuery(codigosAlunos));
        }

        private async Task<string> ObterNomeTurma(long turmaId)
        {
            var consultaTurma = await mediator.Send(new ObterTurmaPorIdQuery(turmaId));

            return consultaTurma.NomePorFiltroModalidade(null);

        }
    }
}