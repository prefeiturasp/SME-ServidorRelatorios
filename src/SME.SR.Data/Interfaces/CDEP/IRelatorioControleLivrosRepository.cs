using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRelatorioControleLivrosRepository
    {
        Task<IEnumerable<AcervoSolicitacaoDto>> ObterRelatorioControleLivrosEmpresados(long[] tiposAcervosPermitidos, string solicitante, string tombo, List<SituacaoEmprestimo> situacaoEmprestimo, bool? somenteDevolvidos);
        Task<IEnumerable<ControleAcervoDTO>> ObterRelatorioControleAcervos(long[] tiposAcervosPermitidos, TipoAcervo? tipoAcervo, SituacaoAcervo? situacaoAcervo);
        Task<IEnumerable<ControleEditoraDTO>> ObterRelatorioControleEditoras(List<int>? idEditoras);
        Task<IEnumerable<ControleAcervoAutorDTO>> ObterRelatorioControleAcervosAutor(long[] tiposAcervosPermitidos, TipoAcervo? tipoAcervo, List<int> autores);
        Task<IEnumerable<AcervoDevolucaoDto>> ObterRelatorioControleDevolucaoLivros(long[] tiposAcervosPermitidos, string solicitante, bool? somenteAtrasados = false);
    }
}