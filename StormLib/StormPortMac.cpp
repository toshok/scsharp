/********************************************************************
*
* Description:  implementation for StormLib - Macintosh port
*        
*   these are function wraps to execute Windows API calls
*   as native Macintosh file calls (open/close/read/write/...)
*
* Derived from Marko Friedemann <marko.friedemann@bmx-chemnitz.de>
* StormPort.cpp for Linux
*
* Author: Daniel Chiaramello <daniel@chiaramello.net>
*
* Carbonized by: Sam Wilkins <swilkins1337@gmail.com>
*
********************************************************************/

#ifndef _WIN32
#include "StormPort.h"
#include "StormLib.h"

// FUNCTIONS EXTRACTED FROM MOREFILE PACKAGE!!!
// FEEL FREE TO REMOVE THEM AND TO ADD THE ORIGINAL ONES!

/*****************************************************************************
* BEGIN OF MOREFILES COPY-PASTE
*****************************************************************************/

#ifdef    __USEPRAGMAINTERNAL
    #ifdef __MWERKS__
        #pragma internal on
    #endif
#endif

static OSErr FSGetFullPath(const FSRef *ref, UInt8 *fullPath, UInt32 fullPathLength)
{
#ifdef __MACH__
    OSErr       result;

    result = FSRefMakePath(ref, fullPath, fullPathLength);
    
    return result;
#else
    OSErr           result;
    FSCatalogInfo   catInfo;
    HFSUniStr255    name;
    FSRef           parentRef, tempRef;
 
    result = FSGetCatalogInfo(ref, kFSCatInfoParentDirID | kFSCatInfoVolume | kFSCatInfoNodeFlags, 
                              &catInfo, &name, NULL, &parentRef);
    if (result == noErr)
    {
        if (catInfo.parentDirID == fsRtParID)
        {
            /* The object is a volume */
            
            /* Add a colon to make it a full pathname */
            CFStringRef nameCFString = CFStringCreateWithCharacters(NULL, name.unicode, name.length);
            CFStringGetCString(nameCFString, (char *)fullPath, fullPathLength, kCFStringEncodingUTF8);
            strcat((char *)fullPath, ":");
            
            /* We're done */
            CFRelease(nameCFString);
        }
        else
        {
            /* The object isn't a volume */
            
            // Allow file/directory name at end of path to not exist.
            CFStringRef nameCFString = CFStringCreateWithCharacters(NULL, name.unicode, name.length);

            /* if the object is a directory, append a colon so full pathname ends with colon */
            if (catInfo.nodeFlags & kFSNodeIsDirectoryBit != 0)
            {
                CFStringGetCString(nameCFString, (char *)fullPath, fullPathLength, kCFStringEncodingUTF8);
                strcat((char *)fullPath, ":");
            }
            
            /* Put the object name in first */
            CFStringGetCString(nameCFString, (char *)fullPath, fullPathLength, kCFStringEncodingUTF8);
            tempRef = *ref;
            char tempPath[MAX_PATH];
            do    /* loop until we have an error or find the root directory */
            {
                tempRef = parentRef;    // Get the next info from the parent FSRef
                result = FSGetCatalogInfo(&tempRef, kFSCatInfoParentDirID, &catInfo, &name, NULL, &parentRef);
                if (result == noErr)
                {
                    /* Append colon to directory name */
                    CFStringGetCString(nameCFString, tempPath, MAX_PATH, kCFStringEncodingUTF8);
                    strcat(tempPath, ":");
                    /* Add directory name to beginning of fullPath */
                    strcat(tempPath, (char *)fullPath);
                    strcpy((char *)fullPath, tempPath);
                }
            } while ((result == noErr) && (catInfo.parentDirID != fsRtParID));

            CFRelease(nameCFString);
        }
    }
    
    if (result != noErr)
    {
        *fullPath = NULL;
    }
    
    return result;
#endif
}

