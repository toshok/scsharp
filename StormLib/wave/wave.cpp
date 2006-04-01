/*****************************************************************************/
/* wave.cpp                               Copyright (c) Ladislav Zezula 2003 */
/*---------------------------------------------------------------------------*/
/* This module contains decompression methods used by Storm.dll to decompress*/
/* WAVe files. Thanks to Tom Amigo for releasing his sources.                */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 11.03.03  1.00  Lad  Splitted from Pkware.cpp                             */
/* 20.05.03  2.00  Lad  Added compression                                    */
/* 19.11.03  2.01  Dan  Big endian handling                                  */
/*****************************************************************************/

#include "wave.h"

//------------------------------------------------------------------------------
// Structures

union TByteAndWordPtr
{
    unsigned short * pw;
    unsigned char  * pb;
};

//-----------------------------------------------------------------------------
// Tables necessary dor decompression

static unsigned long Table1503F120[] =
{
    0xFFFFFFFF, 0x00000000, 0xFFFFFFFF, 0x00000004, 0xFFFFFFFF, 0x00000002, 0xFFFFFFFF, 0x00000006,
    0xFFFFFFFF, 0x00000001, 0xFFFFFFFF, 0x00000005, 0xFFFFFFFF, 0x00000003, 0xFFFFFFFF, 0x00000007,
    0xFFFFFFFF, 0x00000001, 0xFFFFFFFF, 0x00000005, 0xFFFFFFFF, 0x00000003, 0xFFFFFFFF, 0x00000007,  
    0xFFFFFFFF, 0x00000002, 0xFFFFFFFF, 0x00000004, 0xFFFFFFFF, 0x00000006, 0xFFFFFFFF, 0x00000008  
};

static unsigned long Table1503F1A0[] =
{
    0x00000007, 0x00000008, 0x00000009, 0x0000000A, 0x0000000B, 0x0000000C, 0x0000000D, 0x0000000E,
    0x00000010, 0x00000011, 0x00000013, 0x00000015, 0x00000017, 0x00000019, 0x0000001C, 0x0000001F,
    0x00000022, 0x00000025, 0x00000029, 0x0000002D, 0x00000032, 0x00000037, 0x0000003C, 0x00000042,
    0x00000049, 0x00000050, 0x00000058, 0x00000061, 0x0000006B, 0x00000076, 0x00000082, 0x0000008F,
    0x0000009D, 0x000000AD, 0x000000BE, 0x000000D1, 0x000000E6, 0x000000FD, 0x00000117, 0x00000133,
    0x00000151, 0x00000173, 0x00000198, 0x000001C1, 0x000001EE, 0x00000220, 0x00000256, 0x00000292,
    0x000002D4, 0x0000031C, 0x0000036C, 0x000003C3, 0x00000424, 0x0000048E, 0x00000502, 0x00000583,
    0x00000610, 0x000006AB, 0x00000756, 0x00000812, 0x000008E0, 0x000009C3, 0x00000ABD, 0x00000BD0,
    0x00000CFF, 0x00000E4C, 0x00000FBA, 0x0000114C, 0x00001307, 0x000014EE, 0x00001706, 0x00001954,
    0x00001BDC, 0x00001EA5, 0x000021B6, 0x00002515, 0x000028CA, 0x00002CDF, 0x0000315B, 0x0000364B,
    0x00003BB9, 0x000041B2, 0x00004844, 0x00004F7E, 0x00005771, 0x0000602F, 0x000069CE, 0x00007462,
    0x00007FFF
};

//----------------------------------------------------------------------------
// CompressWave

