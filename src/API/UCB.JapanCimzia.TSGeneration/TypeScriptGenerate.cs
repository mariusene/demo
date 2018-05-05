using Microsoft.Build.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Http;
using TypeLite;
using UCB.JapanCimzia.API.Messages;

namespace UCB.JapanCimzia.TSGeneration
{
    public class TypeScriptGenerate : Task
    {
        private static readonly Assembly APIAssembly = typeof(OrderMessage).Assembly;
        private static readonly string MessagesNamespace = "UCB.JapanCimzia.API.Messages";
        private const string ModelNamespace = "api";
        private const string CorePath = @"src\app\core";

        public string WebUiDir { get; set; }

        public override bool Execute()
        {
            Console.WriteLine("START: Generating TS models and services...");
            try
            {
                var modelGenerator = GenerateModels();
                GenerateServices(modelGenerator);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("FINISH: Generating TS models and services...");
            return true;
        }

        private void GenerateServices(TsGenerator modelGenerator)
        {
            var servicesGenerator = new TsServiceGenerator(modelGenerator);
            var controllers =
                APIAssembly.GetTypes()
                    .Where(t => typeof(ApiController).IsAssignableFrom(t));
            foreach (var controller in controllers)
            {
                var serviceCode = servicesGenerator.Generate(controller);
                var serviceFileName = $"{TsServiceGenerator.GetServiceFileName(controller)}.ts";
                SaveToFile(serviceCode.ToString(), serviceFileName);
            }
        }

        private TsGenerator GenerateModels()
        {
            var tsModels = APIAssembly.GetTypes().Where(t => t.Namespace == MessagesNamespace || t.GetCustomAttributes().Any(x => x is TsClassAttribute || x is TsEnumAttribute))
                .OrderBy(t => t.FullName);

            var ts =
                TypeScript.Definitions()
                    .WithModuleNameFormatter(tsmodule => ModelNamespace)
                    .WithMemberFormatter(
                        identifier =>
                            char.ToLower(identifier.Name[0], CultureInfo.InvariantCulture)
                                + identifier.Name.Substring(1))
                    .AsConstEnums(false)
                    .WithIndentation("    "); // 4 spaces

            foreach (var tsModel in tsModels)
            {
                ts.For(tsModel);
            }
            
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var enums = ts.Generate(TsGeneratorOutput.Enums | TsGeneratorOutput.Constants);
                SaveToFile(enums, $"{ModelNamespace}.enums.ts");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }

            var rawInterfaces = ts.Generate(TsGeneratorOutput.Properties);
            var interfaces = EnableIndexingOnKeyValuePairs(rawInterfaces);
            SaveToFile(interfaces, $"{ModelNamespace}.models.d.ts");

            return ts.ScriptGenerator;
        }

        private static string EnableIndexingOnKeyValuePairs(string rawInterfaces)
        {
            var interfaces = Regex.Replace(rawInterfaces, @":\s*ucb\.KeyValuePair\<(?<k>[^\,]+),(?<v>[^\,]+)\>\[\];",
                m => ": {[key: " + m.Groups["k"].Value + "]: " + m.Groups["v"].Value + "};", RegexOptions.Multiline);
            return interfaces;
        }

        private void SaveToFile(string data, string fileName)
        {
            var filePath = Path.Combine(WebUiDir, CorePath, fileName);
            using (var file = File.CreateText(filePath))
            {
                file.Write(data.Replace("export var", "export const"));
            }
            Console.WriteLine($"Writing:{filePath}");
        }
    }
}