static OSErr FSLocationFromFullPath(const void *fullPath, FSRef *ref)
{
#ifdef __MACH__
    OSErr       result;
    
    result = FSPathMakeRef((UInt8 *)fullPath, ref, NULL); // Create an FSRef from the path
    return result;
#else
    OSErr       result;
    FSSpec      tempSpec;
    
    result = FSMakeFSSpec(0, 0, (ConstStr255Param)fullPath, &tempSpec);
    if (result == noErr)
    {
        result = FSpMakeFSRef(&tempSpec, ref);
    }

    return result;
#endif    
}

/*****************************************************************************/

/*****************************************************************************/

static OSErr FSCreateCompat(const FSRef *parentRef, OSType creator, OSType fileType, const UniChar *fileName, 
                            UniCharCount nameLength, FSRef *ref)
{
    FSCatalogInfo theCatInfo;
    OSErr theErr;
    ((FileInfo *)&theCatInfo.finderInfo)->fileCreator = creator;
    ((FileInfo *)&theCatInfo.finderInfo)->fileType = fileType;
    ((FileInfo *)&theCatInfo.finderInfo)->finderFlags = 0;
    SetPt(&((FileInfo *)&theCatInfo.finderInfo)->location, 0, 0);
    ((FileInfo *)&theCatInfo.finderInfo)->reservedField = 0;
        
    theErr = FSCreateFileUnicode(parentRef, nameLength, fileName, kFSCatInfoFinderInfo, &theCatInfo, ref, NULL);
    return theErr;
}


/*****************************************************************************/

static OSErr FSOpenDFCompat(FSRef *ref, char permission, short *refNum)
{
    HFSUniStr255 forkName;
    OSErr theErr;
    Boolean isFolder, wasChanged;
    
    theErr = FSResolveAliasFile(ref, true, &isFolder, &wasChanged);
    if (theErr != noErr)
        return theErr;
    
    FSGetDataForkName(&forkName);
    theErr = FSOpenFork(ref, forkName.length, forkName.unicode, permission, refNum);
    return theErr;
}

/*****************************************************************************
* END OF MOREFILES COPY-PASTE
*****************************************************************************/

#pragma mark -

int globalerr;

/********************************************************************
*    SwapLong
********************************************************************/
unsigned long SwapLong(unsigned long data)
{
    UInt32 data2;

    data2  = (data >> 24) & 0x000000FF;
    data2 |= (data >> 8)  & 0x0000FF00;
    data2 |= (data << 8)  & 0x00FF0000;
    data2 |= (data << 24) & 0xFF000000;

    return data2;
}

/********************************************************************
*    SwapShort
********************************************************************/
unsigned short SwapShort(unsigned short data)
{
    return (data >> 8) | (data << 8);
}

/********************************************************************
*    ConvertUnsignedLongBuffer
********************************************************************/
void ConvertUnsignedLongBuffer(unsigned long *buffer, unsigned long nbLongs)
{
    while (nbLongs-- > 0)
    {
        *buffer = SwapLong(*buffer);
        buffer++;
    }
}

/********************************************************************
*    ConvertUnsignedShortBuffer
********************************************************************/
void ConvertUnsignedShortBuffer(unsigned short *buffer, unsigned long nbShorts)
{
    while (nbShorts-- > 0)
    {
        *buffer = SwapShort(*buffer);
        buffer++;
    }
}

/********************************************************************
*    ConvertTMPQHeader
********************************************************************/
void ConvertTMPQHeader(void *header)
{
    TMPQHeader  *theHeader = (TMPQHeader *)header;
    
    theHeader->dwID = SwapLong(theHeader->dwID);
    theHeader->dwDataOffs = SwapLong(theHeader->dwDataOffs);
    theHeader->dwArchiveSize = SwapLong(theHeader->dwArchiveSize);
    theHeader->wOffs0C = SwapShort(theHeader->wOffs0C);
    theHeader->wBlockSize = SwapShort(theHeader->wBlockSize);
    theHeader->dwHashTablePos = SwapLong(theHeader->dwHashTablePos);
    theHeader->dwBlockTablePos = SwapLong(theHeader->dwBlockTablePos);
    theHeader->dwHashTableSize = SwapLong(theHeader->dwHashTableSize);
    theHeader->dwBlockTableSize = SwapLong(theHeader->dwBlockTableSize);
}

