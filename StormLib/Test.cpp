/*****************************************************************************/
/* StormLibTest.cpp                       Copyright (c) Ladislav Zezula 2003 */
/*---------------------------------------------------------------------------*/
/* This module uses very brutal test methods for StormLib. It extracts all   */
/* files from the archive with Storm.dll and with stormlib and compares them,*/
/* then tries to build a copy of the entire archive, then removes a few files*/
/* from the archive and adds them back, then compares the two archives, ...  */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 25.03.03  1.00  Lad  The first version of StormLibTest.cpp                */
/*****************************************************************************/

#include <io.h>
#include <conio.h>
#include <stdio.h>
#include <windows.h>
#include <mmsystem.h>

#define __LINK_STORM_DLL__      // Force using Storm.dll API
#define __STORMLIB_SELF__
#include "StormLib.h"

#pragma warning(disable : 4505) 
#pragma comment(lib, "Winmm.lib")

//------------------------------------------------------------------------------
// Defines

#define MPQ_BLOCK_SIZE 0x1000

//-----------------------------------------------------------------------------
// Constants

static const char * szWorkDir = ".\\!Work";
static       BYTE * pbBuffer1 = NULL;   // For using in Storm.dll
static       BYTE * pbBuffer2 = NULL;   // For using in StormLib

//-----------------------------------------------------------------------------
// Local testing functions

static void clreol()
{
    printf("\r                                                                              \r");
}

static const char * GetPlainName(const char * szFileName)
{
    char * szTemp;

    if((szTemp = strrchr(szFileName, '\\')) != NULL)
        szFileName = szTemp + 1;
    return szFileName;
}

int GetFirstDiffer(void * ptr1, void * ptr2, int nSize)
{
    char * buff1 = (char *)ptr1;
    char * buff2 = (char *)ptr2;
    int nDiffer;

    for(nDiffer = 0; nDiffer < nSize; nDiffer++)
    {
        if(*buff1++ != *buff2++)
            return nDiffer;
    }
    return -1;
}

static void ShowProcessedFile(const char * szFileName)
{
    char szLine[80];
    int nLength = strlen(szFileName);

    memset(szLine, 0x20, sizeof(szLine));
    szLine[sizeof(szLine)-1] = 0;

    if(nLength > sizeof(szLine)-1)
        nLength = sizeof(szLine)-1;
    memcpy(szLine, szFileName, nLength);
    printf("\r%s\r", szLine);
}

static void WINAPI CompactCB(void * /* lpParam */, DWORD dwWork, DWORD dwParam1, DWORD dwParam2)
{
    switch(dwWork)
    {
        case CCB_CHECKING_FILES:
            printf("Checking files in archive ...\r");
            break;

        case CCB_COMPACTING_FILES:
            printf("Compacting archive (%u of %u) ...\r", dwParam1, dwParam2);
            break;

        case CCB_CLOSING_ARCHIVE:
            printf("Closing archive ...\r");
            break;
    }
}

static int ExtractBytes(HANDLE hMpq, const char * szFileName, void * pBuffer, DWORD & dwBytes)
{
    HANDLE hFile = NULL;
    int nError = ERROR_SUCCESS;

    // Open the file
    if(nError == ERROR_SUCCESS)
    {
        if(!SFileOpenFileEx(hMpq, szFileName, 0, &hFile))
            nError = GetLastError();
    }

    // Read first block
    if(nError == ERROR_SUCCESS)
    {
        if(!SFileReadFile(hFile, pBuffer, dwBytes, &dwBytes, NULL))
            nError = GetLastError();
        if(nError == ERROR_HANDLE_EOF)
            nError = ERROR_SUCCESS;
    }

    // Close the file and cleanup
    if(hFile != NULL)
        SFileCloseFile(hFile);
    return nError;
}

