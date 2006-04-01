/*****************************************************************************/
/* SFileCreateArchiveEx.cpp               Copyright (c) Ladislav Zezula 2003 */
/*---------------------------------------------------------------------------*/
/* MPQ Editing functions                                                     */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 24.03.03  1.00  Lad  Splitted from SFileOpenArchive.cpp                   */
/*****************************************************************************/

#define __STORMLIB_SELF__
#include "StormLib.h"
#include "SCommon.h"

//-----------------------------------------------------------------------------
// Defines

#define DEFAULT_BLOCK_SIZE  3       // Default size of the block

//-----------------------------------------------------------------------------
// Local tables

static DWORD PowersOfTwo[] = 
{
               0x0000002, 0x0000004, 0x0000008,
    0x0000010, 0x0000020, 0x0000040, 0x0000080,
    0x0000100, 0x0000200, 0x0000400, 0x0000800,
    0x0001000, 0x0002000, 0x0004000, 0x0008000,
    0x0010000, 0x0020000, 0x0040000, 0x0080000,
    0x0000000
};

/*****************************************************************************/
/* Public functions                                                          */
/*****************************************************************************/

//-----------------------------------------------------------------------------
// Opens or creates a (new) MPQ archive.
//
//  szMpqName - Name of the archive to be created.
//
//  dwCreationDisposition:
//   Value          Archive exists         Archive doesn't exist
//   ----------     ---------------------  ---------------------
//   CREATE_NEW     Fails                  Creates new archive
//   CREATE_ALWAYS  Overwrites existing    Creates new archive
//   OPEN_EXISTING  Opens the archive      Fails
//   OPEN_ALWAYS    Opens the archive      Creates new archive
//   
// dwHashTableSize - Size of the hash table (only if creating a new archive).
//        Must be between 2^4 (= 16) and 2^18 (= 262 144)
// phMpq - Receives handle to the archive