#pragma mark -

/********************************************************************
*    SetLastError
********************************************************************/
void SetLastError(int err)
{
    globalerr = err;
}

/********************************************************************
*    GetLastError
********************************************************************/
int GetLastError()
{
    return globalerr;
}

/********************************************************************
*    ErrString
********************************************************************/
char *ErrString(int err)
{
    switch (err) 
    {
        case ERROR_INVALID_FUNCTION:
            return "function not implemented";
        case ERROR_FILE_NOT_FOUND:
            return "file not found";
        case ERROR_ACCESS_DENIED:
            return "access denied";
        case ERROR_NOT_ENOUGH_MEMORY:
            return "not enough memory";
        case ERROR_BAD_FORMAT:
            return "bad format";
        case ERROR_NO_MORE_FILES:
            return "no more files";
        case ERROR_HANDLE_EOF:
            return "access beyound EOF";
        case ERROR_HANDLE_DISK_FULL:
            return "no space left on device";
        case ERROR_INVALID_PARAMETER:
            return "invalid parameter";
        case ERROR_DISK_FULL:
            return "no space left on device";
        case ERROR_ALREADY_EXISTS:
            return "file exists";
        case ERROR_CAN_NOT_COMPLETE:
            return "operation cannot be completed";
        case ERROR_INSUFFICIENT_BUFFER:
            return "insufficient buffer";
        default:
            return "unknown error";
    }
}

#pragma mark -

/********************************************************************
*    GetTempPath - returns a '/' or ':'-terminated path
*        szTempLength: length for path
*        szTemp: file path
********************************************************************/
void GetTempPath(DWORD szTempLength, char * szTemp)  // I think I'll change this to use FSRefs.
{
    FSRef   theFSRef;
    OSErr theErr = FSFindFolder(kOnAppropriateDisk, kTemporaryFolderType, kCreateFolder, &theFSRef);
    if (theErr == noErr)
    {
        theErr = FSGetFullPath(&theFSRef, (UInt8 *)szTemp, MAX_PATH);
        if (theErr != noErr)
            szTemp[0] = '\0';
    }
    else
        szTemp[0] = '\0';
    #ifdef __MACH__
    strcat(szTemp, "/");
    #else
    strcat(szTemp, ":");
    #endif
}

/********************************************************************
*    GetTempFileName
*        lpTempFolderPath: the temporary folder path, terminated by "/"
*        lpFileName: a file name base
*        something: unknown
*        szLFName: the final path, built from the path, the file name and a random pattern
********************************************************************/
void GetTempFileName(const char * lpTempFolderPath, const char * lpFileName, DWORD something, char * szLFName)
{
#pragma unused (something)
    char    tmp[2] = "A";

    while (true)
    {
        HANDLE  fHandle;

        strcpy(szLFName, lpTempFolderPath);
        strcat(szLFName, lpFileName);
        strcat(szLFName, tmp);
        
        if ((fHandle = CreateFile(szLFName, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, 0)) == INVALID_HANDLE_VALUE)
            // OK we found it!
            break;
        CloseHandle(fHandle);
        tmp[0]++;
    }
}

/********************************************************************
*    DeleteFile
*        lpFileName: file path
********************************************************************/
BOOL DeleteFile(const char * lpFileName)
{
    OSErr   theErr;
    FSRef   theFileRef;
    
    theErr = FSLocationFromFullPath(lpFileName, &theFileRef);
    if (theErr != noErr)
        return FALSE;
    
    theErr = FSDeleteObject(&theFileRef);
    return theErr == noErr;
}