static int ExtractFile(HANDLE hMpq, const char * szFileName, const char * szLocalFile, DWORD * pdwFlags)
{
    HANDLE hLocalFile = INVALID_HANDLE_VALUE;
    HANDLE hMpqFile = NULL;
    int nError = ERROR_SUCCESS;

    // Create the local file. Overwrite existing
    if(nError == ERROR_SUCCESS)
    {
        hLocalFile = CreateFile(szLocalFile, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL);
        if(hLocalFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }

    // Open the MPQ file
    if(nError == ERROR_SUCCESS)
    {
        if(!SFileOpenFileEx(hMpq, szFileName, 0, &hMpqFile))
            nError = GetLastError();
    }

    // If file flags required, retrieve them
    if(nError == ERROR_SUCCESS && pdwFlags != NULL)
    {
        if((*pdwFlags = SFileGetFileInfo(hMpqFile, SFILE_INFO_FLAGS)) == -1)
            nError = GetLastError();
    }

    // Copy the file's content
    if(nError == ERROR_SUCCESS)
    {
        char  szBuffer[MPQ_BLOCK_SIZE];
        DWORD dwBytes = 1;

        while(dwBytes > 0)
        {
            SFileReadFile(hMpqFile, szBuffer, sizeof(szBuffer), &dwBytes, NULL);
            if(dwBytes > 0)
            {
                if(!WriteFile(hLocalFile, szBuffer, dwBytes, &dwBytes, NULL))
                    nError = GetLastError();
            }
        }
    }

    // Close the files
    if(hMpqFile != NULL)
        SFileCloseFile(hMpqFile);
    if(hLocalFile != NULL)
        CloseHandle(hLocalFile);
    return nError;
}

static int CompareArchivedFiles(const char * szFileName, HANDLE hFile1, HANDLE hFile2, DWORD dwBlockSize)
{
    DWORD dwRead1;                      // Number of bytes read (Storm.dll)
    DWORD dwRead2;                      // Number of bytes read (StormLib)
    DWORD dwBlock = 0;
    BOOL bResult1 = FALSE;              // Result from Storm.dll
    BOOL bResult2 = FALSE;              // Result from StormLib
    int nDiff;
//  int nError = ERROR_SUCCESS;

    // Debug
//    if(!stricmp(szFileName, "monsters\\mega\\megas2.wav"))
//        szFileName = szFileName;

    for(;;)
    {
        // Read the file's content by both methods and compare the result
        clreol();
        printf("SeqRead: \"%s\", block %u (%u bytes) ...\r", szFileName, dwBlock++, dwBlockSize);
        memset(pbBuffer1, 0, dwBlockSize);
        memset(pbBuffer2, 0, dwBlockSize);
        bResult1 = StormReadFile(hFile1, pbBuffer1, dwBlockSize, &dwRead1, NULL);
        bResult2 = SFileReadFile(hFile2, pbBuffer2, dwBlockSize, &dwRead2, NULL);
        if(bResult1 != bResult2)
        {
            printf("Different results from SFileReadFile, Storm.dll: %u, StormLib: %u !!!!\n", bResult1, bResult2);
            break;
        }

        // Test the number of bytes read
        if(dwRead1 != dwRead2)
        {
            printf("Different bytes read from SFileReadFile, Storm.dll: %u, StormLib: %u !!!!\n", dwRead1, dwRead2);
            break;
        }

        // No more bytes ==> OK
        if(dwRead1 == 0)
        {
            clreol();
            return ERROR_SUCCESS;
        }
        
        // Test the content
        if((nDiff = GetFirstDiffer(pbBuffer1, pbBuffer2, dwRead1)) != -1)
        {
            printf("Different block content from SFileReadFile at offset 0x%08lX!!\n", nDiff);
            break;
        }
    }
    return ERROR_READ_FAULT;
}

// Random read version
static int CompareArchivedFilesRR(const char * szFileName, HANDLE hFile1, HANDLE hFile2, DWORD dwBlockSize)
{
    DWORD dwFileSize1;                  // File size (Storm.dll)
    DWORD dwFileSize2;                  // File size (StormLib)
    DWORD dwRead1;                      // Number of bytes read (Storm.dll)
    DWORD dwRead2;                      // Number of bytes read (StormLib)
    BOOL bResult1 = FALSE;              // Result from Storm.dll
    BOOL bResult2 = FALSE;              // Result from StormLib
    int nError = ERROR_SUCCESS;

    // Test the file size
    dwFileSize1 = StormGetFileSize(hFile1, NULL);
    dwFileSize2 = SFileGetFileSize(hFile2, NULL);
    if(dwFileSize1 != dwFileSize2)
    {
        printf("Different size from SFileGetFileSize, Storm.dll: %u, StormLib: %u !!!!\n", dwFileSize1, dwFileSize2);
        return ERROR_GEN_FAILURE;
    }

    for(int i = 0; i < 100; i++)
    {
        DWORD dwRandom   = rand() * rand();
        DWORD dwPosition = dwRandom % dwFileSize1;
        DWORD dwToRead   = dwRandom % dwBlockSize; 

        // Set the file pointer
        printf("RndRead: \"%s\", position %u, size %u ...\r", szFileName, dwPosition, dwToRead);
        dwRead1 = StormSetFilePointer(hFile1, dwPosition, NULL, FILE_BEGIN);
        dwRead2 = SFileSetFilePointer(hFile2, dwPosition, NULL, FILE_BEGIN);
        if(dwRead1 != dwRead2)
        {
            printf("Difference returned by SFileSetFilePointer, Storm.dll: %u, StormLib: %u !!!!\n", dwRead1, dwRead2);
            nError = ERROR_READ_FAULT;
            break;
        }

        // Read the file's content by both methods and compare the result
        bResult1 = StormReadFile(hFile1, pbBuffer1, dwToRead, &dwRead1, NULL);
        bResult2 = SFileReadFile(hFile2, pbBuffer2, dwToRead, &dwRead2, NULL);
        if(bResult1 != bResult2)
        {
            printf("Different results from SFileReadFile, Storm.dll: %u, StormLib: %u !!!!\n", bResult1, bResult2);
            nError = ERROR_READ_FAULT;
            break;
        }

        // Test the number of bytes read
        if(dwRead1 != dwRead2)
        {
            printf("Different bytes read from SFileReadFile, Storm.dll: %u, StormLib: %u !!!!\n", dwRead1, dwRead2);
            nError = ERROR_READ_FAULT;
            break;
        }
        
        // Test the content
        if(dwRead1 != 0 && memcmp(pbBuffer1, pbBuffer2, dwRead1))
        {
            printf("Different block content from SFileReadFile !!\n");
            nError = ERROR_READ_FAULT;
            break;
        }
    }
    clreol();
    return ERROR_SUCCESS;
}

static int TestMPQFromDiabloI(const char * szMpqName, const char * szMpqCopyName)
{
    HANDLE hMpq = NULL;
    int nError = ERROR_SUCCESS;

    if(nError == ERROR_SUCCESS)
    {
        printf("Copying %s to %s ...\n", szMpqName, szMpqCopyName);
        if(!CopyFile(szMpqName, szMpqCopyName, FALSE))
            nError = GetLastError();
    }
    
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening archive %s ...\n", szMpqCopyName);
        if(!SFileCreateArchiveEx(szMpqCopyName, OPEN_EXISTING, 0, &hMpq))
            nError = GetLastError();
    }

    // Delete file from it.
    if(nError == ERROR_SUCCESS)
    {
        printf("Removing file \"hero\" ...\n");
        if(!SFileRemoveFile(hMpq, "hero"))
            nError = GetLastError();
    }

    // Compact the archive
    if(nError == ERROR_SUCCESS)
    {
        printf("Compacting archive ...\n");
        if(!SFileCompactArchive(hMpq, NULL))
            nError = GetLastError();
    }

    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    return nError;
}

