using NUnit.Framework;
using System;

namespace NetTopologySuite.IO.ShapeFile.Test
{
    [NtsIssueNumber(174)]
    class Issue174
    {
        [Test]
        public void ensure_NetTopologySuite_IO_ShapeFile_assembly_is_strongly_named()
        {
            AssertStronglyNamedAssembly(typeof(ShapeReader));
        }

        [Test]
        public void ensure_NetTopologySuite_IO_GDB_assembly_is_strongly_named()
        {
            AssertStronglyNamedAssembly(typeof(GDBReader));
        }

        [Test]
        public void ensure_NetTopologySuite_IO_GeoTools_assembly_is_strongly_named()
        {
            AssertStronglyNamedAssembly(typeof(ShapefileDataReader));
        }

        private void AssertStronglyNamedAssembly(Type typeFromAssemblyToCheck)
        {
            Assert.IsNotNull(typeFromAssemblyToCheck, "Cannot determine assembly from null");
            var assembly = typeFromAssemblyToCheck.Assembly;
            StringAssert.DoesNotContain("PublicKeyToken=null", assembly.FullName, "Strongly named assembly should have a PublicKeyToken in fully qualified name");
        }
    }
}
