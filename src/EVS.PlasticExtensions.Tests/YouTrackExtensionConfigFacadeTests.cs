using Codice.Client.IssueTracker;
using EVS.PlasticExtensions.YouTrackPlugin.Core.Models;
using EVS.PlasticExtensions.YouTrackPlugin.Core.Services.Impl;
using NUnit.Framework;

namespace EVS.PlasticExtensions.Tests
{
    [TestFixture]
    public class YouTrackExtensionConfigFacadeTests
    {
        [Test]
        public void ExplicitlyEmptyBranchPrefix_IsPreserved()
        {
            var config = new IssueTrackerConfiguration
            {
                Parameters = new[]
                {
                    new IssueTrackerConfigurationParameter
                    {
                        Name = ConfigParameterNames.BranchPrefix,
                        Value = string.Empty,
                        Type = IssueTrackerConfigurationParameterType.BranchPrefix,
                        IsGlobal = true
                    }
                }
            };

            var facade = new YouTrackExtensionConfigFacade(config);

            Assert.That(facade.BranchPrefix, Is.Empty);
        }

        [Test]
        public void EmptyWebGuiUrl_IsOptionalAndReturnedEmptyForSettings()
        {
            var config = new IssueTrackerConfiguration
            {
                Parameters = new[]
                {
                    new IssueTrackerConfigurationParameter
                    {
                        Name = ConfigParameterNames.WebGuiRootUrl,
                        Value = string.Empty,
                        Type = IssueTrackerConfigurationParameterType.Text,
                        IsGlobal = true
                    }
                }
            };

            var facade = new YouTrackExtensionConfigFacade(config);
            var webGuiParameter = facade.GetYouTrackParameters()
                .Find(parameter => parameter.Name == ConfigParameterNames.WebGuiRootUrl);

            Assert.That(facade.WebGuiRootUrl, Is.Null);
            Assert.That(webGuiParameter.Value, Is.Empty);
        }
    }
}