static int TestNamelessAccess(const char * szMpqName)
{
    HANDLE hMpq  = NULL;
    HANDLE hFile = NULL;
    char szFileName[MAX_PATH];
    DWORD dwFiles = 0;
    int nError = ERROR_SUCCESS;

    if(nError == ERROR_SUCCESS)
    {
        printf("Opening archive for the nameless access ...\n");
        if(!SFileOpenArchive(szMpqName, 0, 0, &hMpq))
            nError = GetLastError();
    }

    // Try nameless remove (not implemented)
    if(nError == ERROR_SUCCESS)
    {
        if(!SFileRenameFile(hMpq, (char *)1, "data\\music.dat"))
            printf("Cannot delete the file #1");
    }

    // Try nameless delete (not implemented)
    if(nError == ERROR_SUCCESS)
    {
        if(!SFileRemoveFile(hMpq, (char *)1))
            printf("Cannot delete the file #1");
    }

    // Try to open all files
    if(nError == ERROR_SUCCESS)
    {
        dwFiles = SFileGetFileInfo(hMpq, SFILE_INFO_NUM_FILES);
        for(DWORD dwFile = 0; dwFile < dwFiles; dwFile++)
        {
            if(SFileOpenFileEx(hMpq, (char *)dwFile, 0, &hFile))
            {
                SFileGetFileName(hFile, szFileName);
                ShowProcessedFile(szFileName);
                SFileCloseFile(hFile);
            }
        }
        nError = ERROR_SUCCESS;
    }

    // Close the archive handle
    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    if(nError == ERROR_SUCCESS)
        printf("Nameless access test complete (No errors)\n");
    return nError;
}

static int TestFileExtraction(HANDLE hMpq1, HANDLE hMpq2, const char * szListFile, DWORD dwBlockSize, BOOL bDoRandomRead = FALSE)
{
    HANDLE hFile1 = NULL;               // Archive file open by Storm.dll
    HANDLE hFile2 = NULL;               // Archive file open by StormLib
    BOOL bResult1 = FALSE;              // Result from Storm.dll
    BOOL bResult2 = FALSE;              // Result from StormLib
    int nError = ERROR_SUCCESS;

    if(nError == ERROR_SUCCESS)
    {
        clreol(); printf("Allocating blocks ...\r");
        pbBuffer1 = (BYTE *)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, dwBlockSize);
        pbBuffer2 = (BYTE *)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, dwBlockSize);
        if(pbBuffer1 == NULL || pbBuffer2 == NULL)
            return ERROR_NOT_ENOUGH_MEMORY;
    }

    // Start the search
    if(nError == ERROR_SUCCESS)
    {
        SFILE_FIND_DATA wf;
        HANDLE hFind = SFileFindFirstFile(hMpq2, "*", &wf, szListFile);
        BOOL bResult = TRUE;                // MPQ search result

        clreol(); printf("Extracting files\r");

        // Go through all the files found in MPQ
        while(hFind != NULL && bResult == TRUE)
        {
            // Test only of not a list file
            if(stricmp(wf.cFileName, LISTFILE_NAME))
            {
                ShowProcessedFile(wf.cFileName);

                // Use the right locale found
                StormSetLocale(wf.lcLocale);
                SFileSetLocale(wf.lcLocale);

                // Debug
                if(!stricmp(wf.cFileName, "monsters\\mega\\megas2.wav"))
                    wf.cFileName[0] = wf.cFileName[0];

                // Try to open with Storm.dll and StormLib
                bResult1 = StormOpenFileEx(hMpq1, wf.cFileName, 0, &hFile1);
                bResult2 = SFileOpenFileEx(hMpq2, wf.cFileName, 0, &hFile2);
                if(bResult1 != bResult2)
                {
                    printf("Different results from SFileOpenFileEx, Storm.dll: %u, StormLib: %u !!!!\n", bResult1, bResult2);
                    nError = ERROR_GEN_FAILURE;
                    break;
                }

                if(bResult1 == TRUE && bResult2 == TRUE)
                {
                    // Test sequential read
                    if(CompareArchivedFiles(wf.cFileName, hFile1, hFile2, dwBlockSize))
                    {
                        printf("Sequential read test failed on \"%s\" !!!\n", wf.cFileName);
                        break;
                    }

                    // Test random read
                    if(bDoRandomRead && CompareArchivedFilesRR(wf.cFileName, hFile1, hFile2, dwBlockSize))
                    {
                        printf("Random read test failed on \"%s\" !!!\n", wf.cFileName);
                        break;
                    }

                    printf("Closing the file ...\r");
                    bResult1 = StormCloseFile(hFile1);
                    bResult2 = SFileCloseFile(hFile2);
                    if(bResult1 != bResult2)
                    {
                        printf("Different results from SFileCloseFile, Storm.dll: %u, StormLib: %u !!!!\n", bResult1, bResult2);
                        nError = ERROR_GEN_FAILURE;
                        break;
                    }
                }
            }
            // Move to the next file
            bResult = SFileFindNextFile(hFind, &wf);
            clreol();
        }

        // Close search handle
        if(hFind != NULL)
            SFileFindClose(hFind);
    }

    // Cleanup
    if(pbBuffer1 != NULL)
        HeapFree(GetProcessHeap(), 0, pbBuffer1);
    if(pbBuffer2 != NULL)
        HeapFree(GetProcessHeap(), 0, pbBuffer2);
    pbBuffer1 = pbBuffer2 = NULL;
    return nError;
}

