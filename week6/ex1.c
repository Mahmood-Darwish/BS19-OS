#include <stdio.h>
#include <stdlib.h>
#include <time.h>
 #define max(a,b) \
   ({ __typeof__ (a) _a = (a); \
       __typeof__ (b) _b = (b); \
     _a > _b ? _a : _b; })

struct pair{
    int x, y;
};

int n;
clock_t start, end;
double cpu_time_used;
struct pair arv_time[1000000];
int waiting_time[1000000];
int TAT[1000000];
int temp1, temp2;

int comp (const void * elem1, const void * elem2)
{
    struct pair f = *((struct pair*)elem1);
    struct pair s = *((struct pair*)elem2);
    if (f.x > s.x) return  1;
    if (f.x < s.x) return -1;
    return 0;
}

int main()
{
    printf("Enter the number of processes:\n");
    scanf("%d", &n);
    printf("Enter processes line by line in the format \" arrival_time burst_time \" (counting starts from 0):\n");
    int i;
    for(i = 0 ; i < n ;i++){
        scanf("%d %d", &arv_time[i].x, &arv_time[i].y);
    }
    int time = 0;
    start = clock();
    qsort (arv_time, n, sizeof(*arv_time), comp);
    for(i = 0; i < n; i++){
        printf("The %d-th process:\n", i);
        printf("%d %d\n", arv_time[i].x, arv_time[i].y);
    }
    for( i = 0 ; i < n ; i++){
        waiting_time[i] = max(0, time- arv_time[i].x);
        time = max(time, arv_time[i].x);
        time += arv_time[i].y;
        TAT[i] = waiting_time[i] + arv_time[i].y;
        temp1 += waiting_time[i];
        temp2 += TAT[i];
    }
    for(i = 0 ; i < n ; i++){
        printf("Waiting time for the %d-th process: %d\n", i, waiting_time[i]);
        printf("TAT for the %d-th process: %d\n", i, TAT[i]);
    }
    printf("Average waiting time: %lf\n", (temp1+0.0)/(n+0.0));
    printf("Average TAT: %lf\n", (temp2+0.0)/(n+0.0));
    end = clock();
    cpu_time_used = ((double) (end - start)) / CLOCKS_PER_SEC;
    printf("Compilation time: %lf\n", cpu_time_used);
    return 0;
}
