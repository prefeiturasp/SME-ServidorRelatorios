using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class AgruparRelatorioAtribuicaoCJPorDreUeQueryHandler : IRequestHandler<AgruparRelatorioAtribuicaoCJPorDreUeQuery, RelatorioAtribuicaoCjDto>
    {
        private readonly IMediator mediator;

        public AgruparRelatorioAtribuicaoCJPorDreUeQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioAtribuicaoCjDto> Handle(AgruparRelatorioAtribuicaoCJPorDreUeQuery request, CancellationToken cancellationToken)
        {
            var relatorioDto = request.RelatorioAtribuicaoCJ;

            var dresCodigo = new List<string>();
            var uesCodigo = new List<string>();

            if (relatorioDto.AtribuicoesEsporadicas.Any())
            {
                dresCodigo.AddRange(relatorioDto.AtribuicoesEsporadicas.Select(a => a.CodigoDre));
                uesCodigo.AddRange(relatorioDto.AtribuicoesEsporadicas.Select(a => a.CodigoUe));
            }

            if (relatorioDto.AtribuicoesCjPorProfessor.Any())
            {
                var atribuicoesTurma = relatorioDto.AtribuicoesCjPorProfessor.SelectMany(a => a.AtribuiicoesCjTurma);

                dresCodigo.AddRange(atribuicoesTurma.Select(a => a.CodigoDre));
                uesCodigo.AddRange(atribuicoesTurma.Select(a => a.CodigoUe));
            }

            if (relatorioDto.AtribuicoesCjPorTurma.Any())
            {
                var atribuicoesProf = relatorioDto.AtribuicoesCjPorTurma.SelectMany(a => a.AtribuicoesCjProfessor);

                dresCodigo.AddRange(atribuicoesProf.Select(a => a.CodigoDre));
                uesCodigo.AddRange(atribuicoesProf.Select(a => a.CodigoUe));
            }

            dresCodigo = dresCodigo.Distinct().ToList();
            uesCodigo = uesCodigo.Distinct().ToList();

            await MapearDresUes(dresCodigo, uesCodigo, relatorioDto);

            AdicionarAtribuicoes(relatorioDto);

            return relatorioDto;
        }

        private void AdicionarAtribuicoes(RelatorioAtribuicaoCjDto relatorioDto)
        {
            foreach (var dre in relatorioDto.Dres)
            {
                foreach (var ue in dre.Ues)
                {
                    ue.AtribuicoesEsporadicas = relatorioDto.AtribuicoesEsporadicas.Where(a => a.CodigoDre == dre.Codigo && a.CodigoUe == ue.Codigo)?.ToList();
                    ue.AtribuicoesCjPorProfessor = relatorioDto.AtribuicoesCjPorProfessor.Where(p => p.AtribuiicoesCjTurma.Any(a => a.CodigoDre == dre.Codigo && a.CodigoUe == ue.Codigo))?.ToList();
                    ue.AtribuicoesCjPorTurma = relatorioDto.AtribuicoesCjPorTurma.Where(t => t.AtribuicoesCjProfessor.Any(a => a.CodigoDre == dre.Codigo && a.CodigoUe == ue.Codigo))?.ToList();
                }
            }

            relatorioDto.AtribuicoesEsporadicas.Clear();
            relatorioDto.AtribuicoesCjPorProfessor.Clear();
            relatorioDto.AtribuicoesCjPorTurma.Clear();
        }

        private async Task MapearDresUes(List<string> dresCodigo, List<string> uesCodigo, RelatorioAtribuicaoCjDto relatorioDto)
        {
            var dres = await mediator.Send(new ObterTodasDresQuery());
            var ues = await mediator.Send(new ObterPorDresIdQuery(dres.Select(dre => dre.Id).ToArray()));

            relatorioDto.Dres.AddRange(dresCodigo.Select(dreCodigo =>
            {
                var dreDetalhe = dres.FirstOrDefault(d => d.Codigo == dreCodigo);

                var dreDto = new RelatorioAtribuicaoCjDreDto(dreDetalhe.Abreviacao, dreCodigo);

                var uesDaDre = ues.Where(ue => ue.DreId == dreDetalhe.Id && uesCodigo.Contains(ue.Codigo));

                dreDto.Ues.AddRange(uesDaDre.Select(ue =>
                {
                    var ueDto = new RelatorioAtribuicaoCjUeDto(ue.NomeRelatorio, ue.Codigo);
                    return ueDto;
                }));

                return dreDto;
            }));
        }
    }
}