static int TestBlockCompress(const char * szFileName)
{
    HANDLE hFile = INVALID_HANDLE_VALUE;
    BYTE * pDecompressed = NULL;
    BYTE * pCompressed = NULL;
    BYTE * pData = NULL;
    DWORD dwFileSize;
    int   nBlocks = 0;
    int nError = ERROR_SUCCESS;

    // Open the file
    if(nError == ERROR_SUCCESS)
    {
        hFile = CreateFile(szFileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
        if(hFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }

    // Allocate buffers
    if(nError == ERROR_SUCCESS)
    {
        // Must allocate twice blocks due to probable bug in Storm.dll.
        // Storm.dll corrupts stack when uncompresses data with PKWARE DCL
        // and no compression occurs.
        pDecompressed = new BYTE [MPQ_BLOCK_SIZE];
        pCompressed = new BYTE [MPQ_BLOCK_SIZE];
        pData = new BYTE[MPQ_BLOCK_SIZE];
        if(!pDecompressed || !pCompressed || !pData)
            nError = ERROR_NOT_ENOUGH_MEMORY;
    }
    if(nError == ERROR_SUCCESS)
    {
        dwFileSize = GetFileSize(hFile, NULL);
        nBlocks    = dwFileSize / MPQ_BLOCK_SIZE;
        if(dwFileSize % MPQ_BLOCK_SIZE)
            nBlocks++;

        for(int i = 0x10; i < nBlocks; i++)
        {
            DWORD dwTransferred;
            int   nDcmpLength;
            int   nCmpLength;
            int   nCmpLevel = -1;
            int   nCmpType = 0;
            int   nCmp = 0x08;
            int   nDiff;

            clreol(); printf("Testing compression of block %u\r", i + 1);

            // Load the block from the file
            SetFilePointer(hFile, i * MPQ_BLOCK_SIZE, NULL, FILE_BEGIN);
            ReadFile(hFile, pData, MPQ_BLOCK_SIZE, &dwTransferred, NULL);
            if(dwTransferred == 0)
                continue;

            if(i == 2014)
                dwTransferred = MPQ_BLOCK_SIZE;

            // Compress the block
            nCmpLength = dwTransferred;
            SCompCompress((char *)pCompressed, &nCmpLength, (char *)pData, dwTransferred, nCmp, nCmpType, nCmpLevel);

            // Uncompress the block
            nDcmpLength = dwTransferred;
            SCompDecompress((char *)pDecompressed, &nDcmpLength, (char *)pCompressed, nCmpLength);
  
            if(nDcmpLength != (int)dwTransferred)
            {
                printf("Number of uncompressed bytes does not agree with original data !!!\n");
                break;
            }
            if((nDiff = GetFirstDiffer(pDecompressed, pData, dwTransferred)) != -1)
            {
                printf("Decompressed block does not agree with the original data !!! (Offset 0x%08lX)\n", nDiff);
                break;
            }

            if(pCompressed[MPQ_BLOCK_SIZE] != 0xFD)
            {
                printf("Damage after compressed block !!!\n");
                break;
            }
        }
    }

    // Cleanup
    if(pData != NULL)
        delete [] pData;
    if(pCompressed != NULL)
        delete [] pCompressed;
    if(pDecompressed != NULL)
        delete [] pDecompressed;
    if(hFile != INVALID_HANDLE_VALUE)
        CloseHandle(hFile);
    clreol();
    return nError;
}


static int TestCompressions(const char * szTestFile)
{
    HANDLE hFile = INVALID_HANDLE_VALUE;
    BYTE * pDecompressed1 = NULL;
    BYTE * pDecompressed2 = NULL;
    BYTE * pCompressed2 = NULL;
    BYTE * pCompressed1 = NULL;
    BYTE * pData = NULL;
    DWORD dwFileSize;
    DWORD dwAllocSize = 0x10000;
    int nError = ERROR_SUCCESS;

    // Open the file
    if(nError == ERROR_SUCCESS)
    {
        hFile = CreateFile(szTestFile, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
        if(hFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }

    // Allocate buffers
    if(nError == ERROR_SUCCESS)
    {
        pDecompressed1 = new BYTE [dwAllocSize];
        pDecompressed2 = new BYTE [dwAllocSize];
        pCompressed1 = new BYTE [dwAllocSize];
        pCompressed2 = new BYTE [dwAllocSize];
        if(!pDecompressed1 || !pDecompressed2 || !pCompressed1 || !pCompressed2)
            nError = ERROR_NOT_ENOUGH_MEMORY;
    }
    if(nError == ERROR_SUCCESS)
    {
        dwFileSize = GetFileSize(hFile, NULL);

        for(int i = 0; i < 100000; i++)
        {
            DWORD dwTransferred;
            DWORD dwStart = 0;
            DWORD dwBytes = 0x1000;
            DWORD dwCmp   = 0x08;               // Test the Bzip2 compression
            BOOL  bResult1;                     // Result from Storm.dll
            BOOL  bResult2;                     // Result from StormLib
            int   nDcmpLength1;
            int   nDcmpLength2;
            int   nCmpLength1;
            int   nCmpLength2;
            int   nCmpLevel = -1;
            int   nCmpType = 0;
            int   nDiff;

            clreol();
            printf("Testing compression, no. %u\r", i + 1);

            // Debug
//          if(dwStart == 0x002648cc && dwBytes == 0x00000058)
//              dwCmp = 0x08;

            // Load the block from the file
            pData = new BYTE[dwBytes];
            SetFilePointer(hFile, dwStart, NULL, FILE_BEGIN);
            ReadFile(hFile, pData, dwBytes, &dwTransferred, NULL);
            if(dwTransferred == 0)
                continue;

            // Compress the block
            nCmpLevel = -1;
            nCmpLength2 = nCmpLength1 = dwTransferred;
            bResult1 = StormCompress((char *)pCompressed1, &nCmpLength1, (char *)pData, dwTransferred, dwCmp, nCmpType, nCmpLevel);
            bResult2 = SCompCompress((char *)pCompressed2, &nCmpLength2, (char *)pData, dwTransferred, dwCmp, nCmpType, nCmpLevel);

            if(bResult1 != bResult2)
            {
                printf("Compression result from Storm.dll does not agree with StormLib !!!\n");
                nError = ERROR_INVALID_DATA;
                break;
            }

            if(bResult1 == FALSE || bResult2 == FALSE)
                continue;
  
            //
            // Don't compare the compression results, if the compression contained
            // Pklib compression. Storm.dll's compression may produce compressed output
            // or output length different because of unzeroed data buffer passed
            // to "implode" function
            //
            if((dwCmp & 0x08) == 0)
            {
                if(nCmpLength1 != nCmpLength2)
                {
                    printf("Lengths of compressed blocks differ : %u vs. %u\n", nCmpLength1, nCmpLength2);
                    nError = ERROR_INVALID_DATA;
                    break;
                }

                if((nDiff = GetFirstDiffer(pCompressed1, pCompressed2, nCmpLength1)) != -1)
                {
                    printf("Compressed blocks does not agree at offset %u\n", nDiff);
                    nError = ERROR_INVALID_DATA;
                    break;
                }
            }

            // Uncompress the block
            memset(pDecompressed1, 0xCC, dwAllocSize);
            memset(pDecompressed2, 0xCC, dwAllocSize);
            nDcmpLength2 = nDcmpLength1 = dwTransferred;
            StormDecompress((char *)pDecompressed1, &nDcmpLength1, (char *)pCompressed1, nCmpLength1);
            SCompDecompress((char *)pDecompressed2, &nDcmpLength2, (char *)pCompressed2, nCmpLength2);
  
            // Compare the compression
            if(nDcmpLength1 != nDcmpLength2)
            {
                printf("Lengths of decompressed blocks differ : %u vs. %u\n", nDcmpLength1, nDcmpLength2);
                nError = ERROR_INVALID_DATA;
                break;
            }
            if((nDiff = GetFirstDiffer(pDecompressed1, pDecompressed2, nDcmpLength1)) != -1)
            {
                printf("Decompressed blocks does not agree at offset %u\n", nDiff);
                nError = ERROR_INVALID_DATA;
                break;
            }
  
            // Compare with the original data (only if the compression does not contain
            // a lossy method)
            if((dwCmp & 0xF0) == 0)
            {
                if(nDcmpLength2 != (int)dwTransferred)
                {
                    printf("Number of uncompressed bytes does not agree with original data !!!\n");
                    nError = ERROR_INVALID_DATA;
                    break;
                }
                if((nDiff = GetFirstDiffer(pDecompressed2, pData, dwTransferred)) != -1)
                {
                    printf("Decompressed block does not agree with the original data !!! (Offset 0x%08lX)\n", nDiff);
                    nError = ERROR_INVALID_DATA;
                    break;
                }
            }

            delete [] pData;
            pData = NULL;
        }
    }

    // Cleanup
    if(pData != NULL)
        delete [] pData;
    if(pCompressed1 != NULL)
        delete [] pCompressed1;
    if(pCompressed2 != NULL)
        delete [] pCompressed2;
    if(pDecompressed1 != NULL)
        delete [] pDecompressed1;
    if(pDecompressed2 != NULL)
        delete [] pDecompressed2;
    if(hFile != INVALID_HANDLE_VALUE)
        CloseHandle(hFile);
    if(nError == ERROR_SUCCESS)
        printf("Compression test OK                 \n");
    return nError;
}

static int TestWAVECompression(const char * szMpqName, const char * szWaveFile)
{
    const char * szArchivedFile = "Sound.wav";
    HANDLE hMpq  = NULL;
    HANDLE hFile = NULL;
    char szLocalFile[MAX_PATH];
    int nError = ERROR_SUCCESS;

    sprintf(szLocalFile, "%s\\%s", szWorkDir, szArchivedFile);

    for(DWORD dwQuality = MPQ_WAVE_QUALITY_HIGH; dwQuality <= MPQ_WAVE_QUALITY_LOW; dwQuality++)
    {
        // Create a new MPQ archive
        clreol(); printf("Creating new MPQ archive ...\r");
        if(!SFileCreateArchiveEx(szMpqName, CREATE_ALWAYS, 0, &hMpq))
        {
            nError = GetLastError();
            printf("Cannot create the MPQ : %s\n", szMpqName);
            break;
        }

        // Add the wave
        clreol(); printf("Adding wave ...\r");
        if(!SFileAddWave(hMpq, szWaveFile, szArchivedFile, MPQ_FILE_ENCRYPTED | MPQ_FILE_COMPRESS_MULTI, dwQuality))
        {
            nError = GetLastError();
            printf("Cannot add the file %s into MPQ\n", szWaveFile);
            break;
        }

        // Get the WAVE compressed size and ratio
        clreol(); printf("Getting compressed ratio ...\r");
        if(SFileOpenFileEx(hMpq, szArchivedFile, 0, &hFile))
        {
            DWORD dwFSize = SFileGetFileInfo(hFile, SFILE_INFO_FILE_SIZE);
            DWORD dwCSize = SFileGetFileInfo(hFile, SFILE_INFO_COMPRESSED_SIZE);

            clreol();
            printf("File size : %u, Compressed size : %u, Ratio : %u %%\n", dwFSize, dwCSize, (dwCSize * 100) / dwFSize);
            SFileCloseFile(hFile);
        }

        // Extract the file
        clreol(); printf("Extracting wave ...\r");
        if((nError = ExtractFile(hMpq, szArchivedFile, szLocalFile, NULL)) != ERROR_SUCCESS)
        {
            printf("Error extracting wave from MPQ\n");
            break;
        }

        // Play the wave
        clreol(); printf("Playing wave sound ...\r");
        if(!PlaySound(szLocalFile, NULL, SND_FILENAME))
        {
            nError = GetLastError();
            printf("Cannot play wave sound\n");
            break;
        }

        SFileCloseArchive(hMpq);
        DeleteFile(szMpqName);
        DeleteFile(szLocalFile);
        hMpq = NULL;
    }

    // Cleanup and return
    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    DeleteFile(szMpqName);
    DeleteFile(szLocalFile);
    return nError;
}

static int TestMpqFindFile(const char * szMpqName, const char * szListFile)
{
    SFILE_FIND_DATA sf;
    HANDLE hFind = NULL;
    HANDLE hMpq = NULL;
    BOOL bResult = TRUE;
    int nError = ERROR_SUCCESS;

    // Try to open the archive with StormLib
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening \"%s\" ...\n", szMpqName);
        if(!SFileOpenArchive(szMpqName, 0, 0, &hMpq))
            nError = GetLastError();
    }

    if(nError == ERROR_SUCCESS)
    {
        hFind = SFileFindFirstFile(hMpq, "*", &sf, szListFile);

        while(hFind != NULL && bResult == TRUE)
        {
//          printf("%s\n", sf.cFileName);

            bResult = SFileFindNextFile(hFind, &sf);
        }
    }

    // Cleanup & exit
    if(hFind != NULL)
        SFileFindClose(hFind);
    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    return nError;
}


static int TestMpqExtraction(const char * szMpqName1, const char * szMpqName2, const char * szListFile, DWORD & dwHashTableSize, BOOL bDoRandomRead = FALSE)
{
    HANDLE hMpq1 = NULL;                // From Storm.dll
    HANDLE hMpq2 = NULL;                // From StormLib
    int nError = ERROR_SUCCESS;

    // Try to open the archive with Storm.dll
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening \"%s\" (Storm.dll) ...\n", szMpqName1);
        if(!StormOpenArchive(szMpqName1, 0, 0, &hMpq1))
            nError = GetLastError();
    }

    // Try to open the archive with StormLib
    // To Valery : Please try to step inside SFileOpenArchive and see where the problem occurs
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening \"%s\" (StormLib) ...\n", szMpqName2);
        if(!SFileOpenArchive(szMpqName2, 0, 0, &hMpq2))
            nError = GetLastError();
    }

    // Retrieve the size of the hash table in the archive
    if(nError == ERROR_SUCCESS)
    {
        dwHashTableSize = SFileGetFileInfo(hMpq2, SFILE_INFO_HASH_TABLE_SIZE);
        if(dwHashTableSize == 0xFFFFFFFF)
            printf("SFileGetFileInfo(SFILE_INFO_HASH_TABLE_SIZE) failed\n");
    }

    // Try to extract all files from the archive and compare the results
    if(nError == ERROR_SUCCESS)
    {
        printf("Extracting files (sequential scan and random scan) ...\n");
        nError = TestFileExtraction(hMpq1, hMpq2, szListFile, MPQ_BLOCK_SIZE, bDoRandomRead);
    }

    if(hMpq2 != NULL)
        SFileCloseArchive(hMpq2);
    if(hMpq1 != NULL)
        StormCloseArchive(hMpq1);
    if(nError == ERROR_SUCCESS)
        printf("Extract test complete (No errors)\n");
    return nError;
}

