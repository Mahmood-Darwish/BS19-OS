#include <stdio.h>
#include <string.h>

int main(){
	char a[10000];
	printf("Input a string less than 10000 size\n");
	fgets(a, 10000, stdin);
	for(int i = strlen(a) - 1; i >= 0 ; i--){
		printf("%c", a[i]);
	}
	printf("\n");
	return 0;
}
