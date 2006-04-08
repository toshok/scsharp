
#include "StormLib.h"

int main (int argc, char **argv)
{
  HANDLE mpq;

  char *mpq_filename = argv[1];
  char *file_path = argv[2];
  char *output_path = argv[3];

  if (!SFileOpenArchive (mpq_filename, 0, 0, &mpq))
	printf ("FAILED TO OPEN FILE\n");

  HANDLE hFile;

  SFileOpenFileEx(mpq, file_path, 0, &hFile);
  if(hFile != NULL)
    {
        DWORD dwTransferred;
        static char *buffer;
        DWORD fileSize;

        fileSize = SFileGetFileInfo (hFile, SFILE_INFO_FILE_SIZE);

	buffer = (char*)malloc (fileSize);
 
        SFileReadFile(hFile, buffer, fileSize, &dwTransferred, NULL);

        printf ("Read file %s, size %d\n", file_path, dwTransferred);

	FILE *fp = fopen (output_path, "w");
	fwrite (buffer, fileSize, 1, fp);
	fclose (fp);

	SFileCloseFile (hFile);
    }

    SFileCloseArchive(mpq);
}

