#pragma checksum "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d06e3cad26383bce83f49daeb76aa3e4ae9f0332"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\_ViewImports.cshtml"
using Apogee;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\_ViewImports.cshtml"
using Apogee.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d06e3cad26383bce83f49daeb76aa3e4ae9f0332", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a6f659fb9f183bd7cceeb984bde009f85c241a05", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
  
    ViewBag.Title = "Home";

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
 using (Html.BeginForm("GeneralSearch", "Home"))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <div class=""form-horizontal"">
        <div class=""form-group"">
            <div class=""p-1 bg-light shadow-sm mb"">
                <div class=""input-group rounded-pill"">
                    <div class=""input-group-prepend"">
                        <button id=""btn-addon"" type=""submit"" class=""btn btn-link text-warning""><i class=""fa fa-search""></i></button>
                    </div>
                    <input id=""generalSearch"" name=""query"" autocomplete=""off"" type=""search""");
            BeginWriteAttribute("placeholder", " placeholder=\"", 634, "\"", 682, 1);
#nullable restore
#line 16 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
WriteAttributeValue("", 648, Apogee.Resources.Resources.Search, 648, 34, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" aria-describedby=\"btn-addon\" class=\"form-control border-0 bg-light\">\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 21 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"content text-center\">\r\n    <h1>\r\n        ");
#nullable restore
#line 25 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Apogee.Resources.Resources.Welcome);

#line default
#line hidden
#nullable disable
            WriteLiteral("<br />\r\n        Apogée<br />\r\n        by Apside<br />\r\n    </h1>\r\n\r\n    <div class=\"row justify-content-center\">\r\n        ");
#nullable restore
#line 31 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Html.ActionLink(@Apogee.Resources.Resources.NewCollab, "NewCollab", null, new { @class = "btn btn-primary btn-lg font-weight-bold m-3" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 32 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Html.ActionLink(@Apogee.Resources.Resources.SearchCollab, "SearchCollab", null, new { @class = "btn btn-primary btn-lg font-weight-bold m-3" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n    <div class=\"row justify-content-center\">\r\n        ");
#nullable restore
#line 35 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Html.ActionLink(@Apogee.Resources.Resources.CollabMonit, "CollabMonitSearch", null, new { @class = "btn btn-secondary btn-lg font-weight-bold m-3" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 36 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Html.ActionLink(@Apogee.Resources.Resources.SearchCompet, "SearchCompet", null, new { @class = "btn btn-secondary btn-lg font-weight-bold m-3" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n\r\n    <div class=\"row justify-content-center\">\r\n        ");
#nullable restore
#line 40 "C:\Users\formation.DESKTOP-CIFUFI3\source\repos\MigrationCore\Views\Home\Index.cshtml"
   Write(Html.ActionLink("Test création nouveau collaborateur", "AccessDb", null));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n</div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
