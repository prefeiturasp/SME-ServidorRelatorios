using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosConsolidadosRegistroIndividualParaRelatorioQueryHandler : IRequestHandler<ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery, RelatorioRegistroIndividualDto>
    {
        public Task<RelatorioRegistroIndividualDto> Handle(ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery request, CancellationToken cancellationToken)
        {
            var retorno = new RelatorioRegistroIndividualDto();
            retorno.Cabecalho = GerarCabecalho();
            retorno.Alunos.Add(GerarAluno());

            return Task.FromResult(retorno);
        }

        private RelatorioRegistroIndividualCabecalhoDto GerarCabecalho()
        {
            return new RelatorioRegistroIndividualCabecalhoDto()
            {
                Dre = "DRE - BT",
                Ue = "EMEI ANTONIO BENTO",
                Endereco = "RUA JOÃO BATISTA DE SOUSA FILHO, 405 - CAXINGUI",
                Telefone = "(11) 5555-555",
                Turma = "5B",
                Usuario = "ALANA FERREIRA DE OLIVEIRA",
                RF = "1234567",    
                Periodo = "21/02/2021 À 30/03/2021",
            };
        }

        private RelatorioRegistroIndividualAlunoDto GerarAluno()
        {
            var registrosIndividuais = new List<RelatorioRegistroIndividualDetalhamentoDto>();

            var aluno = new RelatorioRegistroIndividualAlunoDto()
            {
                Nome = "ANTONIO CARLOS DOS SANTOS (1234567)",
                Registros = registrosIndividuais,
            };

            registrosIndividuais.Add( new RelatorioRegistroIndividualDetalhamentoDto()
            {
                DataRegistro = DateTime.Now.ToString("dd/MM/yyyy"),
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                RegistradoPor = "IVANA MENDES DE CARVALHO (1234567)",
            });
            registrosIndividuais.Add( new RelatorioRegistroIndividualDetalhamentoDto()
            {
                DataRegistro = DateTime.Now.ToString("dd/MM/yyyy"),
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                RegistradoPor = "IVANA MENDES DE CARVALHO (1234567)",
            });
            registrosIndividuais.Add( new RelatorioRegistroIndividualDetalhamentoDto()
            {
                DataRegistro = DateTime.Now.ToString("dd/MM/yyyy"),
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                RegistradoPor = "IVANA MENDES DE CARVALHO (1234567)",
            });

            return aluno;
        }
    }
}
