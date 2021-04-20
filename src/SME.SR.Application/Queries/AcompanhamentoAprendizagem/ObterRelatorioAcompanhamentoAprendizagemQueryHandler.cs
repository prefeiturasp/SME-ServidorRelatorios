using MediatR;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoAprendizagemQuery, RelatorioAcompanhamentoAprendizagemDto>
    {
        public Task<RelatorioAcompanhamentoAprendizagemDto> Handle(ObterRelatorioAcompanhamentoAprendizagemQuery request, CancellationToken cancellationToken)
        {
            var retorno = new RelatorioAcompanhamentoAprendizagemDto();
            retorno.Cabecalho = GerarCabecalho();
            retorno.Alunos.Add(GerarAluno());

            return Task.FromResult(retorno);
        }

        private RelatorioAcompanhamentoAprendizagemAlunoDto GerarAluno()
        {
            var aluno = new RelatorioAcompanhamentoAprendizagemAlunoDto();
            aluno.CodigoEol = "4241513";
            aluno.DataNascimento = "02/02/2012";
            aluno.Nome = "Nº1 - ALANA FERREIRA DE OLIVEIRA";
            aluno.Responsavel = "JONATHAN DA SILVA PEREIRA (FILIAÇÃO1)";
            aluno.Telefone = "(11) 94596-3666 (Atualizado - 26/06/2018)";

            aluno.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto()
            {
                Caminho = "https://media.gazetadopovo.com.br/viver-bem/2017/03/criancadocumento-600x401-ce1bce00.jpg"
            });

            aluno.Frequencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto()
            {
                Aulas = 50,
                Ausencias = 5,
                Frequencia = 90,
                Bimestre = "1º"
            });

            aluno.Frequencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto()
            {
                Aulas = 50,
                Ausencias = 10,
                Frequencia = 80,
                Bimestre = "2º"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Observacoes = "O cuidado em identificar pontos críticos no novo modelo estrutural aqui preconizado nos obriga à análise do sistema de formação de quadros que corresponde às necessidades.";

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "Desta maneira, o desenvolvimento contínuo de distintas formas de atuação garante a contribuição de um grupo importante na determinação das direções preferenciais no sentido do progresso."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                 Descricao = "O cuidado em identificar pontos críticos na necessidade de renovação processual ainda não demonstrou convincentemente que vai participar na mudança das condições financeiras e administrativas exigidas."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "Todas estas questões, devidamente ponderadas, levantam dúvidas sobre se o novo modelo estrutural aqui preconizado desafia a capacidade de equalização dos procedimentos normalmente adotados."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "O cuidado em identificar pontos críticos na necessidade de renovação processual ainda não demonstrou convincentemente que vai participar na mudança das condições financeiras e administrativas exigidas."
            });

            aluno.RegistroPercursoTurma = "Gostaria de enfatizar que o surgimento do comércio virtual apresenta tendências no sentido de aprovar a manutenção das condições financeiras e administrativas exigidas.";

            return aluno;
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto GerarCabecalho()
        {
            return new RelatorioAcompanhamentoAprendizagemCabecalhoDto() { Dre = "BUTANTA", Ue = "EMEI ANTONIO BENTO", Turma = "5B", Professores = "FRANCISCA DA SILTA MATA, JESSICA DE OLIVEIRA" };
        }
    }
}
