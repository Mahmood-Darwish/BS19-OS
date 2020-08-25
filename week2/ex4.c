#include <stdio.h>

void sswap(int *x, int *y) {
   int temp;
   temp = *x;
   *x = *y;
   *y = temp;
}

int main(){
	int x, y;
	printf("Input two integers x and y respectively.\n");
	scanf("%d %d", &x, &y);
	sswap(&x, &y);
	printf("value of x is %d.\n", x);
	printf("value of y is %d.\n", y);
	return 0;
}
