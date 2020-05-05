// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.CodeFixes.RemoveNewModifier;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics.RemoveNewModifier
{
    [Trait(Traits.Feature, Traits.Features.CodeActionsRemoveNewModifier)]
    public class RemoveNewModifierCodeFixProviderTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        internal override (DiagnosticAnalyzer, CodeFixProvider) CreateDiagnosticProviderAndFixer(Workspace workspace) =>
            (null, new RemoveNewModifierCodeFixProvider());

        [Theory]
        [InlineData(
            @"public static new void [|Method()|] { }",
            @"public static void [|Method()|] { }")]
        [InlineData(
            "public new int [|Test|];",
            "public int [|Test|];")]
        [InlineData(
            "public new int [|Test|] { get; set; }",
            "public int [|Test|] { get; set; }")]
        [InlineData(
            "public new const int [|test|] = 1;",
            "public const int test = 1;")]
        [InlineData(
            "public new event Action [|Test|];",
            "public event Action Test;")]
        [InlineData(
            "public new int [|this[int p]|] => p;",
            "public int this[int p] => p;")]
        [InlineData(
            "new class [|Test|] { }",
            "class Test { }")]
        [InlineData(
            "new struct [|Test|] { }",
            "struct Test { }")]
        [InlineData(
            "new interface [|Test|] { }",
            "interface Test { }")]
        [InlineData(
            "new delegate [|Test|]()",
            "delegate Test()")]
        [InlineData(
            "new enum [|Test|] { }",
            "enum Test { }")]
        [InlineData(
            "new(int a, int b) [|test|];",
            "(int a, int b) test;")]
        public Task TestRemoveNewModifierFromMembersWithRegularFormatting(string original, string expected)
            => TestRemoveNewModifierCodeFixAsync(original, expected);

        [Theory]
        [InlineData(
            "/* start */ public /* middle */ new /* end */ int [|Test|];",
            "/* start */ public /* middle */ int Test;")]
        [InlineData(
            "/* start */ public /* middle */ new    /* end */ int [|Test|];",
            "/* start */ public /* middle */ int Test;")]
        [InlineData(
            "/* start */ public /* middle */new /* end */ int [|Test|];",
            "/* start */ public /* middle */int Test;")]
        [InlineData(
            "/* start */ public /* middle */ new/* end */ int [|Test|];",
            "/* start */ public /* middle */ int Test;")]
        [InlineData(
            "/* start */ public /* middle */new/* end */ int [|Test|];",
            "/* start */ public /* middle */int Test;")]
        [InlineData(
            "new /* end */ int [|Test|];",
            "int Test;")]
        [InlineData(
            "new     int [|Test|];",
            "int Test;")]
        [InlineData(
            "/* start */ new /* end */ int [|Test|];",
            "/* start */ int [|Test|];")]
        public Task TestRemoveNewFromModifiersWithComplexTrivia(string original, string expected)
            => TestRemoveNewModifierCodeFixAsync(original, expected);

        private Task TestRemoveNewModifierCodeFixAsync(string original, string expected)
        {
            return TestInRegularAndScript1Async(
$@"class App
{{
    {original}
}}",
$@"class App
{{
    {expected}
}}");
        }
    }
}
