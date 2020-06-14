namespace SME.SR.Data
{
    public class PermissaoConsultas
    {
        internal static string ObterGrupos = @"SELECT g.id as Grupo, guidperfil as GrupoID, 
                            cdtipofuncaoatividade as TipoFuncaoAtividade, 
                            idabrangencia as Abrangencia, 
                            ehperfilmanual, 
                            cargo
                    FROM public.grupos g
                    LEFT JOIN public.grupocargos gc on g.id = gc.idgrupo ";

    }
}
