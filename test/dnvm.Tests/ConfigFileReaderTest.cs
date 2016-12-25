using System.IO;
using DotNet.Files;
using FluentAssertions;
using Xunit;

namespace DotNet.Test
{
    public class ConfigFileReaderTest
    {
        [Fact]
        public void MultipleFx()
        {
            var file = Read(@"
env: myenvname
fx:
 - 1.0.1
 - 1.2.3
cli: 1.0.0-preview3-003131 ");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should()
                .HaveCount(2)
                .And.Contain(new[] { "1.0.1", "1.2.3" });
            file.Cli.Should().Equals("1.0.0-preview3-003131");
        }

        [Fact]
        public void SingleFx()
        {
            var file = Read(@"
env: myenvname
fx: 1.1.0");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should().ContainSingle("1.1.0");
        }

        [Fact]
        public void Comments()
        {
            var file = Read(@"
# comments
env: myenvname
# more comments

fx: 1.1.0 # trailing comments

#end comments");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should().ContainSingle("1.1.0");
        }


        private ConfigFile Read(string doc)
        {
            var reader = new StringReader(doc);
            return new ConfigFileYamlReader().Read(reader);
        }
    }
}