int CompressWave(unsigned char * pbOutBuffer, int dwOutLength, short * pwInBuffer, int dwInLength, int nCmpType, int nChannels)
{
    TByteAndWordPtr out;                // Output buffer
    long     nArray3[2];                // EBP-48
    long     nArray2[2];                // EBP-40
    long     nArray1[2];                // EBP-38
    long     nWords;                    // EBP-30
    unsigned EBP2C;                     // EBP-2C
    long     nRemains = dwOutLength;    // EBP-28 : Number of bytes remaining
    long     nCmpType2;                 // EBP-24
    unsigned dwStopBit;                 // EBP-20
    long     EBP1C;                     // EBP-1C
    unsigned dwBitBuff;                 // EBP-18
//  char   * pbOutPos = pbOutBuffer;    // EBP-14 : Saved pointer to the output buffer
    long     nLength;                   // EBP-10 :
    long     nIndex;                    // EBP-0C : 
//  short  * pwOutPos = pwInBuffer;     // EBP-08 : Current output pointer
                                        // EBP-04 :
    unsigned short wCmp;                // EBP-02 : 
    unsigned dwBit;                     // EBP+08 : A Bit
    long    nTableValue;
    long    nOneWord;
    long    nValue;

    if(dwInLength < 2)
        return 2;

    // ebx = nCmpType;
    wCmp = (unsigned char)(nChannels - 1);
    out.pb = pbOutBuffer;
    *out.pb++ = 0;
    *out.pb++ = (unsigned char)wCmp;

    if((out.pb - pbOutBuffer + nCmpType * 2) > nRemains)
        return out.pb - pbOutBuffer + nCmpType * 2;

    nArray1[0] = nArray1[1] = 0x2C;

    if(nCmpType > 0)
    {
        for(int i = 0; i < nCmpType; i++)
        {
            nOneWord = LITTLEENDIAN16BITS(*pwInBuffer++);
            *out.pw++ = LITTLEENDIAN16BITS((unsigned short)nOneWord);
            nArray2[i] = nOneWord;
        }
    }

    nLength = dwInLength;
    if(nLength < 0)      // mov eax, dwInLength; cdq; sub eax, edx;
        nLength++;
    nLength   = (nLength / 2) - (out.pb - pbOutBuffer);
    nLength   = (nLength < 0) ? 0 : nLength;
    nCmpType2 = nCmpType;

    nIndex    = nCmpType - 1;         // edi
    nWords    = dwInLength / 2;       // eax
    if(nWords <= nCmpType)
        return out.pb - pbOutBuffer;

    // ebx - nCmpType
    // ecx - pwOutPos
    do // 1500F02D
    {
        if((out.pb - pbOutBuffer + 2) > nRemains)
            return out.pb - pbOutBuffer + 2;

        if(nCmpType == 2)
            nIndex = (nIndex == 0) ? 1 : 0;

        nOneWord = LITTLEENDIAN16BITS(*pwInBuffer++);   // ecx - nOneWord
        // esi - nArray2[nIndex]
        nValue   = nOneWord - nArray2[nIndex];
        if(nValue < 0)
            nValue = ~nValue + 1;   // eax

        unsigned ebx = (nOneWord >= nArray2[nIndex]) ? 1 : 0;
        // esi - nArray1[nIndex]
        // edx - Table1503F1A0[nArray1[nIndex]]
        nArray3[nIndex] = nOneWord;

        // edi - Table1503F1A0[nArray1[nIndex]] >> nChannels
        ebx = (ebx - 1) & 0x40;
        dwStopBit = nChannels;

        // edi - nIndex;
        nTableValue = Table1503F1A0[nArray1[nIndex]];
        if(nValue < (nTableValue >> nChannels))
        {
            if(nArray1[nIndex] != 0)
                nArray1[nIndex]--;
            *out.pb++ = 0x80;
        }
        else
        {
            while(nValue > nTableValue * 2)
            { // 1500F0C0
                if(nArray1[nIndex] >= 0x58 || nLength == 0)
                    break;

                nArray1[nIndex] += 8;
                if(nArray1[nIndex] > 0x58)
                    nArray1[nIndex] = 0x58;

                nTableValue = Table1503F1A0[nArray1[nIndex]];
                *out.pb++ = 0x81;
                nLength--;
            }

            dwBitBuff = 0;
            EBP2C     = nTableValue >> wCmp;
            unsigned esi = 1 << (dwStopBit - 2);
            EBP1C     = 0;
            dwBit     = 1;
            dwStopBit = 0x20;

            if(esi <= 0x20)
                dwStopBit = esi;
            for(;;)
            {
//              esi = EBP1C + nTableValue;
                if((EBP1C + nTableValue) <= nValue)
                {
                    EBP1C += nTableValue;
                    dwBitBuff |= dwBit;
                }
                if(dwBit == dwStopBit)
                    break;

                nTableValue >>= 1;
                dwBit       <<= 1;
            }

            nValue = nArray2[nIndex];
            if(ebx != 0)
            {
                nValue -= (EBP1C + EBP2C);
                if(nValue < (int)0xFFFF8000)
                    nValue = (int)0xFFFF8000;
            }
            else
            {
                nValue += (EBP1C + EBP2C);
                if(nValue > 0x00007FFF)
                    nValue = 0x00007FFF;
            }

            nArray2[nIndex]  = nValue;
            *out.pb++ = (unsigned char)(dwBitBuff | ebx);
            nTableValue      = Table1503F120[dwBitBuff & 0x1F];
            nArray1[nIndex] += nTableValue; 
            if(nArray1[nIndex] < 0)
                nArray1[nIndex] = 0;
            else if(nArray1[nIndex] > 0x58)
                nArray1[nIndex] = 0x58;
        }
        // 1500F1D8
    }
    while(++nCmpType2 < nWords);
    
    return out.pb - pbOutBuffer;
}

