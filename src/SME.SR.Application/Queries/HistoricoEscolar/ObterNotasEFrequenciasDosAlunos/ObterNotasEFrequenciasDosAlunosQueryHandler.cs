using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasEFrequenciasDosAlunosQueryHandler : IRequestHandler<ObterNotasEFrequenciasDosAlunosQuery, IEnumerable<AlunoTurmasNotasFrequenciasDto>>
    {
        private readonly IMediator mediator;

        public ObterNotasEFrequenciasDosAlunosQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<AlunoTurmasNotasFrequenciasDto>> Handle(ObterNotasEFrequenciasDosAlunosQuery request, CancellationToken cancellationToken)
        {

            var retorno = new List<AlunoTurmasNotasFrequenciasDto>();

            if (request.CodigoAlunos.Any())
            {
                var pareceresConclusivosCodigos = await mediator.Send(new ObterPareceresConclusivosPorTipoAprovacaoQuery(true, true, false, false));
                if (!pareceresConclusivosCodigos.Any())
                    throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

                //Obter informações dos alunos
                IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigoAlunos);

                //Obter as turmas dos Alunos
                var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigoAlunos, pareceresConclusivosCodigos.ToArray()));
                if (!turmasDosAlunos.Any())
                    throw new NegocioException("Não foi possíve obter as turmas do(s) aluno(s)");


                foreach (var item in informacoesDosAlunos)
                {
                    var alunoTurmasNotasFrequenciasDto = new AlunoTurmasNotasFrequenciasDto() { Aluno = TransformarDtoAluno(item) };
                    alunoTurmasNotasFrequenciasDto.Turmas = turmasDosAlunos.Where(a => a.AlunoCodigo == item.CodigoAluno)
                        .Select(a => a.TurmaCodigo)
                        .ToArray();

                    retorno.Add(alunoTurmasNotasFrequenciasDto);
                }

            }
            else
            {
                //Obter os alunos da turma
                if (request.CodigoTurma != 0)
                {
                    var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaQuery() { TurmaCodigo = request.CodigoTurma.ToString() });
                    if (!alunosDaTurma.Any())
                        throw new NegocioException("Não foi possível obter os alunos da turma");

                    //TODO: Fazer a Query Acima já buscar essas informações
                    IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(alunosDaTurma.Select(a => (long)a.CodigoAluno).ToArray());


                    foreach (var item in informacoesDosAlunos)
                    {
                        var alunoTurmasNotasFrequenciasDto = new AlunoTurmasNotasFrequenciasDto() { Aluno = TransformarDtoAluno(item) };
                        alunoTurmasNotasFrequenciasDto.Turmas = new long[] { request.CodigoTurma };
                        retorno.Add(alunoTurmasNotasFrequenciasDto);
                    }
                }
                 

            }

            return retorno;

        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterInformacoesDosAlunos(long[] codigoAlunos)
        {
            var informacoesDosAlunos = await mediator.Send(new ObterDadosAlunosHistoricoEscolarQuery() { CodigosAluno = codigoAlunos });
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possíve obter os dados dos alunos");
            return informacoesDosAlunos;
        }

        private InformacoesAlunoDto TransformarDtoAluno(AlunoHistoricoEscolar item)
        {
            var informacoesAlunoDto = new InformacoesAlunoDto();

            informacoesAlunoDto.CidadeNatal = item.CidadeNatal;
            informacoesAlunoDto.Estado = string.Empty;
            informacoesAlunoDto.EstadoNatal = item.EstadoNatal;
            informacoesAlunoDto.Expedicao = item.ExpedicaoData.ToString("dd/MM/yyyy");
            informacoesAlunoDto.Nacionalidade = item.Nacionalidade;
            informacoesAlunoDto.Nascimento = item.DataNascimento.ToString("dd/MM/yyyy");
            informacoesAlunoDto.Nome = item.ObterNomeFinal();
            informacoesAlunoDto.OrgaoExpeditor = item.ExpedicaoOrgaoEmissor;
            informacoesAlunoDto.Rg = item.RG;
            informacoesAlunoDto.Rga = string.Empty;

            return informacoesAlunoDto;
        }
    }
}
