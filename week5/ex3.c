#include<pthread.h>
#include<stdlib.h>
#include<stdio.h>

#define MAX_ITEMS 1000

int consume = 0;
int produce = 0;

int items = 0;

void calculate() {
	usleep(50000);
}

void add_item() {
	items += 1;
}

void remove_item() {
	items -= 1;
}

void * consumer(void* n) {
	while(1) {
		if (items == 0) consume = 0;
		while(!consume);

		calculate();
		remove_item();
		if (items == MAX_ITEMS - 1) produce = 1;
		calculate();
	}
}

void * producer(void* n) {
	while(1) {
		calculate();

		if (items == MAX_ITEMS) produce = 0;
		while(!produce);

		calculate();
		add_item();
		if (items == 1) consume = 1;
	}
}

int main(void) {
	pthread_t prod, cons;
	produce = 1;
	pthread_create(&prod, NULL, producer, NULL);
	pthread_create(&cons, NULL, consumer, NULL);

	while(consumer || producer) {
		sleep(20);
		printf("consumer -> %d\n", consume);
		printf("producer -> %d\n", producer);
	}
	printf("Deadlock.\n");

	exit(0);
}
