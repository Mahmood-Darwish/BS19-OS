#!/bin/bash

for ((n=0;n<10;n++)); 
do 
	gcc ex1.c -o ex1
	./ex1
	rm ex1
done