/********************************************************************
*    MoveFile
*        lpFromFileName: old file path
*        lpToFileName: new file path
********************************************************************/
BOOL MoveFile(const char * lpFromFileName, const char * lpToFileName)
{
    OSErr theErr;
    FSRef fromFileRef;
    FSRef toFileRef;
    FSRef parentFolderRef;
    
    // Get the path to the old file
    theErr = FSLocationFromFullPath(lpFromFileName, &fromFileRef);
    if (theErr != noErr)
        return FALSE;
    
    // Get the path to the new folder for the file
    char folderName[strlen(lpToFileName)];
    CFStringRef folderPathCFString = CFStringCreateWithCString(NULL, lpToFileName, kCFStringEncodingUTF8);
#ifdef __MACH__
    CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, folderPathCFString, kCFURLPOSIXPathStyle, false);
#else
    CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, folderPathCFString, kCFURLHFSPathStyle, false);
#endif
    CFURLRef folderURL = CFURLCreateCopyDeletingLastPathComponent(NULL, fileURL);
    CFURLGetFileSystemRepresentation(folderURL, true, (UInt8 *)folderName, strlen(lpToFileName));
    theErr = FSLocationFromFullPath(folderName, &parentFolderRef);
    CFRelease(fileURL);
    CFRelease(folderURL);
    CFRelease(folderPathCFString);
    
    // Move the old file
    theErr = FSMoveObject(&fromFileRef, &parentFolderRef, &toFileRef);
    if (theErr != noErr)
        return false;
    
    // Get a CFString for the new file name
    CFStringRef newFileNameCFString = CFStringCreateWithCString(NULL, lpToFileName, kCFStringEncodingUTF8);
#ifdef __MACH__
    fileURL = CFURLCreateWithFileSystemPath(NULL, newFileNameCFString, kCFURLPOSIXPathStyle, false);
#else
    fileURL = CFURLCreateWithFileSystemPath(NULL, newFileNameCFString, kCFURLHFSPathStyle, false);
#endif
    CFRelease(newFileNameCFString);
    newFileNameCFString = CFURLCopyLastPathComponent(fileURL);
    CFRelease(fileURL);
    
    // Convert CFString to Unicode and rename the file
    const UniChar *unicodeFileName;
    if (!(unicodeFileName = CFStringGetCharactersPtr(newFileNameCFString)))
    {
        CFRelease(newFileNameCFString);
        return FALSE;
    }
    theErr = FSRenameUnicode(&toFileRef, CFStringGetLength(newFileNameCFString), unicodeFileName, 
                             kTextEncodingUnknown, NULL);
    if (theErr != noErr)
    {
        CFRelease(newFileNameCFString);
        return FALSE;
    }
    
    CFRelease(newFileNameCFString);
    
    return TRUE;
}

