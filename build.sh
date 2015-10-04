#!/bin/bash

set -x

rm -rf bin/
mkdir bin/

cp packages/NUnit.*/lib/nunit.framework.dll bin/

# Build Oats
################################################################################

echo 'using System;' > assembly.cs
echo 'using System.Reflection;' >> assembly.cs
echo 'using System.Runtime.CompilerServices;' >> assembly.cs
echo '[assembly:AssemblyTitle ("Oats")]' >> assembly.cs
echo '[assembly:AssemblyDescription ("C# binary serialisation library.")]' >> assembly.cs
echo '[assembly:AssemblyCopyright ("Ash Pook")]' >> assembly.cs
echo '[assembly:CLSCompliant (true)]' >> assembly.cs
echo '[assembly: AssemblyVersion ("0.9.1")]' >> assembly.cs

mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/oats.dll \
-target:library \
-recurse:assembly.cs \
-recurse:source/oats/src/main/cs/*.cs \
/doc:bin/oats.xml \
-lib:bin/

rm assembly.cs


# Build Tests
################################################################################


mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/oats.test.dll \
-target:library \
-reference:oats.dll \
-reference:nunit.framework.dll \
-recurse:source/oats/src/test/cs/*.cs \
-lib:bin/
