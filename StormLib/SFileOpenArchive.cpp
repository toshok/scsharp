/*****************************************************************************/
/* SFileOpenArchive.cpp                       Copyright Ladislav Zezula 1999 */
/*                                                                           */
/* Author : Ladislav Zezula                                                  */
/* E-mail : ladik@zezula.net                                                 */
/* WWW    : www.zezula.net                                                   */
/*---------------------------------------------------------------------------*/
/*                       Archive functions of Storm.dll                      */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* xx.xx.xx  1.00  Lad  The first version of SFileOpenArchive.cpp            */
/* 19.11.03  1.01  Dan  Big endian handling                                  */
/*****************************************************************************/

#define __STORMLIB_SELF__
#include "StormLib.h"
#include "SCommon.h"

/*****************************************************************************/
/* Local functions                                                           */
/*****************************************************************************/

static BOOL IsAviFile(TMPQHeader * pHeader)
{
    DWORD * AviHdr = (DWORD *)pHeader;

    // Test for 'RIFF', 'AVI ' or 'LIST'
    return (AviHdr[0] == 'FFIR' && AviHdr[2] == ' IVA' && AviHdr[3] == 'TSIL');
}

// This function gets the right positions of the hash table and the block table.
static int RelocateMpqTablePositions(TMPQArchive * ha)
{
    TMPQHeader * pHeader = ha->pHeader;
    DWORD dwFileSize = GetFileSize(ha->hFile, NULL);

    // MPQs must have table positions in the range (0; dwFileSize)
    if(pHeader->dwHashTablePos + ha->dwMpqPos < dwFileSize &&
       pHeader->dwBlockTablePos + ha->dwMpqPos < dwFileSize)
    {
        pHeader->dwHashTablePos  += ha->dwMpqPos;
        pHeader->dwBlockTablePos += ha->dwMpqPos;
        return ERROR_SUCCESS;
    }

    // Otherwise the archive has bad format    
    return ERROR_BAD_FORMAT;
}


/*****************************************************************************/
/* Public functions                                                          */
/*****************************************************************************/

//-----------------------------------------------------------------------------
// SFileGetLocale and SFileSetLocale
// Set the locale for all neewly opened archives and files

LCID WINAPI SFileGetLocale()
{
    return lcLocale;
}

LCID WINAPI SFileSetLocale(LCID lcNewLocale)
{
    lcLocale = lcNewLocale;
    return lcLocale;
}

//-----------------------------------------------------------------------------
// SFileOpenArchiveEx (not a public function !!!)
//
//   szFileName - MPQ archive file name to open
//   dwPriority - When SFileOpenFileEx called, this contains the search priority for searched archives
//   dwFlags    - 
//   phMPQ      - Pointer to store open archive handle

