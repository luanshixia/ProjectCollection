using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Security.Cryptography;

namespace Keygen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string prikey = "<RSAKeyValue><Modulus>qSVQf2HNnkOT2eZkkHTLsVdI6oF4+Z7qdccySfK4IAJeU6lvzUCq6YfWPfgfk2CyonWkfmtQUp9x+YghO84qg9r45lh/tFfldMrHY6f9Q7QZKLn2JHDORCPREUPVTkwTaww2dP4163EdpeRSRBbnfSiGADxpg9xxhdgbT0LXUjE=</Modulus><Exponent>AQAB</Exponent><P>6AFxUDQs/Mt1JPMb5P9hoQskDqYCevEQ1sCCojUeTV2PrW0Yl4AI+ncYxadiMUZnwFdRcxEnjc7uZuLNYih/TQ==</P><Q>uqOZprmPuoz77NVEFcMJ/1rPjZQ0m2XMWOJrS/PAu/NGqCOhQCGUbRijTjMV2fZf7wJ+e389cWanfmvnTJ+0dQ==</Q><DP>QVMuqauXQzKycE33e7ogriyp4WoC5sT7vcwEvFGCj7wZWXaPZxEI9iUUl4qyzjcJvZGYgHLDFV2/qL6Rn+LRPQ==</DP><DQ>NIozieS42kov7SbIKNwj51eYEAIFoS1SDj+G9vWibwZ4AIMvNI9/agrChhQJdbdOoEjydC+Ii3Dbe9JCZaabtQ==</DQ><InverseQ>wbx+tIdBGxjB0lPO5/pW6SVXL9t964hAXR9iX+a+0R+Ov6qwww6zvujmL0A8zuUE3H/8ziApLmTtnIeNza3Xnw==</InverseQ><D>QVf+/xbjfm6Q4a8VzpCwWDRmIOIfwB9aaGGp1dOEuijwE9XzMRH4dPtbEtmWKTNED2rvPz9p6mFagJHMdPQyi9gC3U+WOlm14GhJbDwx7WO+6ZgxD4LRed5/LHtegCUEUrs0z1YaGQOr5VX50cP4EN9c6NRKOpgxHYNo85abOKE=</D></RSAKeyValue>";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            txtKey.Text = GetKey(txtRequestCode.Text);
        }

        private static string GetKey(string requestCode)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(prikey);
                // 加密对象 
                RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsa);
                f.SetHashAlgorithm("SHA1");
                byte[] source = System.Text.ASCIIEncoding.ASCII.GetBytes(requestCode);
                SHA1Managed sha = new SHA1Managed();
                byte[] result = sha.ComputeHash(source);
                byte[] b = f.CreateSignature(result);
                return Convert.ToBase64String(b);
            }
        }
    }
}