BOOL WINAPI SFileCreateArchiveEx(const char * szMpqName, DWORD dwCreationDisposition, DWORD dwHashTableSize, HANDLE * phMPQ)
{
    TMPQArchive * ha = NULL;                // MPQ archive handle
    HANDLE hFile    = INVALID_HANDLE_VALUE; // File handle
    DWORD dwMpqPos = 0;                     // Position of MPQ header within the file
    DWORD dwTransferred = 0;                    // Number of bytes written into the archive
    BOOL bFileExists = FALSE;
    int nIndex = 0;
    int nError = ERROR_SUCCESS;

    // Pre-initialize the result value
    if(phMPQ != NULL)
        *phMPQ = NULL;

    // Check the parameters, if they are valid
    if(szMpqName == NULL || *szMpqName == 0 || phMPQ == NULL)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    // Check the value of dwCreationDisposition against file existence
    bFileExists = (GetFileAttributes(szMpqName) != 0xFFFFFFFF);

    // If the file exists and open required, do it.
    if(bFileExists && (dwCreationDisposition == OPEN_EXISTING || dwCreationDisposition == OPEN_ALWAYS))
    {
        // Try to open the archive normal way. If it fails, it means that
        // the file exist, but it is not a MPQ archive.
        if(SFileOpenArchiveEx(szMpqName, 0, 0, phMPQ, GENERIC_READ | GENERIC_WRITE))
            return TRUE;
    }

    // Two error cases
    if(dwCreationDisposition == CREATE_NEW && bFileExists)
    {
        SetLastError(ERROR_ALREADY_EXISTS);
        return FALSE;
    }
    if(dwCreationDisposition == OPEN_EXISTING && bFileExists == FALSE)
    {
        SetLastError(ERROR_FILE_NOT_FOUND);
        return FALSE;
    }

    // At this point, we have to create the archive. If the file exists,
    // we will convert it to MPQ archive.
    // Check the value of hash table size. It has to be a power of two
    // and must be between HASH_TABLE_SIZE_MIN and HASH_TABLE_SIZE_MAX
    if(dwHashTableSize < HASH_TABLE_SIZE_MIN)
        dwHashTableSize = HASH_TABLE_SIZE_MIN;
    if(dwHashTableSize > HASH_TABLE_SIZE_MAX)
        dwHashTableSize = HASH_TABLE_SIZE_MAX;
    
    // Round the hash table size up to the nearest power of two
    for(nIndex = 0; PowersOfTwo[nIndex] != 0; nIndex++)
    {
        if(dwHashTableSize <= PowersOfTwo[nIndex])
        {
            dwHashTableSize = PowersOfTwo[nIndex];
            break;
        }
    }

    // Prepare the buffer for decryption engine
    if(nError == ERROR_SUCCESS)
        nError = PrepareStormBuffer();

    // Get the position where the MPQ header will begin.
    if(nError == ERROR_SUCCESS)
    {
        hFile = CreateFile(szMpqName, GENERIC_READ | GENERIC_WRITE, 0, NULL, dwCreationDisposition, 0, NULL);
        if(hFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }

    // Retrieve the file size and round it up to 0x200 bytes
    if(nError == ERROR_SUCCESS)
    {
        dwMpqPos = ((GetFileSize(hFile, NULL) + 0x1FF) & 0xFFFFFE00);
        if(SetFilePointer(hFile, dwMpqPos, NULL, FILE_BEGIN) == 0xFFFFFFFF)
            nError = GetLastError();
    }

    // Set the new end of the file to the MPQ header position
    if(nError == ERROR_SUCCESS)
    {
        if(!SetEndOfFile(hFile))
            nError = GetLastError();
    }

    // Create the archive handle
    if(nError == ERROR_SUCCESS)
    {
        if((ha = ALLOCMEM(TMPQArchive, 1)) == NULL)
            nError = ERROR_NOT_ENOUGH_MEMORY;
    }

    // Fill the MPQ archive handle structure and create the header,
    // block buffer, hash table and block table
    if(nError == ERROR_SUCCESS)
    {
        memset(ha, 0, sizeof(TMPQArchive));
        strcpy(ha->szFileName, szMpqName);
        ha->hFile         = hFile;
        ha->dwBlockSize   = 0x200 << DEFAULT_BLOCK_SIZE;
        ha->dwMpqPos      = dwMpqPos;
        ha->dwFilePos     = dwMpqPos;
        ha->pHeader       = &ha->Header;
        ha->pHashTable    = ALLOCMEM(TMPQHash, dwHashTableSize);
        ha->pBlockTable   = ALLOCMEM(TMPQBlock, dwHashTableSize);
        ha->pbBlockBuffer = ALLOCMEM(BYTE, ha->dwBlockSize);
        ha->pListFile     = NULL;
        ha->dwFlags      |= MPQ_FLAG_CHANGED;

        if(!ha->pHashTable || !ha->pBlockTable || !ha->pbBlockBuffer)
            nError = GetLastError();
        hFile = INVALID_HANDLE_VALUE;
    }

    // Fill the MPQ header and all buffers
    if(nError == ERROR_SUCCESS)
    {
        memset(ha->pHeader, 0, sizeof(TMPQHeader));
        ha->pHeader->dwID            = ID_MPQ;
        ha->pHeader->dwDataOffs      = sizeof(TMPQHeader);
        ha->pHeader->wBlockSize      = 3;        // 0x1000 bytes per block
        ha->pHeader->dwArchiveSize   = sizeof(TMPQHeader) + dwHashTableSize * sizeof(TMPQHash);
        ha->pHeader->dwHashTableSize = dwHashTableSize;
        ha->pHeader->dwHashTablePos  = ha->dwMpqPos + sizeof(TMPQHeader);
        ha->pHeader->dwBlockTablePos = ha->dwMpqPos + sizeof(TMPQHeader) + dwHashTableSize * sizeof(TMPQHash);

        memset(ha->pBlockTable, 0, sizeof(TMPQBlock) * dwHashTableSize);
        memset(ha->pHashTable, 0xFF, sizeof(TMPQHash) * dwHashTableSize);
    }

    // Write the MPQ header to the file
    if(nError == ERROR_SUCCESS)
    {
        ha->dwFilePos = ha->dwMpqPos + sizeof(TMPQHeader);
        CONVERTTMPQHEADERTOLITTLEENDIAN(ha->pHeader);
        if(!WriteFile(ha->hFile, ha->pHeader, sizeof(TMPQHeader), &dwTransferred, NULL))
            nError = GetLastError();
        CONVERTTMPQHEADERTOLITTLEENDIAN(ha->pHeader);
    }

    // Create the internal listfile
    if(nError == ERROR_SUCCESS)
        nError = SListFileCreateListFile(ha);

    // Try to add the internal listfile
    if(nError == ERROR_SUCCESS)
        SFileAddListFile((HANDLE)ha, NULL);

    // Cleanup : If an error, delete all buffers and return
    if(nError != ERROR_SUCCESS)
    {
        FreeMPQArchive(ha);
        if(hFile != INVALID_HANDLE_VALUE)
            CloseHandle(hFile);
        SetLastError(nError);
    }
    
    // Return the values
    *phMPQ = (HANDLE)ha;
    return (nError == ERROR_SUCCESS);
}

//-----------------------------------------------------------------------------
// Changes locale ID of a file

BOOL WINAPI SFileSetFileLocale(HANDLE hFile, LCID lcNewLocale)
{
    TMPQFile * hf = (TMPQFile *)hFile;

    // Invalid handle => do nothing
    if(IsValidFileHandle(hf) == FALSE || IsValidMpqHandle(hf->ha) == FALSE)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    // If the file has not been open for writing, do nothing.
    if(hf->ha->pListFile == NULL)
        return ERROR_ACCESS_DENIED;

    hf->pHash->lcLocale = lcNewLocale;
    hf->ha->dwFlags |= MPQ_FLAG_CHANGED;
    return TRUE;
}

//-----------------------------------------------------------------------------
// Adds a file into the archive

BOOL WINAPI SFileAddFileEx(HANDLE hMPQ, const char * szFileName, const char * szArchivedName, DWORD dwFlags, DWORD dwQuality, int nFileType)
{
    TMPQArchive * ha = (TMPQArchive *)hMPQ;
    HANDLE hFile = INVALID_HANDLE_VALUE;
    BOOL   bReplaced = FALSE;          // TRUE if replacing file in the archive
    int    nError = ERROR_SUCCESS;

    if(nError == ERROR_SUCCESS)
    {
        // Check valid parameters
        if(IsValidMpqHandle(ha) == FALSE || szFileName == NULL || *szFileName == 0 || szArchivedName == NULL || *szArchivedName == 0)
            nError = ERROR_INVALID_PARAMETER;

        // Check the values of dwFlags
        if((dwFlags & MPQ_FILE_COMPRESS_PKWARE) && (dwFlags & MPQ_FILE_COMPRESS_MULTI))
            nError = ERROR_INVALID_PARAMETER;
    }

    // If anyone is trying to add listfile, and the archive already has a listfile,
    // deny the operation, but return success.
    if(nError == ERROR_SUCCESS)
    {
        if(ha->pListFile != NULL && !stricmp(szFileName, LISTFILE_NAME))
            return ERROR_SUCCESS;
    }

    // Open added file
    if(nError == ERROR_SUCCESS)
    {
        hFile = CreateFile(szFileName, GENERIC_READ, FILE_SHARE_READ, 0, OPEN_EXISTING, 0, NULL);
        if(hFile == INVALID_HANDLE_VALUE)
            nError = GetLastError();
    }

    if(nError == ERROR_SUCCESS)
        nError = AddFileToArchive(ha, hFile, szArchivedName, dwFlags, dwQuality, nFileType, &bReplaced);

    // Add the file into listfile also
    if(nError == ERROR_SUCCESS && bReplaced == FALSE)
        nError = SListFileAddNode(ha, szArchivedName);

    // Cleanup and exit
    if(hFile != INVALID_HANDLE_VALUE)
        CloseHandle(hFile);
    if(nError != ERROR_SUCCESS)
        SetLastError(nError);
    return (nError == ERROR_SUCCESS);
}
                                                                                                                                 
// Adds a data file into the archive
BOOL WINAPI SFileAddFile(HANDLE hMPQ, const char * szFileName, const char * szArchivedName, DWORD dwFlags)
{
    return SFileAddFileEx(hMPQ, szFileName, szArchivedName, dwFlags, 0, SFILE_TYPE_DATA);
}

// Adds a WAVE file into the archive
BOOL WINAPI SFileAddWave(HANDLE hMPQ, const char * szFileName, const char * szArchivedName, DWORD dwFlags, DWORD dwQuality)
{
    return SFileAddFileEx(hMPQ, szFileName, szArchivedName, dwFlags, dwQuality, SFILE_TYPE_WAVE);
}

//-----------------------------------------------------------------------------
// BOOL SFileRemoveFile(HANDLE hMPQ, char * szFileName)
//
// This function removes a file from the archive. The file content
// remains there, only the entries in the hash table and in the block
// table are updated. 

BOOL WINAPI SFileRemoveFile(HANDLE hMPQ, const char * szFileName)
{
    TMPQArchive * ha = (TMPQArchive *)hMPQ;
    TMPQHash    * pFileHash = NULL;     // Hash entry of deleted file
    TMPQBlock   * pFileBlock = NULL;    // Block entry of deleted file
    DWORD dwBlockIndex = 0;
    int nError = ERROR_SUCCESS;

    // Check the parameters
    if(nError == ERROR_SUCCESS)
    {
        if(IsValidMpqHandle(ha) == FALSE)
            nError = ERROR_INVALID_PARAMETER;
        if((DWORD)szFileName > 0x00010000 && *szFileName == 0)
            nError = ERROR_INVALID_PARAMETER;
    }

    // Do not allow to remove listfile
    if(nError == ERROR_SUCCESS)
    {
        if((DWORD)szFileName > 0x00010000 && !stricmp(szFileName, LISTFILE_NAME))
            nError = ERROR_ACCESS_DENIED;
    }

    // Get hash entry belonging to this file
    if(nError == ERROR_SUCCESS)
    {
        if((pFileHash = GetHashEntryEx(ha, (char *)szFileName, lcLocale)) == NULL)
            nError = ERROR_FILE_NOT_FOUND;
    }

    // If index was not found, or is greater than number of files, exit.
    if(nError == ERROR_SUCCESS)
    {
        if((dwBlockIndex = pFileHash->dwBlockIndex) > ha->pHeader->dwBlockTableSize)
            nError = ERROR_FILE_NOT_FOUND;
    }

    // Get block and test if the file is not already deleted
    if(nError == ERROR_SUCCESS)
    {
        pFileBlock = ha->pBlockTable + dwBlockIndex;
        if((pFileBlock->dwFlags & MPQ_FILE_EXISTS) == 0)
            nError = ERROR_FILE_NOT_FOUND;
    }

    // Now invalidate the block entry and the hash entry. Do not make any
    // relocations and file copying, use SFileCompactArchive for it.
    if(nError == ERROR_SUCCESS)
    {
//      pFileBlock->dwFSize     = 0;
        pFileBlock->dwFlags     = 0;
        pFileHash->dwName1      = 0xFFFFFFFF;
        pFileHash->dwName2      = 0xFFFFFFFF;
        pFileHash->lcLocale     = 0xFFFFFFFF;
        pFileHash->dwBlockIndex = HASH_ENTRY_DELETED;

        // Update MPQ archive
        ha->dwFlags |= MPQ_FLAG_CHANGED;
    }

    // Remove the file from the list file
    if(nError == ERROR_SUCCESS && lcLocale == LANG_NEUTRAL)
        nError = SListFileRemoveNode(ha, szFileName);

    // Resolve error and exit
    if(nError != ERROR_SUCCESS)
        SetLastError(nError);
    return (nError == ERROR_SUCCESS);
}

// Renames the file within the archive.
BOOL WINAPI SFileRenameFile(HANDLE hMPQ, const char * szFileName, const char * szNewFileName)
{
    TMPQArchive * ha = (TMPQArchive *)hMPQ;
    TMPQHash * pOldHash = NULL;         // Hash entry for the original file
    TMPQHash * pNewHash = NULL;         // Hash entry for the renamed file
    DWORD dwBlockIndex = 0;
    int nError = ERROR_SUCCESS;

    // Test the valid parameters
    if(nError == ERROR_SUCCESS)
    {
        if(hMPQ == NULL || szNewFileName == NULL || *szNewFileName == 0)
            nError = ERROR_INVALID_PARAMETER;
        if((DWORD)szFileName > 0x00010000 && *szFileName == 0)
            nError = ERROR_INVALID_PARAMETER;
    }

    // Do not allow to rename listfile
    if(nError == ERROR_SUCCESS)
    {
        if((DWORD)szFileName > 0x00010000 && !stricmp(szFileName, LISTFILE_NAME))
            nError = ERROR_ACCESS_DENIED;
    }

    // Test if the file already exists in the archive
    if(nError == ERROR_SUCCESS)
    {
        if((pNewHash = GetHashEntryEx(ha, szNewFileName, lcLocale)) != NULL)
            nError = ERROR_ALREADY_EXISTS;
    }

    // Get the hash table entry for the original file
    if(nError == ERROR_SUCCESS)
    {
        if((pOldHash = GetHashEntryEx(ha, szFileName, lcLocale)) == NULL)
            nError = ERROR_FILE_NOT_FOUND;
    }

    // Get the hash table entry for the renamed file
    if(nError == ERROR_SUCCESS)
    {
        // Save block table index and remove the hash table entry
        dwBlockIndex = pOldHash->dwBlockIndex;
        pOldHash->dwName1      = 0xFFFFFFFF;
        pOldHash->dwName2      = 0xFFFFFFFF;
        pOldHash->lcLocale     = 0xFFFFFFFF;
        pOldHash->dwBlockIndex = HASH_ENTRY_DELETED;

        if((pNewHash = FindFreeHashEntry(ha, szNewFileName)) == NULL)
            nError = ERROR_CAN_NOT_COMPLETE;
    }

    // Save the block index and clear the hash entry
    if(nError == ERROR_SUCCESS)
    {
        // Copy the block table index
        pNewHash->dwBlockIndex = dwBlockIndex;
        ha->dwFlags |= MPQ_FLAG_CHANGED;
    }

    // Rename the file in the list file
    if(nError == ERROR_SUCCESS)
        nError = SListFileRenameNode(ha, szFileName, szNewFileName);

    // Resolve error and return
    if(nError != ERROR_SUCCESS)
        SetLastError(nError);
    return (nError == ERROR_SUCCESS);
}

