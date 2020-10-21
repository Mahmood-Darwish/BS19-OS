#include <stdio.h>
#include <sys/stat.h>
#include <stdlib.h>
#include <dirent.h>

int n;

int filter(struct stat* entry)
{
    return (entry->st_ino == n) ? 1 : 0;
}

int main(){
	DIR *dirp;
	dirp = opendir("/Home/OS/week10/tmp");
	FILE* dp;
	while ((dp = readdir(dirp)) != NULL) {	
		struct stat file_stat;  
		fstat (dp, &file_stat); 
		if(file_stat.st_nlink >= 0){
			n = file_stat.st_ino;
			struct dirent **namelist = NULL;
			int* noOfFiles;
			*noOfFiles = scandir("/home/", &namelist, filter, NULL);
			for (int i = 0; i < noOfFiles; i++)
			{
				printf("%c", noOfFiles[i]);
				free(noOfFiles[i]);
			}
			free(namelist);
		}
	}
}
