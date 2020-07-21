using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosETurmasHistoricoEscolarQueryHandler : IRequestHandler<ObterAlunosETurmasHistoricoEscolarQuery, IEnumerable<AlunoTurmasHistoricoEscolarDto>>
    {
        private readonly IMediator mediator;

        public ObterAlunosETurmasHistoricoEscolarQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> Handle(ObterAlunosETurmasHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {

            var retorno = new List<AlunoTurmasHistoricoEscolarDto>();

            var pareceresConclusivosIds = await mediator.Send(new ObterPareceresConclusivosPorTipoAprovacaoQuery(true));
            if (!pareceresConclusivosIds.Any())
                throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

            if (request.CodigoAlunos.Any())
            {
                //Obter informações dos alunos
                IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigoAlunos);

                //Obter as turmas dos Alunos
                var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigoAlunos, pareceresConclusivosIds.ToArray()));
                if (!turmasDosAlunos.Any())
                    throw new NegocioException(" Não foi encontrado nenhum histórico de promoção para o(s) aluno(s).");


                foreach (var item in informacoesDosAlunos)
                {
                    var alunoTurmasNotasFrequenciasDto = new AlunoTurmasHistoricoEscolarDto() { Aluno = TransformarDtoAluno(item) };

                    var turmasdoAluno = turmasDosAlunos.Where(a => a.AlunoCodigo == item.CodigoAluno);
                    foreach (var turmaDoAluno in turmasdoAluno)
                    {
                        alunoTurmasNotasFrequenciasDto.Turmas.Add(new Turma() { 
                            Ano = turmaDoAluno.Ano.ToString(), 
                            Codigo = turmaDoAluno.TurmaCodigo,
                            ModalidadeCodigo = turmaDoAluno.Modalidade
                        });
                    }

                    retorno.Add(alunoTurmasNotasFrequenciasDto);
                }
            }
            else
            {
                //Obter os alunos da turma 
                if (request.CodigoTurma != 0)
                {

                    var turma = await mediator.Send(new ObterTurmaQuery() { CodigoTurma = request.CodigoTurma.ToString() });
                    if (turma == null)
                        throw new NegocioException("Não foi possível obter a turma.");

                    var alunosPromovidosCodigos = await mediator.Send(new ObterAlunosPorTurmaParecerConclusivoQuery(request.CodigoTurma, pareceresConclusivosIds.ToArray()));

                    if (!alunosPromovidosCodigos.Any())
                        throw new NegocioException(" Não foi encontrado nenhum histórico de promoção para o(s) aluno(s) da turma.");

                    IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(alunosPromovidosCodigos.Select(a => a.AlunoCodigo).ToArray());

                    foreach (var item in informacoesDosAlunos)
                    {
                        var alunoTurmasNotasFrequenciasDto = new AlunoTurmasHistoricoEscolarDto() { Aluno = TransformarDtoAluno(item) };
                        alunoTurmasNotasFrequenciasDto.Turmas.Add(turma);
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
            informacoesAlunoDto.Rga = item.CodigoAluno.ToString();
            informacoesAlunoDto.Codigo = item.CodigoAluno.ToString();

            return informacoesAlunoDto;
        }
    }
}
