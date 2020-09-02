#include <stdio.h>

int n;

void bubble_sort(int a[])
{
	for(int i = 0 ; i < n ; i++){
		for(int j = 0 ; j < n - 1; j++){
			if(a[j] > a[j + 1]){
				int temp = a[j + 1];
				a[j + 1] = a[j];
				a[j] = temp;
			}
		}
	}
	return;
}

int main(){
	printf("Please enter the size of the array\n");
	scanf("%d", &n);
	int a[n + 5];
	printf("Please enter the elements of the array\n");
	for(int i = 0; i < n; i++){
		scanf("%d", &a[i]);
	}
	bubble_sort(a);
	printf("The array after sorting:\n");
	for(int i = 0 ; i < n ; i++)
		printf("%d ", a[i]);
	printf("\n");
	return 0;
}
