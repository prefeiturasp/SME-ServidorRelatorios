using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SME.SR.Infra
{
    public static class CriptografiaExtension
    {
        private static readonly byte[] tdesKey = new byte[] { 107, 8, 82, 60, 113, 135, 190, 128, 188, 51, 238, 120, 59, 135, 57, 140, 107, 8, 82, 60, 113, 135, 190, 128 };
        private static readonly byte[] tdesIV = new byte[] { 113, 135, 190, 128, 186, 217, 34, 47 };

        public static string CriptografarSenha(this string senha, TipoCriptografia tipo)
        {
            switch (tipo)
            {
                case TipoCriptografia.TripleDES:
                    return CriptografarSenhaTripleDES(senha);
                case TipoCriptografia.MD5:
                    throw new NotImplementedException();
                case TipoCriptografia.SHA512:
                    return CriptografarSenhaSHA512(senha);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string CriptografarSenhaTripleDES(string senha)
        {
            byte[] plainByte = ASCIIEncoding.ASCII.GetBytes(senha);
            MemoryStream ms = new MemoryStream();
            SymmetricAlgorithm sym = TripleDES.Create();
            CryptoStream encStream = new CryptoStream(ms, sym.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
            encStream.Write(plainByte, 0, plainByte.Length);
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            return Convert.ToBase64String(cryptoByte);
        }

        private static string CriptografarSenhaSHA512(string senha)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(Encoding.Unicode.GetBytes(senha))).TrimStart('/');
            }
        }

    }
}
