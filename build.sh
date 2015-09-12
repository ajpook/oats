#!/bin/bash

set -x #echo on

rm -rf bin/
mkdir bin/

cp packages/NUnit.2.6.4/lib/nunit.framework.dll bin/

mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/oats.dll \
-target:library \
-recurse:source/oats/src/main/cs/*.cs \
/doc:bin/oats.xml \
-lib:bin/


mcs \
-unsafe \
-debug \
-define:DEBUG \
-out:bin/oats.test.dll \
-target:library \
-recurse:source/oats/src/test/cs/*.cs \
-lib:bin/ \
-lib:packages/NUnit.2.6.4/lib \
-reference:oats.dll \
-reference:nunit.framework.dll
