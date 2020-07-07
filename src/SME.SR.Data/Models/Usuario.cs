using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
    public class Usuario
    {
        private const string MENSAGEM_ERRO_USUARIO_SEM_ACESSO = "Usuário sem perfis de acesso.";
        public string CodigoRf { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public Guid PerfilAtual { get; set; }
        public IEnumerable<PrioridadePerfil> Perfis { get; private set; }

        public bool EhPerfilDRE()
        {
            return Perfis.Any(c => c.Tipo == TipoPerfil.DRE && c.CodigoPerfil == PerfilAtual);
        }

        public bool EhPerfilSME()
        {
            return Perfis.Any(c => c.Tipo == TipoPerfil.SME && c.CodigoPerfil == PerfilAtual);
        }

        public bool EhPerfilUE()
        {
            return Perfis.Any(c => c.Tipo == TipoPerfil.UE && c.CodigoPerfil == PerfilAtual);
        }

        public bool EhProfessor()
        {
            return PerfilAtual == Data.Perfis.PERFIL_PROFESSOR;
        }

        public bool EhProfessorCj()
        {
            return PerfilAtual == Data.Perfis.PERFIL_CJ;
        }

        public bool EhProfessorPoa()
        {
            return PerfilAtual == Data.Perfis.PERFIL_POA;
        }

        public TipoPerfil? ObterTipoPerfilAtual()
        {
            return Perfis.FirstOrDefault(a => a.CodigoPerfil == PerfilAtual).Tipo;
        }

        public bool PossuiPerfilCJ()
            => Perfis != null &&
                Perfis.Any(c => c.CodigoPerfil == Data.Perfis.PERFIL_CJ);

        public bool PossuiPerfilProfessor()
           => Perfis != null &&
               Perfis.Any(c => c.CodigoPerfil == Data.Perfis.PERFIL_PROFESSOR);

        public bool PossuiPerfilDre()
        {
            return Perfis != null && Perfis.Any(c => c.Tipo == TipoPerfil.DRE);
        }

        public bool PossuiPerfilDreOuUe()
        {
            if (Perfis == null || !Perfis.Any())
            {
                throw new NegocioException(MENSAGEM_ERRO_USUARIO_SEM_ACESSO);
            }
            return PossuiPerfilDre() || PossuiPerfilUe();
        }

        public bool PossuiPerfilCJPrioritario()
        {
            return Perfis != null && PossuiPerfilCJ() && PossuiPerfilProfessor() &&
                   !Perfis.Any(c => c.CodigoPerfil == Data.Perfis.PERFIL_DIRETOR ||
                                                     c.CodigoPerfil == Data.Perfis.PERFIL_CP ||
                                                     c.CodigoPerfil == Data.Perfis.PERFIL_AD);
        }

        public bool PossuiPerfilSme()
        {
            return Perfis != null && Perfis.Any(c => c.Tipo == TipoPerfil.SME);
        }

        public bool PossuiPerfilSmeOuDre()
        {
            if (Perfis == null || !Perfis.Any())
            {
                throw new NegocioException(MENSAGEM_ERRO_USUARIO_SEM_ACESSO);
            }
            return PossuiPerfilSme() || PossuiPerfilDre();
        }

        public bool PossuiPerfilUe()
        {
            return Perfis != null && Perfis.Any(c => c.Tipo == TipoPerfil.UE);
        }

        public bool TemPerfilGestaoUes()
        {
            return (PerfilAtual == Data.Perfis.PERFIL_DIRETOR || PerfilAtual == Data.Perfis.PERFIL_AD ||
                    PerfilAtual == Data.Perfis.PERFIL_SECRETARIO || PerfilAtual == Data.Perfis.PERFIL_CP ||
                    EhPerfilSME() || EhPerfilDRE());
        }

        public bool TemPerfilSupervisorOuDiretor()
        {
            return (PerfilAtual == Data.Perfis.PERFIL_DIRETOR || PerfilAtual == Data.Perfis.PERFIL_SUPERVISOR);
        }
    }
}
