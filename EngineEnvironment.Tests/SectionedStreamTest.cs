using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Serialization;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Data
{
    [TestFixture]
    class SectionedStreamTest
    {
        [Test]
        public void TestEscaping()
        {
            var reader = new SectionedStreamReader(new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes("Ello"))));
            Assert.False( reader.isStartSection(SectionedStreamWriter.Escape("[ello]")));
            Assert.False(reader.isStartSection(SectionedStreamWriter.Escape("ello]")));
            Assert.False(reader.isStartSection(SectionedStreamWriter.Escape("[ello")));
            Assert.True(reader.isStartSection("[ello]"));
        }
    }
}