static int TestMpqCreation(const char * szMpqName, const char * szMpqCopyName, const char * szListFile, DWORD dwHashTableSize, BOOL bConvertExisting)
{
    char   szLocalFile[MAX_PATH] = "";
    HANDLE hMpq1 = NULL;                // Handle of existing archive
    HANDLE hMpq2 = NULL;                // Handle of created archive 
    int nError = ERROR_SUCCESS;

    // Create a copy of self. This copy will become the new archive.
    if(nError == ERROR_SUCCESS)
    {
        if(bConvertExisting)
        {
            char szMyName[MAX_PATH];

            GetModuleFileName(NULL, szMyName, sizeof(szMyName));
            if(!CopyFile(szMyName, szMpqCopyName, FALSE))
                nError = GetLastError();
        }
        else
        {
            if(!DeleteFile(szMpqCopyName))
                nError = GetLastError();
            if(nError == ERROR_FILE_NOT_FOUND)
                nError = ERROR_SUCCESS;
        }
    }

    // Open the existing MPQ archive
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening %s ...\n", szMpqName);
        if(!SFileOpenArchive(szMpqName, 0, 0, &hMpq1))
            nError = GetLastError();
    }

    // Well, now create the MPQ archive
    if(nError == ERROR_SUCCESS)
    {
        printf("Creating %s ...\n", szMpqCopyName);
        if(!SFileCreateArchiveEx(szMpqCopyName, OPEN_ALWAYS, dwHashTableSize, &hMpq2))
            nError = GetLastError();
    }

    // Copy all files from one archive to another
    if(nError == ERROR_SUCCESS)
    {
        SFILE_FIND_DATA wf;
        HANDLE hFind = SFileFindFirstFile(hMpq1, "*.*", &wf, szListFile);
        BOOL bResult = TRUE;

        printf("Adding files\n");

        while(hFind != NULL && bResult == TRUE)
        {
//          DWORD dwFlags = MPQ_FILE_COMPRESS_MULTI | MPQ_FILE_ENCRYPTED | MPQ_FILE_FIXSEED;
            DWORD dwFlags = MPQ_FILE_COMPRESS_PKWARE | MPQ_FILE_ENCRYPTED; // For Hellfire

            SFileSetLocale(wf.lcLocale);
            ShowProcessedFile(wf.cFileName);

            // Create the local file name
            sprintf(szLocalFile, "%s\\%s", szWorkDir, GetPlainName(wf.cFileName));
            if((nError = ExtractFile(hMpq1, wf.cFileName, szLocalFile, &dwFlags)) != ERROR_SUCCESS)
            {
                printf("Failed to extract %s\n", wf.cFileName);
                break;
            }

            dwFlags &= ~MPQ_FILE_REPLACEEXISTING;
            if(!SFileAddFile(hMpq2, szLocalFile, wf.cFileName, dwFlags))
            {
                printf("Failed to add the file %s into archive\n", wf.cFileName);
                nError = GetLastError();
                if(nError == ERROR_ALREADY_EXISTS)
                    nError = ERROR_SUCCESS;
                else
                    break;
            }

            // Delete the added file
            DeleteFile(szLocalFile);

            // Find the next file
            bResult = SFileFindNextFile(hFind, &wf);
        }

        // Delete the extracted file in the case of an error
        if(nError != ERROR_SUCCESS)
            DeleteFile(szLocalFile);

        // Close the search handle
        if(hFind != NULL)
            SFileFindClose(hFind);
    }

    // Close both archives
    if(hMpq2 != NULL)
        SFileCloseArchive(hMpq2);
    if(hMpq1 != NULL)
        SFileCloseArchive(hMpq1);
    if(nError == ERROR_SUCCESS)
        printf("MPQ creating complete (No errors)\n");
    return nError;
}

