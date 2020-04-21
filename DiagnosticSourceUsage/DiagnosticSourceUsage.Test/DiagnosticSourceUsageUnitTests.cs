using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DiagnosticSourceUsage.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TestType
    {   
        private static readonly DiagnosticListener Diagnostics = new DiagnosticListener(""Test"");

        public void Foo()
        {
            Diagnostics.Write(""Foo"", new { a = 42 });
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "DiagnosticSourceGuard",
                Message = $"Call to DiagnosticSource.Write should be guarded with IsEnabled",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 17, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixTest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TestType
    {   
        private static readonly DiagnosticListener Diagnostics = new DiagnosticListener(""Test"");

        public void Foo()
        {
            if (Diagnostics.IsEnabled(""Foo""))
                Diagnostics.Write(""Foo"", new { a = 42 });
        }
    }
}";
            VerifyCSharpFix(test, fixTest, allowNewCompilerDiagnostics: true);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DiagnosticSourceUsageCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DiagnosticSourceGuardAnalyzer();
        }
    }
}
