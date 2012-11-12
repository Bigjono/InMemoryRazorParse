using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
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
            const string namespaceForDynamicClasses = "InMemory.Razor";
            const string dynamicClassFullName = namespaceForDynamicClasses + "." + dynamicallyGeneratedClassName;

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
                {
                    DefaultBaseClass = typeof (DynamicContentGeneratorBase).FullName,
                    DefaultClassName = dynamicallyGeneratedClassName,
                    DefaultNamespace = namespaceForDynamicClasses,
                };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Text");
            host.NamespaceImports.Add("System.Linq");
            host.NamespaceImports.Add("System.Collections.Generic");

            if (NamespacesImports.Count != 0)
            {
                foreach (var nameSpaceItem in NamespacesImports)
                        host.NamespaceImports.Add(nameSpaceItem);
                
            }

            var engine = new RazorTemplateEngine(host);
            var tr = new StringReader(templateText);

            GeneratorResults razorTemplate = engine.GenerateCode(tr);


            if (razorTemplate.ParserErrors.Any())
            {
                throw new Exception(razorTemplate.ParserErrors.Aggregate("", (current, error) => current + Environment.NewLine + error.Location + ":" + error.Message));
            }


            var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);

            return (DynamicContentGeneratorBase) compiledAssembly.CreateInstance(dynamicClassFullName);
        }


        private Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile)
        {
            var compilerParameters = new CompilerParameters();
            var assemblies = GetLoadedAssemblies();
            foreach (var item in assemblies)
                compilerParameters.ReferencedAssemblies.Add(item);

            if (ReferencedAssemblies.Any())
            {
                foreach (var referencedAssembly in ReferencedAssemblies) 
                    compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
                
            }

            
            compilerParameters.GenerateInMemory = true;
            compilerParameters.CompilerOptions = "/optimize";

            CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters,
                                                                                              unitToCompile);

            if (compilerResults.Errors.Count != 0)
            {
                throw new Exception(GetErrorMessage(compilerResults));
            }

            Assembly compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }



        private string GetErrorMessage(CompilerResults compilerResults)
        {
            var retVal = new StringBuilder();

            for (int x = 0; x < compilerResults.Errors.Count; x++)
            {
                retVal.AppendLine(string.Format("Error #{0}  ({1} , {2}) - {3}",
                            compilerResults.Errors[x].ErrorNumber,
                            compilerResults.Errors[x].Line,
                            compilerResults.Errors[x].Column,
                            compilerResults.Errors[x].ErrorText)
                        );


            }




            return retVal.ToString();
        }


        private IEnumerable<string> GetLoadedAssemblies()
        {

            var retVal = new Collection<string>();
            foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    retVal.Add(assembly.Location);
                }
                catch { }


            }



            return retVal;
        }


    }
}