static int TestMpqRenaming(const char * szMpqName, const char * szListFile)
{
    const char * szNewFileName = "FirstWaveFileInTheArchive.wav";
    SFILE_FIND_DATA wf;
    HANDLE hMpq = NULL;                 // Handle of existing archive
    HANDLE hFind = NULL;                // Search
    char szBuffer1[MPQ_BLOCK_SIZE * 2];
    char szBuffer2[MPQ_BLOCK_SIZE * 2];
    DWORD dwBytes1 = 0;
    DWORD dwBytes2 = 0; 
    int nError = ERROR_SUCCESS;

    printf("Renaming file in MPQ ...\n");
    memset(&wf, 0, sizeof(SFILE_FIND_DATA));

    // Open the archive
    if(nError == ERROR_SUCCESS)
    {
        clreol(); printf("Opening \"%s\" for renaming\r", szMpqName);
        if(!SFileCreateArchiveEx(szMpqName, OPEN_EXISTING, 0, &hMpq))
            nError = GetLastError();
    }

    // Open a file
    if(nError == ERROR_SUCCESS)
    {
        if((hFind = SFileFindFirstFile(hMpq, "*.gs", &wf, szListFile)) == NULL)
            nError = GetLastError();
    }

    // Read the first two blocks
    if(nError == ERROR_SUCCESS)
    {
        dwBytes1 = sizeof(szBuffer1);
        SFileSetLocale(wf.lcLocale);
        nError = ExtractBytes(hMpq, wf.cFileName, szBuffer1, dwBytes1);
    }

    if(hFind != NULL)
        SFileFindClose(hFind);

    // Rename the file in the archive
    if(nError == ERROR_SUCCESS)
    {
        clreol(); printf("Renaming \"%s\" ... \r", wf.cFileName);
        if(!SFileRenameFile(hMpq, wf.cFileName, szNewFileName))
            nError = GetLastError();
        if(nError == ERROR_ALREADY_EXISTS)
            printf("Cannot rename the file because it already exists\n");
    }

    // Try to read again
    if(nError == ERROR_SUCCESS)
    {
        dwBytes2 = sizeof(szBuffer2);
        nError = ExtractBytes(hMpq, szNewFileName, szBuffer2, dwBytes2);
    }

    // Compare the blocks
    if(nError == ERROR_SUCCESS)
    {
        clreol();
        if(!memcmp(szBuffer1, szBuffer2, dwBytes2))
            printf("Renaming file OK\n");
        else
            printf("Rename file failed\n");
    }

    // Close the archive handle
    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    
    // If the problem was "no files found", it is OK.
    if(nError == ERROR_NO_MORE_FILES || nError == ERROR_ALREADY_EXISTS)
        nError = ERROR_SUCCESS;
    return nError;
}

