using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioNotasEConceitosFinaisExcelQueryHandler : IRequestHandler<ObterRelatorioNotasEConceitosFinaisExcelQuery, IEnumerable<RelatorioNotasEConceitosFinaisExcelDto>>
    {
        private readonly IMediator mediator;

        public ObterRelatorioNotasEConceitosFinaisExcelQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<RelatorioNotasEConceitosFinaisExcelDto>> Handle(ObterRelatorioNotasEConceitosFinaisExcelQuery request, CancellationToken cancellationToken)
        {
            var listaNotasEConceitosFinais = new List<RelatorioNotasEConceitosFinaisExcelDto>();

            foreach (var dre in request.RelatorioNotasEConceitosFinais.Dres)
            {
                foreach (var ue in dre.Ues)
                {
                    foreach (var ano in ue.Anos)
                    {
                        foreach (var bimestre in ano.Bimestres)
                        {
                            foreach (var componente in bimestre.ComponentesCurriculares)
                            {
                                foreach (var aluno in componente.NotaConceitoAlunos)
                                {
                                    listaNotasEConceitosFinais.Add(await ObterRelatorioNotasEConceito(dre.Nome, ue.Nome, bimestre.Nome,
                                                                                            ano.Nome, aluno.TurmaNome, componente.Nome,
                                                                                            aluno.AlunoCodigo, aluno.AlunoNomeCompleto, aluno.NotaConceito, aluno.ConselhoClasseAlunoId));
                                }
                            }
                        }

                    }
                }
            }

            return await Task.FromResult(listaNotasEConceitosFinais);
        }

        private async Task<RelatorioNotasEConceitosFinaisExcelDto> ObterRelatorioNotasEConceito(string dreNome, string ueNome, string bimestre,
                                                                                      string ano, string turma, string componenteCurricular,
                                                                                      int alunoCodigo, string alunoNome, string notaConceito, long? conselhoClasseAlunoId)
        {
            RelatorioNotasEConceitosFinaisExcelDto relatorioDto = new RelatorioNotasEConceitosFinaisExcelDto();

            relatorioDto.DreNome = dreNome;
            relatorioDto.UnidadeEscolarNome = ueNome;
            relatorioDto.Bimestre = bimestre;
            relatorioDto.Ano = ano;
            relatorioDto.Turma = turma;
            relatorioDto.ComponenteCurricular = componenteCurricular;
            relatorioDto.EstudanteCodigo = alunoCodigo.ToString();
            relatorioDto.EstudanteNome = alunoNome;
            relatorioDto.NotaConceito = notaConceito;
            relatorioDto.EmAprovacao = false;

            await VerificaSeTemNotaConceitoEmAprovacao(relatorioDto.EstudanteCodigo, conselhoClasseAlunoId, relatorioDto);

            return relatorioDto;
        }

        private async Task VerificaSeTemNotaConceitoEmAprovacao(string codigoAluno, long? conselhoClasseAlunoId, RelatorioNotasEConceitosFinaisExcelDto relatorioDto)
        {
            var dadosNotasConceitosEmAprovacao = await mediator.Send(new ObterNotaConceitoEmAprovacaoQuery(codigoAluno, conselhoClasseAlunoId));

            if (dadosNotasConceitosEmAprovacao != null)
            {
                relatorioDto.NotaConceito = dadosNotasConceitosEmAprovacao.NotaConceito;
                relatorioDto.EmAprovacao = true;
            }
        }
    }
}
