#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdlib.h>

int main() {
    int n = 22;
    pid_t pid;
	pid = fork();
	if (pid == -1){
		return 1;
	}
	if (pid == 0){
        printf("Hello from child [%d - %d]\n", pid, n);
		return 0;
	}
    printf("Hello from parent [%d - %d]\n", pid, n);
    return 0;
}