/********************************************************************
*    CreateFile
*        ulMode: GENERIC_READ | GENERIC_WRITE
*        ulSharing: FILE_SHARE_READ
*        pSecAttrib: NULL
*        ulCreation: OPEN_EXISTING, OPEN_ALWAYS, CREATE_NEW
*        ulFlags: 0
*        hFile: NULL
********************************************************************/
HANDLE CreateFile(  const char *sFileName,          /* file name */
                    DWORD ulMode,                   /* access mode */
                    DWORD ulSharing,                /* share mode */
                    void *pSecAttrib,               /* SD */
                    DWORD ulCreation,               /* how to create */
                    DWORD ulFlags,                  /* file attributes */
                    HANDLE hFile    )               /* handle to template file */
{
#pragma unused (ulSharing, pSecAttrib, ulFlags, hFile)

    OSErr   theErr;
    FSRef   theFileRef;
    FSRef   theParentRef;
    short   fileRef;
    char    permission;
    static OSType   gCreator;
    static OSType   gType;
    
    theErr = FSLocationFromFullPath(sFileName, &theFileRef);
    if (theErr == fnfErr)
    {   // Create the FSRef for the parent directory.
        memset(&theFileRef, 0, sizeof(FSRef));
        UInt8 folderName[1024];
        CFStringRef folderPathCFString = CFStringCreateWithCString(NULL, sFileName, kCFStringEncodingUTF8);
        #ifdef __MACH__
        CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, folderPathCFString, kCFURLPOSIXPathStyle, false);
        #else
        CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, folderPathCFString, kCFURLHFSPathStyle, false);
        #endif
        CFURLRef folderURL = CFURLCreateCopyDeletingLastPathComponent(NULL, fileURL);
        CFURLGetFileSystemRepresentation(folderURL, true, folderName, 1024);
        theErr = FSLocationFromFullPath(folderName, &theParentRef);
        CFRelease(fileURL);
        CFRelease(folderURL);
        CFRelease(folderPathCFString);
    }         
    if (theErr != noErr)
    {
        if (ulCreation == OPEN_EXISTING || theErr != fnfErr)
            return INVALID_HANDLE_VALUE;
    }
    
    if (ulCreation != OPEN_EXISTING)
    {   /* We create the file */
        const UniChar *unicodeFileName;
        CFStringRef filePathCFString = CFStringCreateWithCString(NULL, sFileName, kCFStringEncodingUTF8);
        #ifdef __MACH__
        CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, filePathCFString, kCFURLPOSIXPathStyle, false);
        #else
        CFURLRef fileURL = CFURLCreateWithFileSystemPath(NULL, filePathCFString, kCFURLHFSPathStyle, false);
        #endif
        CFStringRef fileNameCFString = CFURLCopyLastPathComponent(fileURL);
        if(!(unicodeFileName = CFStringGetCharactersPtr(fileNameCFString)))
        {
            CFRelease(fileNameCFString);
            CFRelease(filePathCFString);
            CFRelease(fileURL);
            return INVALID_HANDLE_VALUE;
        }
        theErr = FSCreateCompat(&theParentRef, gCreator, gType, unicodeFileName, 
                                CFStringGetLength(fileNameCFString), &theFileRef);
        CFRelease(fileNameCFString);
        CFRelease(filePathCFString);
        CFRelease(fileURL);
        if (theErr != noErr)
            return INVALID_HANDLE_VALUE;
    }

    if (ulMode == GENERIC_READ)
        permission = fsRdPerm;
    else
    {
        if (ulMode == GENERIC_WRITE)
            permission = fsWrPerm;
        else
            permission = fsRdWrPerm;
    }
    theErr = FSOpenDFCompat(&theFileRef, permission, &fileRef);
    
    if (theErr == noErr)
        return (HANDLE)(int)fileRef;
    else
        return INVALID_HANDLE_VALUE;
}

/********************************************************************
*    CloseHandle
********************************************************************/
BOOL CloseHandle(   HANDLE hFile    )    /* handle to object */
{
    if ((hFile == NULL) || (hFile == INVALID_HANDLE_VALUE))
        return 0;

    FSCloseFork((short)(int)hFile);
    
    return 1;
}

/********************************************************************
*    GetFileSize
********************************************************************/
DWORD GetFileSize(  HANDLE hFile,           /* handle to file */
                    DWORD *ulOffSetHigh )   /* high-order word of file size */
{
    SInt64  fileLength;
    OSErr   theErr;

    if ((hFile == NULL) || (hFile == INVALID_HANDLE_VALUE))
        return -1u;
    
    theErr = FSGetForkSize((short)(int)hFile, &fileLength);
    if (theErr != noErr)
        return -1u;
    
    if (ulOffSetHigh != NULL)
        *ulOffSetHigh = fileLength >> 32;

    return fileLength;
}

