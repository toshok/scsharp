
#include "Storm.h"

int main (int argc, char **argv)
{
  HANDLE mpq;

  char *mpq_filename = argv[1];
  char *file_path = argv[2];

  if (!SFileOpenArchive (mpq_filename, 0, 0, &mpq))
	printf ("FAILED TO OPEN FILE\n");

  HANDLE hFile;

  SFileOpenFileEx(mpq, file_path, 0, &hFile);
  if(hFile != NULL)
    {
        DWORD dwTransferred;
        static char Buffer1[0x100000];
        static char Buffer2[0x100000];
        int nOutlength = sizeof(Buffer2);
        DWORD fileSize;

        SFileGetFileSize (hFile, &fileSize);
        printf ("fileSize = %d\n", fileSize);
 
        SFileReadFile(hFile, Buffer1, sizeof(Buffer1), &dwTransferred, NULL);

        printf ("Read file %s, size %d\n", file_path, dwTransferred);
    }

    SFileCloseArchive(mpq);
}

