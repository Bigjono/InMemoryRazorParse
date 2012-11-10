using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Web.Razor;
using Microsoft.CSharp;

//  inspiration for this came from here --< http://fabiomaulo.blogspot.co.uk/2011/08/parse-string-as-razor-template.html


namespace InMemoryRazorParser.Core
{
    public class RazorParser
    {
        public dynamic Model { get; set; }
        public Collection<string> NamespacesImports { get; set; }
        public Collection<string> ReferencedAssemblies { get; set; }


        public RazorParser()
        {
            Model = new ExpandoObject();
            NamespacesImports = new Collection<string>();
            ReferencedAssemblies = new Collection<string>();
        }


        public string Parse(string template, dynamic model)
        {
            Model = model;
            return Parse(template);
        }

        public string Parse(string template)
        {

            var templateInstance = CreateRazorInstance(template);

            if (templateInstance == null) return "";

            templateInstance.Model = Model;

            return templateInstance.GetContent();

        }


        private DynamicContentGeneratorBase CreateRazorInstance(string templateText)
        {
            const string dynamicallyGeneratedClassName = "DynamicContentTemplate";
            const string namespaceForDynamicClasses = "Watchfinder.InMemory.Razor";
            const string dynamicClassFullName = namespaceForDynamicClasses + "." + dynamicallyGeneratedClassName;

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
            {
                DefaultBaseClass = typeof(DynamicContentGeneratorBase).FullName,
                DefaultClassName = dynamicallyGeneratedClassName,
                DefaultNamespace = namespaceForDynamicClasses,
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Text");

            if (NamespacesImports.Count != 0)
            {
                foreach (var nameSpaceItem in NamespacesImports)
                {
                    host.NamespaceImports.Add(nameSpaceItem);
                }
            }

            var engine = new RazorTemplateEngine(host);
            var tr = new StringReader(templateText);


            GeneratorResults razorTemplate = engine.GenerateCode(tr);

            var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);

            return (DynamicContentGeneratorBase)compiledAssembly.CreateInstance(dynamicClassFullName);
        }


        private Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile)
        {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");

            if (ReferencedAssemblies.Count != 0)
            {
                foreach (var referencedAssembly in ReferencedAssemblies)
                {
                    compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
                }
            }

            compilerParameters.ReferencedAssemblies.Add(typeof(DynamicContentGeneratorBase).Assembly.Location);
            compilerParameters.GenerateInMemory = true;

            CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            Assembly compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }


    }
}
