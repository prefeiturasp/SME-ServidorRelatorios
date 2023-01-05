using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentoAeeUseCase : IRelatorioEncaminhamentoAeeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentoAeeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoAeeDto>();
            var encaminhamentosAee = await mediator.Send(new EncaminhamentoAEEQuery(filtroRelatorio));

            if (encaminhamentosAee == null || !encaminhamentosAee.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var planosAgrupados = encaminhamentosAee.GroupBy(g => new
            {
                DreNome = g.DreAbreviacao,
                UeNome = $"{g.UeCodigo} - {g.TipoEscola.ShortName()} {g.UeNome}",
            }, (key, group) =>
            new AgrupamentoEncaminhamentoAeeDreUeDto()
            {
                DreNome = key.DreNome,
                UeNome = key.UeNome,
                Detalhes = group.Select(s =>
                new DetalheEncaminhamentoAeeDto()
                {
                    Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                    Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}",
                    Situacao = ((SituacaoPlanoAee)s.Situacao).Name(),
                    ResponsavelPAAI = !string.IsNullOrEmpty(s.ResponsavelPaaiNome) ? $"{s.ResponsavelPaaiNome} ({s.ResponsavelPaaiLoginRf})" : string.Empty,
                }).OrderBy(oAluno => oAluno.Aluno).ThenBy(oAluno => oAluno.Turma).ToList()
            }).OrderBy(oDre => oDre.DreNome).ThenBy(oUe => oUe.UeNome).ToList();
            


            throw new NotImplementedException();
        }
    }
}