/********************************************************************
*    SetFilePointer
*        pOffSetHigh: NULL
*        ulMethod: FILE_BEGIN, FILE_CURRENT
********************************************************************/
DWORD SetFilePointer(   HANDLE hFile,           /* handle to file */
                        LONG lOffSetLow,        /* bytes to move pointer */
                        LONG *pOffSetHigh,      /* bytes to move pointer */
                        DWORD ulMethod  )       /* starting point */
{
    OSErr theErr;

    if (ulMethod == FILE_CURRENT)
    {
        SInt64  bytesToMove;
        
        if (pOffSetHigh != NULL)
            bytesToMove = *pOffSetHigh << 32 + lOffSetLow;
        else
            bytesToMove = lOffSetLow;
        
        SInt64  newPos;
        
        theErr = FSSetForkPosition((short)(int)hFile, fsFromMark, bytesToMove);
        if (theErr != noErr)
            return -1u;
        
        theErr = FSGetForkPosition((short)(int)hFile, &newPos);
        if (theErr != noErr)
            return -1u;
        
        if (pOffSetHigh != NULL)
            *pOffSetHigh = newPos >> 32;
        
        return newPos;
    }
    else if (ulMethod == FILE_BEGIN)
    {
        SInt64  bytesToMove;
        
        if (pOffSetHigh != NULL)
            bytesToMove = *pOffSetHigh << 32 + lOffSetLow;
        else
            bytesToMove = lOffSetLow;
        
        theErr = FSSetForkPosition((short)(int)hFile, fsFromStart, bytesToMove);
        if (theErr != noErr)
            return -1u;
        
        return lOffSetLow;
    }
    else
    {
        SInt64  bytesToMove;
        
        if (pOffSetHigh != NULL)
            bytesToMove = *pOffSetHigh << 32 + lOffSetLow;
        else
            bytesToMove = lOffSetLow;
        
        theErr = FSSetForkPosition((short)(int)hFile, fsFromLEOF, bytesToMove);
        if (theErr != noErr)
            return -1u;
        
        return lOffSetLow;
    }
}

/********************************************************************
*    SetEndOfFile
********************************************************************/
BOOL SetEndOfFile(  HANDLE hFile    )   /* handle to file */
{
    OSErr theErr;
    
    theErr = FSSetForkSize((short)(int)hFile, fsAtMark, 0);
    if (theErr != noErr)
        return FALSE;
    
    return TRUE;
}

/********************************************************************
*    ReadFile
*        pOverLapped: NULL
********************************************************************/
BOOL ReadFile(  HANDLE hFile,           /* handle to file */
                void *pBuffer,          /* data buffer */
                DWORD ulLen,            /* number of bytes to read */
                DWORD *ulRead,          /* number of bytes read */
                void *pOverLapped   )   /* overlapped buffer */
{
#pragma unused (pOverLapped)

    ByteCount   nbCharsRead;
    OSErr       theErr;
    
    nbCharsRead = ulLen;
    theErr = FSReadFork((short)(int)hFile, fsAtMark, 0, nbCharsRead, pBuffer, &nbCharsRead);
    *ulRead = nbCharsRead;
    
    return theErr == noErr;
}

/********************************************************************
*    WriteFile
*        pOverLapped: NULL
********************************************************************/
BOOL WriteFile( HANDLE hFile,           /* handle to file */
                const void *pBuffer,    /* data buffer */
                DWORD ulLen,            /* number of bytes to write */
                DWORD *ulWritten,       /* number of bytes written */
                void *pOverLapped   )   /* overlapped buffer */
{
#pragma unused (pOverLapped)

    ByteCount   nbCharsToWrite;
    OSErr       theErr;
    
    nbCharsToWrite = ulLen; 
    theErr = FSWriteFork((short)(int)hFile, fsAtMark, 0, nbCharsToWrite, pBuffer, &nbCharsToWrite);
    *ulWritten = nbCharsToWrite;
    
    return theErr == noErr;
}

// Check if a memory block is accessible for reading
BOOL IsBadReadPtr(const void * ptr, int size)
{
#pragma unused (ptr, size)

    return FALSE;
}

// Returns attributes of a file
DWORD GetFileAttributes(const char * szFileName)
{
#pragma unused (szFileName)

    return 0;
}

#endif
