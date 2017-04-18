#!/bin/bash
LOG_DATA=/Users/Franz/Library/Logs/Unity/Editor.log
TMP=/Users/Franz/Documents/524LoopControl/LoopControl/tmp_mid
RES=/Users/Franz/Documents/524LoopControl/LoopControl/tmp_log
echo `strings $LOG_DATA` > $TMP
echo "" > $RES
for i in `cat logtool.conf`;
do
    if grep -q $i $TMP;
    then
        echo `grep -q $i $TMP` >> $RES
    fi
done

