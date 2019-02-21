#!/bin/sh
echo "STM8 eForth MODBUS Test"

# export temp_in=$(mktemp)
# export temp_out=$(mktemp)
# echo '010100000002BDCB' | xxd -r -p > b
# xxd b
# timeout --foreground 2 sstm8 -g -tS103 -Suart=1,in=b,out=c home/SWIMCOM-forth.ihx
# xxd c

# echo '010100000002BDCB' | xxd -r -p > $temp_in
# timeout --signal=SIGKILL 2
# timeout 2 sstm8 -V -w -tS103 -g -Suart=1,in=$temp_in,out=$temp_out home/SWIMCOM-forth.ihx
# xxd "$temp_out"
# rm $temp_in $temp_out
