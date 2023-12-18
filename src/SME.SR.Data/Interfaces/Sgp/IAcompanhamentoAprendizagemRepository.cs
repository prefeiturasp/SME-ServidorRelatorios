using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAcompanhamentoAprendizagemRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemAlunoDto>> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre);
        Task<UltimoSemestreAcompanhamentoGeradoDto> ObterUltimoSemestreAcompanhamentoGerado(string alunoCodigo);
        Task<IEnumerable<AcompanhamentoTurmaAlunoImagemBase64Dto>> ObterInformacoesAcompanhamentoComImagemBase64TurmaAlunos(long turmaId,  int semestre, string alunoCodigo, params string[] tagsImagensConsideradas);
    }
}