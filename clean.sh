#!/bin/bash

find . -name .DS_Store -exec rm -rf {} \;
find source/ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
