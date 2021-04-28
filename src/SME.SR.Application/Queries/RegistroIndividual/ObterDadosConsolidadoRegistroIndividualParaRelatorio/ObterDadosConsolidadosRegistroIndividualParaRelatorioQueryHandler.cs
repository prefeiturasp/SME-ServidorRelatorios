using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosConsolidadosRegistroIndividualParaRelatorioQueryHandler : IRequestHandler<ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery, RelatorioRegistroIndividualDto>
    {
        public Task<RelatorioRegistroIndividualDto> Handle(ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery request, CancellationToken cancellationToken)
        {

            var alunosEol = request.AlunosEol;
            var turma = request.Turma;
            var ueEndereco = request.UeEndereco;
            var registrosIndividuais = request.RegistrosIndividuais;
            var filtro = request.Filtro;

            var relatorio = new RelatorioRegistroIndividualDto
            {
                Cabecalho = GerarCabecalho(turma, ueEndereco, filtro),
                Alunos = GerarAlunos(registrosIndividuais, alunosEol),
            };

            return Task.FromResult(relatorio);
        }

        private RelatorioRegistroIndividualCabecalhoDto GerarCabecalho(Turma turma, UeEolEnderecoDto ueEndereco, FiltroRelatorioRegistroIndividualDto filtro)
        {
            return new RelatorioRegistroIndividualCabecalhoDto()
            {
                Dre = turma.Dre.Abreviacao,
                Ue = turma.Ue.NomeComTipoEscola,
                Endereco = ueEndereco.Endereco,
                Telefone = ueEndereco.TelefoneFormatado,
                Turma = turma.NomeRelatorio,
                Usuario = filtro.UsuarioNome,
                RF = filtro.UsuarioRF,
                Periodo = filtro.Periodo,
            };
        }

        private List<RelatorioRegistroIndividualAlunoDto> GerarAlunos(IEnumerable<RegistroIndividualRetornoDto> registrosIndividuais, IEnumerable<AlunoReduzidoDto> alunosEol)
        {
            var registrosIndividuaisAlunos = new List<RelatorioRegistroIndividualAlunoDto>();

            var alunosCodigosFiltrados = registrosIndividuais.Select(r => r.AlunoCodigo).Distinct();

            foreach (var alunoCodigo in alunosCodigosFiltrados)
            {
                registrosIndividuaisAlunos.Add(new RelatorioRegistroIndividualAlunoDto
                {
                    Nome = alunosEol.FirstOrDefault(a => a.AlunoCodigo == alunoCodigo).NomeRelatorio,
                    Registros = GerarRegistros(registrosIndividuais.Where(r => r.AlunoCodigo == alunoCodigo).OrderByDescending(r => r.DataRegistro)),
                });
            }
            return registrosIndividuaisAlunos.OrderBy(a => a.Nome).ToList();
        }

        private List<RelatorioRegistroIndividualDetalhamentoDto> GerarRegistros(IEnumerable<RegistroIndividualRetornoDto> registrosIndividuais)
        {
            var registros = new List<RelatorioRegistroIndividualDetalhamentoDto>();

            foreach (var registro in registrosIndividuais)
            {
                registros.Add(new RelatorioRegistroIndividualDetalhamentoDto
                {
                    DataRegistro = registro.DataRelatorioFormatada(), //registro.DataRelatorio,
                    Descricao = registro.RegistroFormatado(),
                    RegistradoPor = registro.RegistradoPor,
                });
            }
            return registros;
        }
    }
}
