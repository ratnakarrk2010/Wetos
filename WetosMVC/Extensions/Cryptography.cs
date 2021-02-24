using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
//using System.Web.Security;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using WETOS.Util;

namespace WetosMVCMainApp.Extensions
{
    public class Cryptography
    {

        public string EncryptPassword(string strPassword)
        {
            try
            {
                string plainText, passPhrase, saltValue, AlgorithmName, initVector;
                int keySize, passwordIterations;
                plainText = strPassword;
                passPhrase = "Pas5pr@se";         // 'can be any string
                saltValue = "s@1tValue";          // ' can be any string
                AlgorithmName = "SHA1";           // ' can be "MD5"
                passwordIterations = 2;           // ' can be any number
                initVector = "@1B2c3D4e5F6g7H8";  // ' must be 16 bytes @1B2c3D4e5F6g7H8
                keySize = 256;                    // ' can be 192 or 128 pehle 256

                plainText = WETOS.Util.Cryptography.EncryptPassword(plainText, passPhrase, saltValue, AlgorithmName, passwordIterations, initVector, keySize);

                return plainText;
            }
            catch
            {

                throw;
            }
        }

        public string DecryptPassword(string strPassword)
        {
            try
            {
                string plainText, passPhrase, saltValue, AlgorithmName, initVector;
                int keySize, passwordIterations;
                plainText = strPassword;
                passPhrase = "Pas5pr@se";         // 'can be any string
                saltValue = "s@1tValue";          // ' can be any string
                AlgorithmName = "SHA1";           // ' can be "MD5"
                passwordIterations = 2;           // ' can be any number
                initVector = "@1B2c3D4e5F6g7H8";  // ' must be 16 bytes
                keySize = 256;                    // ' can be 192 or 128

                plainText = WETOS.Util.Cryptography.DecryptPassword(plainText, passPhrase, saltValue, AlgorithmName, passwordIterations, initVector, keySize);


                return plainText;
            }
            catch
            {


                throw;
            }
        }



    }
}
