using Genny;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpsCoolWeb.Web.Templates
{
    [GennyModuleDescriptor("Default system module template")]
    public class Module : GennyModule
    {
        [GennyParameter(0, Required = true)]
        public String Model { get; set; }

        [GennyParameter(1, Required = true)]
        public String Controller { get; set; }

        [GennyParameter(2, Required = false)]
        public String Area { get; set; }

        public Module(IServiceProvider services)
            : base(services)
        {
        }

        public override void Run()
        {
            String path = (Area != null ? Area + "/" : "") + Controller;
            Dictionary<String, GennyScaffoldingResult> results = new Dictionary<String, GennyScaffoldingResult>();

            results.Add($"../UpsCoolWeb.Resources/Resources/Views/{path}/{Model}View.json", Scaffold("Resources/View"));

            results.Add($"../UpsCoolWeb.Controllers/{path}/{Controller}Controller.cs", Scaffold("Controllers/Controller"));
            results.Add($"../../test/UpsCoolWeb.Tests/Unit/Controllers/{path}/{Controller}ControllerTests.cs", Scaffold("Tests/ControllerTests"));

            results.Add($"../UpsCoolWeb.Objects/Models/{path}/{Model}.cs", Scaffold("Objects/Model"));
            results.Add($"../UpsCoolWeb.Objects/Views/{path}/{Model}View.cs", Scaffold("Objects/View"));

            results.Add($"../UpsCoolWeb.Services/{path}/{Model}Service.cs", Scaffold("Services/Service"));
            results.Add($"../UpsCoolWeb.Services/{path}/I{Model}Service.cs", Scaffold("Services/IService"));
            results.Add($"../../test/UpsCoolWeb.Tests/Unit/Services/{path}/{Model}ServiceTests.cs", Scaffold("Tests/ServiceTests"));

            results.Add($"../UpsCoolWeb.Validators/{path}/{Model}Validator.cs", Scaffold("Validators/Validator"));
            results.Add($"../UpsCoolWeb.Validators/{path}/I{Model}Validator.cs", Scaffold("Validators/IValidator"));
            results.Add($"../../test/UpsCoolWeb.Tests/Unit/Validators/{path}/{Model}ValidatorTests.cs", Scaffold("Tests/ValidatorTests"));

            results.Add($"../UpsCoolWeb.Web/Views/{path}/Index.cshtml", Scaffold("Web/Index"));
            results.Add($"../UpsCoolWeb.Web/Views/{path}/Create.cshtml", Scaffold("Web/Create"));
            results.Add($"../UpsCoolWeb.Web/Views/{path}/Details.cshtml", Scaffold("Web/Details"));
            results.Add($"../UpsCoolWeb.Web/Views/{path}/Edit.cshtml", Scaffold("Web/Edit"));
            results.Add($"../UpsCoolWeb.Web/Views/{path}/Delete.cshtml", Scaffold("Web/Delete"));

            if (results.Any(result => result.Value.Errors.Any()))
            {
                Dictionary<String, GennyScaffoldingResult> errors = new Dictionary<String, GennyScaffoldingResult>(results.Where(x => x.Value.Errors.Any()));

                Write(errors);

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolding failed! Rolling back...", ConsoleColor.Red);
            }
            else
            {
                Logger.WriteLine("");

                TryWrite(results);

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolded successfully!", ConsoleColor.Green);
            }
        }

        public override void ShowHelp()
        {
            Logger.WriteLine("Parameters:");
            Logger.WriteLine("    1 - Scaffolded model.");
            Logger.WriteLine("    2 - Scaffolded controller.");
            Logger.WriteLine("    3 - Scaffolded area (optional).");
        }

        private GennyScaffoldingResult Scaffold(String path)
        {
            return Scaffolder.Scaffold("Templates/Module/" + path, new ModuleModel(Model, Controller, Area));
        }
    }
}
