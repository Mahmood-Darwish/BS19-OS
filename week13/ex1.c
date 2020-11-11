#include <stdio.h>
#include <stdlib.h>

#define m 3
#define n 5
int E[m];
int A[m];
int allocation[n][m];
int request[n][m];
int work[m];
int finish[n];

int main()
{
    FILE *in_file  = fopen("input.txt", "r");
    FILE *out_file = fopen("output.txt", "w");
    int i, j;
    for( i = 0 ; i < m ; i++)
        fscanf(in_file, "%d", &E[i]);
    for( i = 0 ; i < m ; i++)
        fscanf(in_file, "%d", &A[i]);
    for(i = 0 ; i < m ; i++)
        work[i] = E[i] - A[i];
    for( i = 0 ; i < n ; i++){
        for( j = 0; j < m ; j++){
            fscanf(in_file, "%d", &allocation[i][j]);
        }
    }
    for( i = 0 ; i < n ; i++){
        for( j = 0; j < m ; j++){
            fscanf(in_file, "%d", &request[i][j]);
        }
    }
    for(i = 0 ; i < n ; i++){
        int temp = 0;
        for(j = 0 ; j < m ; j++){
            if(request[i][j] != 0)
                temp = 1;
        }
        if(temp == 0)
            finish[i] = 1;
        else
            finish[i] = 0;
    }
    for(i = 0 ; i < n ; i++){
        if(finish[i] == 0){
            int temp = 0;
            for(j = 0 ; j < m ; j++){
                if(request[i][j] > work[j]){
                    temp = 1;
                }
            }
            if(temp == 0){
                for(j = 0; j < m ; j++)
                    work[j] += allocation[i][j];
                finish[i] = 1;
                i = -1;
            }
        }
    }
    int temp = 0;
    for(i = 0 ; i < n ; i++){
        if(finish[i] == 0){
            temp = 1;
            fprintf(out_file, "process %d deadlocked\n", i + 1);
        }
    }
    if(temp == 0)
        fprintf(out_file, "no deadlock\n");
    return 0;
}
