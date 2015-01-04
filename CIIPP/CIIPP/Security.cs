using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.Management;

using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace CIIPP
{
    public static class Security
    {
        private static string pubkey = "<RSAKeyValue><Modulus>qSVQf2HNnkOT2eZkkHTLsVdI6oF4+Z7qdccySfK4IAJeU6lvzUCq6YfWPfgfk2CyonWkfmtQUp9x+YghO84qg9r45lh/tFfldMrHY6f9Q7QZKLn2JHDORCPREUPVTkwTaww2dP4163EdpeRSRBbnfSiGADxpg9xxhdgbT0LXUjE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static bool Validate(string requestCode, string keyCode)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                bool success = false;
                try
                {
                    rsa.FromXmlString(pubkey);
                    RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsa);
                    f.SetHashAlgorithm("SHA1");

                    byte[] key = Convert.FromBase64String(keyCode);
                    SHA1Managed sha = new SHA1Managed();
                    byte[] name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(requestCode));
                    success = f.VerifySignature(name, key);
                }
                catch
                {
                }
                return success;
            } 
        }

        public static string GetRequestCode()
        {
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            string strID = null;
            foreach (ManagementObject mo in moc)
            {
                strID = mo.Properties["ProcessorId"].Value.ToString();
                break;
            }
            return strID;
        }

        public static LicenseState CheckLicense(string licenseFile)
        {
            if (!System.IO.File.Exists(licenseFile))
            {
                return LicenseState.Trial;
            }
            else
            {
                License license = GetLicense(licenseFile);
                if (license.RequestCode != GetRequestCode())
                {
                    return LicenseState.Illegal;
                }
                else
                {
                    if (Validate(license.RequestCode, license.Key))
                    {
                        return LicenseState.Licensed;
                    }
                    else
                    {
                        return LicenseState.Illegal;
                    }
                }
            }
        }

        private static License GetLicense(string licenseFile)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.Stream fs = System.IO.File.OpenRead(licenseFile))
            {
                try
                {
                    License license = bf.Deserialize(fs) as License;
                    return license;
                }
                catch
                {
                    return new License();
                }
            }       
        }

        public static void SaveLicense(string licenseFile, License licenseData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(licenseFile, System.IO.FileMode.Create,
                System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                bf.Serialize(fs, licenseData);
            }
        }
    }

    [Serializable]
    public class License
    {
        public string RequestCode;
        public string Key;
    }

    public enum LicenseState
    {
        Unknown,
        Trial,
        Licensed,
        Illegal
    }
}
