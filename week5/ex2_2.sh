#!/bin/bash

n=0

if [ ! -f ex2.txt ]; then
	echo 0 >> ex2.txt
fi

while ! ln ex2.txt ex2.txt.lock 2> /dev/null
do
	sleep 1
done

for i in {1..50}
do
	n=$(tail -n 1 ex2.txt)
	n=$((n + 1))
	echo $n >> ex2.txt
done

rm -f ex2.txt.lock
