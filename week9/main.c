#include <stdio.h>
#include <stdlib.h>

#define Max_Page_Number 100100

int page[Max_Page_Number];
int pageFrame[Max_Page_Number];
int total, misses;

int main()
{
    int pageFrames;
    printf("Enter number of page frames:\n");
    scanf("%d", &pageFrames);
    int sizeofPageFrame;
    printf("Enter size of page frame (in bits):\n");
    scanf("%d", &sizeofPageFrame);
    int i;
    for(i = 0 ; i < pageFrames; i++)
        pageFrame[i] = -1;
    FILE *file = fopen("input.txt", "r");
    int x;
    fscanf (file, "%d", &x);
    while (!feof (file))
    {
        int found = 0, old = 0;
        total++;
        for(i = 0 ; i < pageFrames; i++){
            if(page[i] == x){
                pageFrame[i] = (pageFrame[i] >> 1) | (1 << (sizeofPageFrame - 1));
                found = 1;
            }
            else{
                pageFrame[i] >>= 1;
            }
            if(pageFrame[i] < pageFrame[old]){
                old = i;
            }
        }
        if(found == 0){
            misses++;
            pageFrame[old] = (1 << (sizeofPageFrame - 1));
            page[old] = x;
        }
        fscanf (file, "%d", &x);
    }
    printf("Total References: %d\n", total);
    printf("Hits: %d\n", total - misses);
    printf("Misses: %d\n", misses);
    printf("Hits/Misses: %.2f\n", (double)(total-misses + 0.0) / (misses + 0.0));
    printf("Hits/Total percentage: %.2f\n", (double)(((total-misses) * 100) + 0.0) / (total + 0.0));
    printf("Misses/Total percentage: %.2f\n", (double)((misses * 100) + 0.0) / (total + 0.0));
    fclose(file);
    return 0;
}
