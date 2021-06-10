using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IAlunoFotoRepository
    {
        Task<IEnumerable<AlunoFotoArquivoDto>> ObterFotosDoAlunoPorCodigos(string[] codigosAluno);
    }
}
