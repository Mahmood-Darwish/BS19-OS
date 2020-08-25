#include <stdio.h>

int main(){
	int n;
	printf("Length of the triangle: ");
	scanf("%d", &n);
	printf("\n");
	int i = 1;
	while(i <= n){
		int temp = n - i;
		while(temp > 0){
			printf(" ");
			temp--;
		}
		temp = (2 * i) - 1;
		while(temp > 0){
			printf("*");
			temp--;
		}
		temp = n - i;
		while(temp > 0){
			printf(" ");
			temp--;
		}
		i++;
		printf("\n");
	}
	return 0;
}