static int TestMpqDeleting(const char * szMpqName, const char * szListFile)
{
    char szLocalFile[MAX_PATH] = "";    // Local name of extracted file
    HANDLE hMpq  = NULL;
    DWORD dwFlags;
    int nError = ERROR_SUCCESS;

    // Open the archive
    if(nError == ERROR_SUCCESS)
    {
        printf("Opening \"%s\" for file deleting and compacting ...\n", szMpqName);
        if(!SFileCreateArchiveEx(szMpqName, OPEN_EXISTING, 0, &hMpq))
            nError = GetLastError();
    }

    if(nError == ERROR_SUCCESS)
    {
        SFILE_FIND_DATA wf;
        HANDLE hFind = SFileFindFirstFile(hMpq, "*.*", &wf, szListFile);
        HANDLE hFile = NULL;
        DWORD dwCount = 0;
        BOOL bResult = TRUE;

        printf("Removing files...\n");

        while(hFind != NULL && bResult == TRUE)
        {
            if((dwCount++ % 3) == 0)
            {
                SFileSetLocale(wf.lcLocale);
                ShowProcessedFile(wf.cFileName);

                // Extract the file from the archive
                sprintf(szLocalFile, "%s\\%s", szWorkDir, GetPlainName(wf.cFileName));
                if(ExtractFile(hMpq, wf.cFileName, szLocalFile, &dwFlags) != ERROR_SUCCESS)
                {
                    printf("Error extracting %s from the archive\n", wf.cFileName);
                    break;
                }

                // Remove the file from the archive
                if(!SFileRemoveFile(hMpq, wf.cFileName))
                {
                    printf("Error removing %s from the archive\n");
                    break;
                }

                // Try if can be open
                if(wf.lcLocale == LANG_NEUTRAL)
                {
                    if(SFileOpenFileEx(hMpq, wf.cFileName, 0, &hFile))
                    {
                        SFileCloseFile(hFile);
                        printf("Error : %s can be opened after it's been deleted !!!\n", wf.cFileName);
                        break;
                    }
                }

                // Add the file back
                if(!SFileAddFile(hMpq, szLocalFile, wf.cFileName, dwFlags))
                {
                    printf("Error : Failed to add the file %s.\n", wf.cFileName);
                    break;
                }

                // Try if can be open
                if(SFileOpenFileEx(hMpq, wf.cFileName, 0, &hFile))
                    SFileCloseFile(hFile);
                else
                    printf("Error : %s can not be opened after it's been added back !!!\n", wf.cFileName);

                // Delete the added file
                DeleteFile(szLocalFile);
            }
            bResult = SFileFindNextFile(hFind, &wf);
        }

        if(hFind != NULL)
            SFileFindClose(hFind);
    }

    // Compact the archive
    if(nError == ERROR_SUCCESS)
    {
        printf("Compacting archive ...\r");
        SFileSetCompactCallback(hMpq, CompactCB, NULL);
        if(!SFileCompactArchive(hMpq, szListFile))
            nError = GetLastError();
    }

    if(hMpq != NULL)
        SFileCloseArchive(hMpq);
    if(nError == ERROR_SUCCESS)
        printf("Deleting and compacting complete (No errors)\n");
    return nError;
}

