using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.Management;

namespace TongJi.Drawing
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
    }

    public class LicenseBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return typeof(License);
        }
    }

    [Serializable]
    public class License
    {
        public LicenseState State;
        public string RequestCode;
        public string Key;
        public DateTime Expire;
        public int Times;

        public License()
        {
            State = LicenseState.Trial;
            Expire = DateTime.Now.AddDays(15);
            Times = 15;
        }
    }

    public class LicenseManager
    {
        public string LicenseFile { get; private set; }
        public License License { get; private set; }

        public LicenseManager(string licenseFile)
        {
            LicenseFile = licenseFile;
            License = GetLicense(licenseFile);
            UpdateLicenseState();
        }

        public bool CanRun()
        {
            if (License.State == LicenseState.Licensed)
            {
                return true;
            }
            else if (License.State == LicenseState.Trial)
            {
                return !IsTrialExpired();
            }
            else
            {
                return false;
            }
        }

        public bool IsTrialExpired()
        {
            if (DateTime.Now < License.Expire && License.Times != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool TryActivate(string requestCode, string keyCode)
        {
            if (Security.Validate(requestCode, keyCode))
            {
                License.RequestCode = requestCode;
                License.Key = keyCode;
                License.State = LicenseState.Licensed;
                SaveLicense(LicenseFile, License);
                return true;
            }
            return false;
        }

        public void StartTrial()
        {
            SaveLicense(LicenseFile, License);
        }

        private static License GetLicense(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                try
                {
                    return Dreambuild.IO.Serialization.XmlLoad<License>(fileName);
                }
                catch (Exception e)
                {
                    return new License();
                }
            }
            else
            {
                return new License();
            }
        }

        private static void SaveLicense(string fileName, License licenseData)
        {
            Dreambuild.IO.Serialization.XmlSave(licenseData, fileName);
        }

        private void UpdateLicenseState()
        {
            if (string.IsNullOrEmpty(License.Key))
            {
                License.State = LicenseState.Trial;
                License.Times = License.Times > 0 ? License.Times - 1 : License.Times;
            }
            else if (License.RequestCode != Security.GetRequestCode())
            {
                License.State = LicenseState.Illegal;
            }
            else
            {
                if (Security.Validate(License.RequestCode, License.Key))
                {
                    License.State = LicenseState.Licensed;
                }
                else
                {
                    License.State = LicenseState.Illegal;
                }
            }
        }
    }

    [Serializable]
    public enum LicenseState
    {
        Unknown,
        Trial,
        Licensed,
        Illegal
    }
}
