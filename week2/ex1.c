#include <stdio.h>
#include <limits.h>
#include <float.h>

int main(){
	int x = INT_MAX;
	float y = FLT_MAX;
	double z = DBL_MAX;
	printf("Value of the integer is %d, and the size is %lu\n", x, sizeof(x));
	printf("Value of the float is %f, and the size is %lu\n", y, sizeof(y));
	printf("Value of the double is %lf, and the size is %lu\n", z, sizeof(z));
	return 0;
}
