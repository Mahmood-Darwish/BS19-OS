#include <stdio.h>
#include <stdlib.h>
#include <string.h>

void* Realloc(void* p1, size_t sz) {
  if (p1 == NULL) return malloc(sz);
  if (sz == 0) {
    free(p1);
    return NULL;
  }
  void* p2 = malloc(sz);
  memcpy(p2, p1, sz);
  free(p1);
  return p2;
}

int n = 5, m = 9;

int main() {
  int *arr = Realloc(NULL, n * sizeof(int));
  for(int i = 0 ; i < n ; i++){
  	arr[i] = i;
  	printf("%d ", arr[i]);
  }
  printf("\n");
  arr = Realloc(arr, m * sizeof(int));
   for(int i = 0 ; i < m ; i++)
  	printf("%d ", arr[i]);
  arr = Realloc(arr, 0);
  return 0;
}
