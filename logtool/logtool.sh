#!/bin/bash
rm logdata
for i in `cat logtool.conf`;
do
    if grep -q $i ./../Log_*;
    then
        echo `grep $i ./../Log_*` >> logdata
    fi
done
