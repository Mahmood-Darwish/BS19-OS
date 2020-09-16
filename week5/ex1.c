#include<pthread.h>
#include<stdio.h>
#include<stdlib.h>



void * F(void * temp) {
	printf("Printing from thread %d \n", gettid());
	pthread_exit(NULL);
}

int main() {
	int n = 10;
	pthread_t threads[n];
	for (int i = 0; i < n; i++) {
		int rc = pthread_create(&threads[i], NULL, F, NULL);
		if (rc == 0){
			printf("Thread %d created.\n", i);
		}
		rc = pthread_join(threads[i], NULL);
		if (rc == 0){
			printf("Thread %d finished\n", i);
		}
	}
	pthread_exit(NULL);
}
