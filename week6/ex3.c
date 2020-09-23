#include<stdio.h>
#include <time.h>

void swap(int* first, int* second) {
    int temp;
    temp = *first;
    *first = *second;
    *second = temp;
}

int N, arrive[1000000], burst[1000000];
int Q;
int index[1000000];
int burst1[1000000];
int timee = 0;
int turnAround[1000000];
int wait[1000000];
int i, j;
clock_t start, end;
double cpu_timee_used;


int main() {

    printf("Enter the number of processes:\n");
    scanf("%d", &N);

    printf("Enter the value of quantum:\n");
    scanf("%d", &Q);

    printf("Enter processes line by line in the format \" arrival_time burst_time \" (counting starts from 0):\n");
    for (i = 0; i < N; i++) {
        index[i] = i + 1;
        scanf("%d", &arrive[i]);
        scanf("%d", &burst[i]);
        burst1[i] = burst[i];
    }

    start = clock();
    for (i = 0; i < N; i++) {
        for (j = 0; j < N - 1; j++) {
            if (arrive[j] > arrive[j + 1]) {
                swap(&arrive[j], &arrive[j + 1]);
                swap(&burst[j], &burst[j + 1]);
                swap(&burst1[j], &burst1[j + 1]);
                swap(&index[j], &index[j + 1]);
            }
        }
    }

    wait[0] = arrive[0];
    int numOfEx = 0;
    while (numOfEx != N) {
        int temp = 0;
        for ( i = 0; i < N; i++) {
            if (timee >= arrive[i]) {
                temp = 1;
                if (burst[i] > 0) {
                    if (burst[i] <= Q) {
                        numOfEx++;
                        timee = timee + burst[i];
                        turnAround[i] = timee - arrive[i];
                        burst[i] = burst[i] - Q;
                    }
                    else {
                        burst[i] = burst[i] - Q;
                        timee = timee + Q;
                    }
                }
            }
        }
        if(temp == 0){
            timee++;
        }
    }
    double avWait = 0;
    for ( i = 0; i < N; i++) {
        wait[i] = turnAround[i] - burst1[i];
        avWait = avWait + wait[i];
    }
    avWait = avWait / N;

    double avTurnAround = 0;
    for ( i = 0; i < N; i++) {
        avTurnAround = avTurnAround + turnAround[i];
    }
    avTurnAround = avTurnAround / N;

    for(i = 0 ; i < N ; i++){
        printf("The %d-th process:\n", i);
        printf("%d %d\n", arrive[i], burst1[i]);
    }

    for(i = 0 ; i < N ; i++){
        printf("Waiting time for the %d-th process: %d\n", i, wait[i]);
        printf("TAT for the %d-th process: %d\n", i, turnAround[i]);
    }

    printf("Average waiting timee: %lf\n", (avWait+0.0)/(N+0.0));
    printf("Average TAT: %lf\n", (avTurnAround+0.0)/(N+0.0));
    end = clock();
    cpu_timee_used = ((double) (end - start)) / CLOCKS_PER_SEC;
    printf("Compilation timee: %lf\n", cpu_timee_used);
    return 0;
}
