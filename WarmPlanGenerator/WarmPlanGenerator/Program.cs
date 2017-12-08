using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WarmPlanGenerator
{
    class Program
    {
        static readonly Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "wait", "KW" },
            { "sleep24h", "KW" },
            { "sleep2h", "KW" },
            { "brazilus", "CQ-SN" },
            { "centraluseuap", "DM-EUAP" },
            { "eastus2euap", "BN-EUAP" },
            { "westcentralus", "CY" },
            { "northcentralus", "CH" },
            { "westus", "BY" },
            { "westeurope", "AM" },
            { "eastus", "BL" },
            { "southcentralus", "SN" },
            { "eastasia", "HK" },
            { "uksouth", "LN" },
            { "westindia", "BM" },
            { "uknorth", "MM" },
            { "canadacentral", "YT" },
            { "brazilsouth", "CQ" },
            { "japanwest", "OS" },
            { "australiasoutheast", "ML" },
            { "koreacentral", "SE" },
            { "northeurope", "DB" },
            { "eastus2", "BN" },
            { "centralus", "DM" },
            { "southeastasia", "SG" },
            { "ukwest", "CW" },
            { "centralindia", "PN" },
            { "southindia", "MA" },
            { "uksouth2", "LO" },
            { "canadaeast", "YQ" },
            { "westus2", "MWH" },
            { "japaneast", "KW" },
            { "australiaeast", "SY" },
            { "koreasouth", "PS" },
        };

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("Usage: WarmPlanGenerator.exe inputFileName");
                return;
            }

            var inputFileName = args[0];
            if (!File.Exists(inputFileName))
            {
                Console.WriteLine("Input file not found.");
                return;
            }

            var inputLines = TryCatch(() => File.ReadAllLines(inputFileName), ex => null);
            if (inputLines == null)
            {
                Console.WriteLine("Input file not valid.");
                return;
            }

            var output = GetOutput(inputLines);
            output.Save(Console.Out);
        }

        static T TryCatch<T>(Func<T> tryFunc, Func<Exception, T> catchFunc)
        {
            try
            {
                return tryFunc();
            }
            catch (Exception ex)
            {
                return catchFunc(ex);
            }
        }

        static XDocument GetOutput(string[] inputLines)
        {
            var template = @"<Release>
              <Component>[component]</Component>
              <ReleaseType>[releasetype]</ReleaseType>
              <JobType>Deploy Only</JobType>
              <DeploymentType>Component</DeploymentType>
              <Notes>WARMsubmit of [product]</Notes>
              <Build>
                <Branch>[branch]</Branch>
                <BuildNumber>[buildnumber]</BuildNumber>
                <BuildPath>[buildshare]</BuildPath>
              </Build>
              <Deployments>
                <Deployment>
                  <Environment>Production</Environment>
                  <DeploymentTasks>
                  </DeploymentTasks>
                </Deployment>
              </Deployments>
            </Release>";

            var xd = XDocument.Parse(template);
            var tasks = xd.Descendants("DeploymentTasks").Single();

            foreach (var line in inputLines)
            {
                var parts = line.Split(',');
                tasks.Add(GetTask(parts[0], parts[1]));
            }

            return xd;
        }

        static XElement GetTask(string batch, string region)
        {
            return new XElement("DeploymentTask",
                new XElement("BatchNumber", batch),
                new XElement("Endpoint", "[deendpoint]"),
                new XElement("SettingsFile", "Settings_AUX_Prod_[product].xml"),
                new XElement("Template", GetTemplate(region)),
                new XElement("ScaleUnit", GetScaleUnit(region)),
                new XElement("Toolset", @"usesecurity;usegit_engsys_mda_mds_release;ext_auxcsmtools=[buildshare]\retail-amd64\RDTools"),
                new XElement("InitialState", "Run"));
        }

        static string GetTemplate(string region)
        {
            if (region.ToLower().Contains("wait"))
            {
                return "AUX_WaitForPacificOfficeHours.xml";
            }
            else if (region.ToLower().Contains("sleep24h"))
            {
                return "AUX_Sleep_24h.xml";
            }
            else if (region.ToLower().Contains("sleep2h"))
            {
                return "AUX_Sleep_2h.xml";
            }

            return "AUX_UpgradeDeploy.xml";
        }

        static string GetScaleUnit(string region)
        {
            return $"[scaleunitproduct]-PROD-{dict[region.ToLower()]}-01";
        }
    }
}
