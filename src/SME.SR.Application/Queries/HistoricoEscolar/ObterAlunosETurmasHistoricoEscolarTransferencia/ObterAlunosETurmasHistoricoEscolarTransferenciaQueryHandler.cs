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
    class ObterAlunosETurmasHistoricoEscolarTransferenciaQueryHandler : IRequestHandler<ObterAlunosETurmasHistoricoEscolarTransferenciaQuery, IEnumerable<AlunoTurmasHistoricoEscolarDto>>
    {
        private readonly IMediator mediator;

        public ObterAlunosETurmasHistoricoEscolarTransferenciaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> Handle(ObterAlunosETurmasHistoricoEscolarTransferenciaQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoTurmasHistoricoEscolarDto>();

            if (request.CodigoAlunos.Any())
            {
                //Obter informações dos alunos
                IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigoAlunos);

                //Obter as turmas dos Alunos
                var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosSemParecerQuery(request.CodigoAlunos));
                if (!turmasDosAlunos.Any())
                    return retorno;

                foreach (var item in informacoesDosAlunos)
                {
                    var alunoTurmasNotasFrequenciasDto = new AlunoTurmasHistoricoEscolarDto() { Aluno = TransformarDtoAluno(item) };

                    var turmasdoAluno = turmasDosAlunos.Where(a => a.AlunoCodigo == item.CodigoAluno);
                    foreach (var turmaDoAluno in turmasdoAluno)
                    {
                        alunoTurmasNotasFrequenciasDto.Turmas.Add(new Turma()
                        {
                            Ano = turmaDoAluno.Ano.ToString(),
                            Codigo = turmaDoAluno.TurmaCodigo,
                            ModalidadeCodigo = turmaDoAluno.Modalidade,
                            EtapaEJA = turmaDoAluno.EtapaEJA,
                            Ciclo = turmaDoAluno.Ciclo,
                            RegularCodigo = turmaDoAluno.RegularCodigo,
                            TipoTurma = turmaDoAluno.TipoTurma
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
                        return retorno;

                    var alunosPromovidosCodigos = await mediator.Send(new ObterAlunosPorTurmaSemParecerConclusivoQuery(request.CodigoTurma));
                    if (!alunosPromovidosCodigos.Any())
                        return retorno;

                    var codigoAlunos = alunosPromovidosCodigos.Select(a => a.AlunoCodigo).ToArray();
                    var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosSemParecerQuery(codigoAlunos));
                    if (!turmasDosAlunos.Any())
                        return retorno;

                    IEnumerable<AlunoHistoricoEscolar> informacoesDosAlunos = await ObterInformacoesDosAlunos(alunosPromovidosCodigos.Select(a => a.AlunoCodigo).ToArray());

                    foreach (var item in informacoesDosAlunos)
                    {
                        var alunoTurmasNotasFrequenciasDto = new AlunoTurmasHistoricoEscolarDto() { Aluno = TransformarDtoAluno(item) };

                        var turmasDoAluno = turmasDosAlunos
                            .Where(x => x.AlunoCodigo == item.CodigoAluno)
                            .Select(x => new Turma()
                            {
                                Ano = x.Ano.ToString(),
                                Codigo = x.TurmaCodigo,
                                ModalidadeCodigo = x.Modalidade,
                                EtapaEJA = x.EtapaEJA,
                                Ciclo = x.Ciclo,
                                RegularCodigo = x.RegularCodigo,
                                TipoTurma = x.TipoTurma
                            })
                            .ToList();
                        alunoTurmasNotasFrequenciasDto.Turmas.AddRange(turmasDoAluno);
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
                throw new NegocioException("Não foi possível obter os dados dos alunos");

            informacoesDosAlunos = informacoesDosAlunos.GroupBy(d => d.CodigoAluno)
                                   .SelectMany(g => g.OrderByDescending(d => d.AnoLetivo)
                                                     .ThenByDescending(m => m.DataSituacao)
                                                     .Take(1));

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
            informacoesAlunoDto.CodigoSituacaoMatricula = item.CodigoSituacaoMatricula;
            informacoesAlunoDto.SituacaoMatricula = item.SituacaoMatricula;
            informacoesAlunoDto.DataSituacao = item.DataSituacao;
            informacoesAlunoDto.Ativo = item.EstaAtivo(DateTime.Now);

            return informacoesAlunoDto;
        }
    }
}