//----------------------------------------------------------------------------
// DecompressWave

// 1500F230
int DecompressWave(unsigned char * pbOutBuffer, int dwOutLength, unsigned char * pbInBuffer, int dwInLength, int nChannels)
{
    TByteAndWordPtr out;
    TByteAndWordPtr in;
    unsigned char * pbInEnd = pbInBuffer + dwInLength;  // End on input buffer
    unsigned long index;
    long nArray1[2];
    long nArray2[2];

    out.pb     = pbOutBuffer;
    in.pb      = pbInBuffer;
    nArray1[0] = 0x2C;
    nArray1[1] = 0x2C;
    in.pw++;

    // 15007AD7
    for(int count = 0; count < nChannels; count++)
    {
        long temp;

        temp = LITTLEENDIAN16BITS(*(short *)in.pw++);
        nArray2[count] = temp;

        if(dwOutLength < 2)
            return out.pb - pbOutBuffer;

        *out.pw++    = LITTLEENDIAN16BITS((unsigned short)temp);
        dwOutLength -= 2;
    }

    index = nChannels - 1;
    while(in.pb < pbInEnd)
    {
        unsigned char oneByte = *in.pb++;

        if(nChannels == 2)
            index = (index == 0) ? 1 : 0;

        // Get one byte from input buffer
        // 15007B25
        if(oneByte & 0x80)
        {
            // 15007B32
            switch(oneByte & 0x7F)
            {
                case 0:     // 15007B8E
                    if(nArray1[index] != 0)
                        nArray1[index]--;

                    if(dwOutLength < 2)
                        break;

                    *out.pw++ = LITTLEENDIAN16BITS((unsigned short)nArray2[index]);
                    dwOutLength -= 2;
                    continue;

                case 1:     // 15007B72
                    nArray1[index] += 8;   // EBX also
                    if(nArray1[index] > 0x58)
                        nArray1[index] = 0x58;
                    if(nChannels == 2)
                        index = (index == 0) ? 1 : 0;
                    continue;

                case 2:
                    continue;

                default:
                    nArray1[index] -= 8;
                    if(nArray1[index] < 0)
                        nArray1[index] = 0;

                    if(nChannels != 2)
                        continue;

                    index = (index == 0) ? 1 : 0;
                    continue;
            }
        }
        else
        {
            unsigned long temp1 = Table1503F1A0[nArray1[index]]; // EDI
            unsigned long temp2 = temp1 >> pbInBuffer[1];      // ESI
            long  temp3 = nArray2[index];                // ECX

            if(oneByte & 0x01)          // EBX = oneByte
                temp2 += (temp1 >> 0);

            if(oneByte & 0x02)
                temp2 += (temp1 >> 1);

            if(oneByte & 0x04)
                temp2 += (temp1 >> 2);

            if(oneByte & 0x08)
                temp2 += (temp1 >> 3);

            if(oneByte & 0x10)
                temp2 += (temp1 >> 4);

            if(oneByte & 0x20)
                temp2 += (temp1 >> 5);

            if(oneByte & 0x40)
            {
                temp3 -= temp2;
                if(temp3 <= (long)0xFFFF8000)
                    temp3 = (long)0xFFFF8000;
            }
            else
            {
                temp3 += temp2;
                if(temp3 >= 0x7FFF)
                    temp3 = 0x7FFF;
            }
            nArray2[index] = temp3;
            if(dwOutLength < 2)
                break;

            temp2 = nArray1[index];
            oneByte &= 0x1F;
            
            *out.pw++ = LITTLEENDIAN16BITS((unsigned short)temp3);
            dwOutLength -= 2;

            temp2 += Table1503F120[oneByte];
            nArray1[index] = temp2;

            if(nArray1[index] < 0)
                nArray1[index] = 0;
            else if(nArray1[index] > 0x58)
                nArray1[index] = 0x58;
        }
    }
    return (out.pb - pbOutBuffer);
}

