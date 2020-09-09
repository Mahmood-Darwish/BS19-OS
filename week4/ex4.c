#include <stdio.h>
#include <sys/types.h>
#include <stdlib.h>
#include <string.h>

void main() {  
  pid_t pid = getpid(); 
  while(1) {
    char message[256];
  	char command[200];  
    printf("SimpleShell~$: ");

    fgets(command, 200, stdin);
    strcpy(message, command);

    message[strcspn(message, "\n")] = 0;
	printf("%s\n", message);
	
    pid = fork();
    if (pid == 0) {  
      system(message);  
      exit(0);
    }
  }

  return;
}