static int TestSingleFile(const char * szMpqName, const char * szFileName)
{
    HANDLE hMpq = NULL;

    SFileOpenArchive(szMpqName, 0, 0, &hMpq);
    if(hMpq != NULL)
    {
        HANDLE hFile = NULL;

        SFileOpenFileEx(hMpq, szFileName, 0, &hFile);
        if(hFile != NULL)
        {
            DWORD dwTransferred;
            static char Buffer1[0x100000];
            static char Buffer2[0x100000];
            int nOutlength = sizeof(Buffer2);

            SFileReadFile(hFile, Buffer1, sizeof(Buffer1), &dwTransferred, NULL);
            SCompCompress(Buffer2, &nOutlength, Buffer1, dwTransferred, 0x10, 0, 0);
            SFileCloseFile(hFile);
        }

        SFileCloseArchive(hMpq);
    }

    return ERROR_SUCCESS;
}


//-----------------------------------------------------------------------------
// Main
// 
// The program must be run with two command line arguments
//
// Arg1 - The source MPQ name (for testing reading and file find)
// Arg2 - Listfile name
//
// To Valery: Run this test program against one of the MPQs that MpqEditor is not able
//            to open. Choose a list file at your will.
//

void main(int argc, char ** argv)
{
    char szMpqName[MAX_PATH] = "";
    char szMpqCopyName[MAX_PATH] = "";
    char szListFile[MAX_PATH] = "";
    char szTestFile[MAX_PATH] = "";
    DWORD dwHashTableSize = 0x8000;     // Size of the hash table.
    int nError = ERROR_SUCCESS;

    // Mix the random number generator
#ifndef _DEBUG
    srand(GetTickCount());
#endif

    // Use command line arguments, when some
    if(argc >= 2)
        strcpy(szMpqName, argv[1]);
    if(argc >= 3)
        strcpy(szListFile, argv[2]);
    if(argc >= 4)
        strcpy(szTestFile, argv[3]);

    // When not given or not exist, read them
    while(szMpqName[0] == 0 || _access(szMpqName, 0) != 0)
    {
        printf("Enter the name of MPQ archive: ");
        gets(szMpqName);
    }
    while(szListFile[0] == 0 || _access(szListFile, 0) != 0)
    {
        printf("Enter the name of file list: ");
        gets(szListFile);
    }
/*
    while(szTestFile[0] == 0 || _access(szTestFile, 0) != 0)
    {
        printf("Enter the name of the test file: ");
        gets(szTestFile);
    }
*/
    // Set the lowest priority to allow running in the background
    SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_BELOW_NORMAL);

    // Create the name of the MPQ copy
    if(nError == ERROR_SUCCESS)
    {
        char * szTemp = NULL;

        strcpy(szMpqCopyName, szMpqName);
        if((szTemp = strrchr(szMpqCopyName, '.')) != NULL)
            strcpy(szTemp, "_copy.mpq");
    }

    // Create the working directory
    if(nError == ERROR_SUCCESS)
    {
        if(!CreateDirectory(szWorkDir, NULL))
            nError = GetLastError();
        if(nError == ERROR_ALREADY_EXISTS)
            nError = ERROR_SUCCESS;
    }

    TestSingleFile(szMpqName, "TigonFemale.m2");

    // Test Diablo I save games
//  if(nError == ERROR_SUCCESS)
//      nError = TestMPQFromDiabloI(szMpqName, szMpqCopyName);

    // Test the compressions
//  if(nError == ERROR_SUCCESS)
//      nError = TestCompressions(szTestFile);

    // Test compresssion of the whole MPQ blocks
//  if(nError == ERROR_SUCCESS)
//      nError = TestBlockCompress(szTestFile);

//  if(nError == ERROR_SUCCESS)
//      nError = TestWAVECompression(szMpqCopyName, szTestFile);

    // Test to extract all files from the archive
    if(nError == ERROR_SUCCESS)
        nError = TestMpqFindFile(szMpqName, szListFile);

    // Test to extract all files from the archive
    if(nError == ERROR_SUCCESS)
        nError = TestMpqExtraction(szMpqName, szMpqName, szListFile, dwHashTableSize, TRUE);

    // Test the MPQ creation and file adding
    if(nError == ERROR_SUCCESS)
        nError = TestMpqCreation(szMpqName, szMpqCopyName, szListFile, dwHashTableSize, FALSE);

    // Test to extract all files from the archive copy
    if(nError == ERROR_SUCCESS)                                                           
        nError = TestMpqExtraction(szMpqName, szMpqCopyName, szListFile, dwHashTableSize, TRUE);

    // Test to rename a file in the archive copy
    if(nError == ERROR_SUCCESS)
        nError = TestMpqRenaming(szMpqCopyName, szListFile);

    // Test nameless access
    if(nError == ERROR_SUCCESS)
        nError = TestNamelessAccess(szMpqCopyName);

    // Test to delete every third file within the archive
    if(nError == ERROR_SUCCESS)
        nError = TestMpqDeleting(szMpqCopyName, szListFile);

    // Test check the extraction again
    if(nError == ERROR_SUCCESS)
        nError = TestMpqExtraction(szMpqName, szMpqCopyName, szListFile, dwHashTableSize);

    // Remove the working directory
    RemoveDirectory(szWorkDir);
    clreol();
    if(nError != ERROR_SUCCESS)
        printf("One or more errors occurred when testing StormLib\n");
    printf("Work complete. Press any key to exit ...\n");
    getch();
}
