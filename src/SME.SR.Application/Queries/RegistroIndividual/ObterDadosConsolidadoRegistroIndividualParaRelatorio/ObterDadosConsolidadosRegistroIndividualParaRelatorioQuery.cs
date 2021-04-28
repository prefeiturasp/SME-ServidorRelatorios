using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery : IRequest<RelatorioRegistroIndividualDto>
    {
        public ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery(Turma turma, UeEolEnderecoDto ueEndereco, IEnumerable<AlunoReduzidoDto> alunosEol, IEnumerable<RegistroIndividualRetornoDto> registrosIndividuais, FiltroRelatorioRegistroIndividualDto filtro)
        {
            Turma = turma;
            UeEndereco = ueEndereco;
            AlunosEol = alunosEol;
            RegistrosIndividuais = registrosIndividuais;
            Filtro = filtro;
        }

        public Turma Turma { get; set; }
        public UeEolEnderecoDto UeEndereco { get; set; }
        public IEnumerable<AlunoReduzidoDto> AlunosEol { get; set; }
        public IEnumerable<RegistroIndividualRetornoDto> RegistrosIndividuais { get; set; }
        public FiltroRelatorioRegistroIndividualDto Filtro { get; set; }

    }
}
