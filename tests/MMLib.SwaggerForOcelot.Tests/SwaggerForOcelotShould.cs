﻿using JsonDiffPatchDotNet;
using MMLib.SwaggerForOcelot.Transformation;
using Newtonsoft.Json.Linq;
using System;
using Xunit;
using Xunit.Sdk;

namespace MMLib.SwaggerForOcelot.Tests
{
    /// <summary>
    /// Generic tests for transformation downstream swagger to upstream format. 
    /// Source test cases are located as resources in '/tests' folder and providing by <see cref="TestCasesProvider" />.
    /// </summary>
    public class SwaggerForOcelotShould
    {
        [Theory]
        [ClassData(typeof(TestCasesProvider))]
        public void TransferDownstreamSwaggerToUpstreamFormat(TestCase testData)
        {
            var transformer = new SwaggerJsonTransformer();

            string transformed = transformer.Transform(
                testData.DownstreamSwagger.ToString(),
                testData.Routes,
                testData.HostOverride);

            AreEqual(transformed, testData.ExpectedTransformedSwagger, testData.FileName);
        }

        private static void AreEqual(string transformed, JObject expected, string filename)
        {
            var actual = JObject.Parse(transformed);

            if (!JObject.DeepEquals(actual, expected))
            {
                var jdp = new JsonDiffPatch();
                JToken patch = jdp.Diff(actual, expected);

                throw new XunitException(
                    $"Transformed upstream swagger is not equal to expected.{Environment.NewLine}File: {filename}. {Environment.NewLine}Diff: {patch}");
            }
        }
    }
}