BOOL SFileOpenArchiveEx(const char * szMpqName, DWORD dwPriority, DWORD /* dwFlags */, HANDLE * phMPQ, DWORD dwAccessMode)
{
    TMPQArchive * ha = NULL;            // Archive handle
    HANDLE hFile = INVALID_HANDLE_VALUE;// Opened archive file handle
    DWORD dwMaxBlockIndex = 0;          // Maximum value of block entry
    DWORD dwBlockTableSize;             // Block table size.
    DWORD dwTransferred;                // Number of bytes read
    DWORD dwBytes = 0;                  // Number of bytes to read
    int nError = ERROR_SUCCESS;   

    // Check the right parameters
    if(nError == ERROR_SUCCESS)
    {
        if(szMpqName == NULL || *szMpqName == 0 || phMPQ == NULL)
            nError = ERROR_INVALID_PARAMETER;
    }

    // Ensure that StormBuffer is allocated
    if(nError == ERROR_SUCCESS)
        nError = PrepareStormBuffer();

    // Open the MPQ archive file
    if(nError == ERROR_SUCCESS)
    {
        hFile = CreateFile(szMpqName, dwAccessMode, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
        if(hFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }
    
    // Allocate the MPQhandle
    if(nError == ERROR_SUCCESS)
    {
        if((ha = ALLOCMEM(TMPQArchive, 1)) == NULL)
            nError = ERROR_NOT_ENOUGH_MEMORY;
    }

    // Initialize handle structure and allocate structure for MPQ header
    if(nError == ERROR_SUCCESS)
    {
        memset(ha, 0, sizeof(TMPQArchive));
        strncpy(ha->szFileName, szMpqName, strlen(szMpqName));
        ha->hFile      = hFile;
        ha->bFromCD    = FALSE;
        ha->dwPriority = dwPriority;
        ha->pHeader    = &ha->Header;
        ha->pListFile  = NULL;
        hFile = INVALID_HANDLE_VALUE;
    }

    // Find the offset of MPQ header within the file
    if(nError == ERROR_SUCCESS)
    {
        for(;;)
        {
            ha->pHeader->dwID = 0;
            SetFilePointer(ha->hFile, ha->dwMpqPos, NULL, FILE_BEGIN);
            ReadFile(ha->hFile, ha->pHeader, sizeof(TMPQHeader), &dwTransferred, NULL);

            // Convert Header to LittleEndian
            CONVERTTMPQHEADERTOLITTLEENDIAN(ha->pHeader);

            // Special check : Some MPQs are actually AVI files, only with
            // changed extension.
            if(ha->dwMpqPos == 0 && IsAviFile(ha->pHeader))
            {
                nError = ERROR_AVI_FILE;
                break;
            }

            // If different number of bytes read, break the loop
            if(dwTransferred != sizeof(TMPQHeader))
            {
                nError = ERROR_BAD_FORMAT;
                break;
            }

            // Special value used by W3M Map Protector (Power)
            if(ha->pHeader->dwDataOffs == 0x6D9E4B86)
            {
                ha->dwFlags |= MPQ_FLAG_PROTECTED;
                ha->pHeader->dwDataOffs = sizeof(TMPQHeader);
            }

            // If valid signature has been found, break the loop
            if(ha->pHeader->dwID == ID_MPQ                              &&
               ha->pHeader->dwDataOffs == sizeof(TMPQHeader)            &&
               ha->pHeader->dwHashTablePos < ha->pHeader->dwArchiveSize &&
               ha->pHeader->dwBlockTablePos < ha->pHeader->dwArchiveSize)
                break;

            // Move to the next possible offset
            ha->dwMpqPos += 0x200;
        }
    }

    // Relocate tables position and check again
    if(nError == ERROR_SUCCESS)
    {
        ha->dwBlockSize = (0x200 << ha->pHeader->wBlockSize);
        nError = RelocateMpqTablePositions(ha);
    }

    // Allocate buffers
    if(nError == ERROR_SUCCESS)
    {
        //
        // Note that the block table should be as large as the hash table
        // (For later file additions).
        //
        // I have found a MPQ which has the block table larger than
        // the hash table. We should avoid buffer overruns caused by that.
        //
        dwBlockTableSize = max(ha->pHeader->dwHashTableSize, ha->pHeader->dwBlockTableSize);

        ha->pHashTable    = ALLOCMEM(TMPQHash, ha->pHeader->dwHashTableSize);
        ha->pBlockTable   = ALLOCMEM(TMPQBlock, dwBlockTableSize);
        ha->pbBlockBuffer = ALLOCMEM(BYTE, ha->dwBlockSize);

        if(!ha->pHashTable || !ha->pBlockTable || !ha->pbBlockBuffer)
            nError = ERROR_NOT_ENOUGH_MEMORY;
    }

    // Read the hash table into the buffer
    if(nError == ERROR_SUCCESS)
    {
        dwBytes = ha->pHeader->dwHashTableSize * sizeof(TMPQHash);

        SetFilePointer(ha->hFile, ha->pHeader->dwHashTablePos, NULL, FILE_BEGIN);
        ReadFile(ha->hFile, ha->pHashTable, dwBytes, &dwTransferred, NULL);

        // We have to convert every DWORD in ha->pHashTable from LittleEndian
        CONVERTBUFFERTOLITTLEENDIAN32BITS((DWORD *)ha->pHashTable, dwBytes / sizeof(DWORD));

        if(dwTransferred != dwBytes)
            nError = ERROR_FILE_CORRUPT;
    }

    // Decrypt hash table and check if it is correctly decrypted
    if(nError == ERROR_SUCCESS)
    {
        TMPQHash * pHashEnd = ha->pHashTable + ha->pHeader->dwHashTableSize;
        TMPQHash * pHash;

        DecryptHashTable((DWORD *)ha->pHashTable, (BYTE *)"(hash table)", (ha->pHeader->dwHashTableSize * 4));

        // Check hash table if is correctly decrypted
        for(pHash = ha->pHashTable; pHash < pHashEnd; pHash++)
        {
            // Some MPQs from World of Warcraft
            // have lcLocale set to 0x01000000.
            if(pHash->lcLocale != 0xFFFFFFFF && (pHash->lcLocale & 0x00FF0000) != 0)
            {
                nError = ERROR_BAD_FORMAT;
                break;
            }
            
            // Remember the highest block table entry
            if(pHash->dwBlockIndex < HASH_ENTRY_DELETED && pHash->dwBlockIndex > dwMaxBlockIndex)
                dwMaxBlockIndex = pHash->dwBlockIndex;
        }
    }

    // Now, read the block table
    if(nError == ERROR_SUCCESS)
    {
        dwBytes = ha->pHeader->dwBlockTableSize * sizeof(TMPQBlock);
        memset(ha->pBlockTable, 0, ha->pHeader->dwHashTableSize * sizeof(TMPQBlock));
        SetFilePointer(ha->hFile, ha->pHeader->dwBlockTablePos, NULL, FILE_BEGIN);
        ReadFile(ha->hFile, ha->pBlockTable, dwBytes, &dwTransferred, NULL);

        // We have to convert every DWORD in ha->block from LittleEndian
        CONVERTBUFFERTOLITTLEENDIAN32BITS((DWORD *)ha->pBlockTable, dwBytes / sizeof(DWORD));

        if(dwTransferred != dwBytes)
            nError = ERROR_FILE_CORRUPT;
    }

    // Decrypt block table.
    // Some MPQs don't have Decrypted block table, e.g. cracked Diablo version
    // We have to check if block table is already Decrypted
    if(nError == ERROR_SUCCESS)
    {
        TMPQBlock * pBlockEnd = ha->pBlockTable + dwMaxBlockIndex + 1;
        TMPQBlock * pBlock   = NULL;
        DWORD dwArchiveSize = ha->pHeader->dwArchiveSize + ha->dwMpqPos;

        if(ha->pHeader->dwDataOffs != ha->pBlockTable->dwFilePos)
            DecryptBlockTable((DWORD *)ha->pBlockTable, (BYTE *)"(block table)", (ha->pHeader->dwBlockTableSize * 4));

        for(pBlock = ha->pBlockTable; pBlock < pBlockEnd; pBlock++)
        {
            if(pBlock->dwFlags & MPQ_FILE_EXISTS)
            {
                if(pBlock->dwFilePos > dwArchiveSize || pBlock->dwCSize > dwArchiveSize)
                {
                    if((ha->dwFlags & MPQ_FLAG_PROTECTED) == 0)
                    {
                        nError = ERROR_BAD_FORMAT;
                        break;
                    }
                }
                pBlock->dwFilePos += ha->dwMpqPos;
            }
        }
    }

    // If open for writing, extract the temporary listfile
    // Note that the block table size must be less than the hash table size.
    if(nError == ERROR_SUCCESS)
        SListFileCreateListFile(ha);

    // Add the internal listfile
    if(nError == ERROR_SUCCESS)
        SFileAddListFile((HANDLE)ha, NULL);

    // Cleanup and exit
    if(nError != ERROR_SUCCESS)
    {
        FreeMPQArchive(ha);
        if(hFile != INVALID_HANDLE_VALUE)
            CloseHandle(hFile);
        SetLastError(nError);
    }
    else
    {
        if(pFirstOpen == NULL)
            pFirstOpen = ha;
    }
    *phMPQ = ha;
    return (nError == ERROR_SUCCESS);
}

BOOL WINAPI SFileOpenArchive(const char * szMpqName, DWORD dwPriority, DWORD dwFlags, HANDLE * phMPQ)
{
    return SFileOpenArchiveEx(szMpqName, dwPriority, dwFlags, phMPQ, GENERIC_READ);
}

//-----------------------------------------------------------------------------
// BOOL SFileCloseArchive(HANDLE hMPQ);
//

BOOL WINAPI SFileCloseArchive(HANDLE hMPQ)
{
    TMPQArchive * ha = (TMPQArchive *)hMPQ;
    
    if(!IsValidMpqHandle(ha))
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    if(ha->dwFlags & MPQ_FLAG_CHANGED)
    {
        SListFileSaveToMpq(ha);
        SaveMPQTables(ha);
    }
    FreeMPQArchive(ha);
    return TRUE;
}

