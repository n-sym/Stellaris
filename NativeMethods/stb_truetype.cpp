#include "pch.h"
// stb_truetype.h - v1.24 - public domain
// authored from 2009-2020 by Sean Barrett / RAD Game Tool
#ifndef stbtt_uint8
typedef unsigned char   stbtt_uint8;
typedef signed   char   stbtt_int8;
typedef unsigned short  stbtt_uint16;
typedef signed   short  stbtt_int16;
typedef unsigned int    stbtt_uint32;
typedef signed   int    stbtt_int32;
#endif

typedef char stbtt__check_size32[sizeof(stbtt_int32) == 4 ? 1 : -1];
typedef char stbtt__check_size16[sizeof(stbtt_int16) == 2 ? 1 : -1];

// e.g. #define your own STBTT_ifloor/STBTT_iceil() to avoid math.h
#ifndef STBTT_ifloor
#include <math.h>
#define STBTT_ifloor(x)   ((int) floor(x))
#define STBTT_iceil(x)    ((int) ceil(x))
#endif

#ifndef STBTT_sqrt
#include <math.h>
#define STBTT_sqrt(x)      sqrt(x)
#define STBTT_pow(x,y)     pow(x,y)
#endif

#ifndef STBTT_fmod
#include <math.h>
#define STBTT_fmod(x,y)    fmod(x,y)
#endif

#ifndef STBTT_cos
#include <math.h>
#define STBTT_cos(x)       cos(x)
#define STBTT_acos(x)      acos(x)
#endif

#ifndef STBTT_fabs
#include <math.h>
#define STBTT_fabs(x)      fabs(x)
#endif

// #define your own functions "STBTT_malloc" / "STBTT_free" to avoid malloc.h
#ifndef STBTT_malloc
#include <stdlib.h>
#define STBTT_malloc(x,u)  ((void)(u),malloc(x))
#define STBTT_free(x,u)    ((void)(u),free(x))
#endif

#ifndef STBTT_assert
#include <assert.h>
#define STBTT_assert(x)    assert(x)
#endif

#ifndef STBTT_strlen
#include <string.h>
#define STBTT_strlen(x)    strlen(x)
#endif

#ifndef STBTT_memcpy
#include <string.h>
#define STBTT_memcpy       memcpy
#define STBTT_memset       memset
#endif

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////
////   INTERFACE
////
////

#ifndef __STB_INCLUDE_STB_TRUETYPE_H__
#define __STB_INCLUDE_STB_TRUETYPE_H__

#ifdef STBTT_STATIC
#define STBTT_DEF static
#else
#define STBTT_DEF extern "C" __declspec(dllexport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

    // private structure
    typedef struct
    {
        unsigned char* data;
        int cursor;
        int size;
    } stbtt__buf;

    struct stbtt_fontinfo
    {
        void* userdata;
        unsigned char* data;              // pointer to .ttf file
        int              fontstart;         // offset of start of font

        int numGlyphs;                     // number of glyphs, needed for range checking

        int loca, head, glyf, hhea, hmtx, kern, gpos, svg; // table locations as offset from start of .ttf
        int index_map;                     // a cmap mapping for our chosen character encoding
        int indexToLocFormat;              // format needed to map from glyph index to glyph

        stbtt__buf cff;                    // cff font data
        stbtt__buf charstrings;            // the charstring index
        stbtt__buf gsubrs;                 // global charstring subroutines index
        stbtt__buf subrs;                  // private charstring subroutines index
        stbtt__buf fontdicts;              // array of font dicts
        stbtt__buf fdselect;               // map from glyph to fontdict
    };
 
    STBTT_DEF int stbtt_InitFont(stbtt_fontinfo* info, const unsigned char* data, int offset);

    STBTT_DEF int stbtt_FindGlyphIndex(const stbtt_fontinfo* info, int unicode_codepoint);
    STBTT_DEF float stbtt_ScaleForPixelHeight(const stbtt_fontinfo* info, float pixels);
   
    STBTT_DEF int stbtt_GetCodepointBox(const stbtt_fontinfo* info, int codepoint, int* x0, int* y0, int* x1, int* y1);
    // Gets the bounding box of the visible part of the glyph, in unscaled coordinates

    STBTT_DEF int  stbtt_GetGlyphBox(const stbtt_fontinfo* info, int glyph_index, int* x0, int* y0, int* x1, int* y1);
    // as above, but takes one or more glyph indices for greater efficiency

    typedef struct stbtt_kerningentry
    {
        int glyph1; // use stbtt_FindGlyphIndex
        int glyph2;
        int advance;
    } stbtt_kerningentry;

#ifndef STBTT_vmove // you can predefine these to use different values (but why?)
    enum {
        STBTT_vmove = 1,
        STBTT_vline,
        STBTT_vcurve,
        STBTT_vcubic
    };
#endif

#ifndef stbtt_vertex // you can predefine this to use different values
    // (we share this with other code at RAD)
#define stbtt_vertex_type short // can't use stbtt_int16 because that's not visible in the header file
    typedef struct
    {
        stbtt_vertex_type x, y, cx, cy, cx1, cy1;
        unsigned char type, padding;
    } stbtt_vertex;
#endif

    STBTT_DEF int stbtt_IsGlyphEmpty(const stbtt_fontinfo* info, int glyph_index);
    // returns non-zero if nothing is drawn for this glyph

    STBTT_DEF int stbtt_GetCodepointShape(const stbtt_fontinfo* info, int unicode_codepoint, stbtt_vertex** vertices);
    STBTT_DEF int stbtt_GetGlyphShape(const stbtt_fontinfo* info, int glyph_index, stbtt_vertex** vertices);

    STBTT_DEF void stbtt_FreeBitmap(unsigned char* bitmap, void* userdata);
    STBTT_DEF void stbtt_MakeCodepointBitmap(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int codepoint);

    STBTT_DEF void stbtt_MakeCodepointBitmapSubpixel(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint);

    STBTT_DEF void stbtt_GetCodepointBitmapBox(const stbtt_fontinfo* font, int codepoint, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1);

    STBTT_DEF void stbtt_GetCodepointBitmapBoxSubpixel(const stbtt_fontinfo* font, int codepoint, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1);

    STBTT_DEF unsigned char* stbtt_GetGlyphBitmapSubpixel(const stbtt_fontinfo* info, float scale_x, float scale_y, float shift_x, float shift_y, int glyph, int* width, int* height, int* xoff, int* yoff);
    STBTT_DEF void stbtt_MakeGlyphBitmap(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph);
    STBTT_DEF void stbtt_MakeGlyphBitmapSubpixel(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph);
    STBTT_DEF void stbtt_GetGlyphBitmapBoxSubpixel(const stbtt_fontinfo* font, int glyph, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1);


    // @TODO: don't expose this structure
    typedef struct
    {
        int w, h, stride;
        unsigned char* pixels;
    } stbtt__bitmap;

    // rasterize a shape with quadratic beziers into a bitmap
    STBTT_DEF void stbtt_Rasterize(stbtt__bitmap* result,        // 1-channel bitmap to draw into
        float flatness_in_pixels,     // allowable error of curve in pixels
        stbtt_vertex* vertices,       // array of vertices defining shape
        int num_verts,                // number of vertices in above array
        float scale_x, float scale_y, // scale applied to input vertices
        float shift_x, float shift_y, // translation applied to input vertices
        int x_off, int y_off,         // another translation applied to input
        int invert,                   // if non-zero, vertically flip shape
        void* userdata);              // context for to STBTT_MALLOC

#define STBTT_MACSTYLE_DONTCARE     0
#define STBTT_MACSTYLE_BOLD         1
#define STBTT_MACSTYLE_ITALIC       2
#define STBTT_MACSTYLE_UNDERSCORE   4
#define STBTT_MACSTYLE_NONE         8   // <= not same as 0, this makes us check the bitfield is 0

    enum { // platformID
        STBTT_PLATFORM_ID_UNICODE = 0,
        STBTT_PLATFORM_ID_MAC = 1,
        STBTT_PLATFORM_ID_ISO = 2,
        STBTT_PLATFORM_ID_MICROSOFT = 3
    };

    enum { // encodingID for STBTT_PLATFORM_ID_UNICODE
        STBTT_UNICODE_EID_UNICODE_1_0 = 0,
        STBTT_UNICODE_EID_UNICODE_1_1 = 1,
        STBTT_UNICODE_EID_ISO_10646 = 2,
        STBTT_UNICODE_EID_UNICODE_2_0_BMP = 3,
        STBTT_UNICODE_EID_UNICODE_2_0_FULL = 4
    };

    enum { // encodingID for STBTT_PLATFORM_ID_MICROSOFT
        STBTT_MS_EID_SYMBOL = 0,
        STBTT_MS_EID_UNICODE_BMP = 1,
        STBTT_MS_EID_SHIFTJIS = 2,
        STBTT_MS_EID_UNICODE_FULL = 10
    };

    enum { // encodingID for STBTT_PLATFORM_ID_MAC; same as Script Manager codes
        STBTT_MAC_EID_ROMAN = 0, STBTT_MAC_EID_ARABIC = 4,
        STBTT_MAC_EID_JAPANESE = 1, STBTT_MAC_EID_HEBREW = 5,
        STBTT_MAC_EID_CHINESE_TRAD = 2, STBTT_MAC_EID_GREEK = 6,
        STBTT_MAC_EID_KOREAN = 3, STBTT_MAC_EID_RUSSIAN = 7
    };

    enum { // languageID for STBTT_PLATFORM_ID_MICROSOFT; same as LCID...
           // problematic because there are e.g. 16 english LCIDs and 16 arabic LCIDs
        STBTT_MS_LANG_ENGLISH = 0x0409, STBTT_MS_LANG_ITALIAN = 0x0410,
        STBTT_MS_LANG_CHINESE = 0x0804, STBTT_MS_LANG_JAPANESE = 0x0411,
        STBTT_MS_LANG_DUTCH = 0x0413, STBTT_MS_LANG_KOREAN = 0x0412,
        STBTT_MS_LANG_FRENCH = 0x040c, STBTT_MS_LANG_RUSSIAN = 0x0419,
        STBTT_MS_LANG_GERMAN = 0x0407, STBTT_MS_LANG_SPANISH = 0x0409,
        STBTT_MS_LANG_HEBREW = 0x040d, STBTT_MS_LANG_SWEDISH = 0x041D
    };

    enum { // languageID for STBTT_PLATFORM_ID_MAC
        STBTT_MAC_LANG_ENGLISH = 0, STBTT_MAC_LANG_JAPANESE = 11,
        STBTT_MAC_LANG_ARABIC = 12, STBTT_MAC_LANG_KOREAN = 23,
        STBTT_MAC_LANG_DUTCH = 4, STBTT_MAC_LANG_RUSSIAN = 32,
        STBTT_MAC_LANG_FRENCH = 1, STBTT_MAC_LANG_SPANISH = 6,
        STBTT_MAC_LANG_GERMAN = 2, STBTT_MAC_LANG_SWEDISH = 5,
        STBTT_MAC_LANG_HEBREW = 10, STBTT_MAC_LANG_CHINESE_SIMPLIFIED = 33,
        STBTT_MAC_LANG_ITALIAN = 3, STBTT_MAC_LANG_CHINESE_TRAD = 19
    };

#ifdef __cplusplus
}
#endif

#endif // __STB_INCLUDE_STB_TRUETYPE_H__

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////
////   IMPLEMENTATION
////
////

#ifndef STBTT_MAX_OVERSAMPLE
#define STBTT_MAX_OVERSAMPLE   8
#endif

#if STBTT_MAX_OVERSAMPLE > 255
#error "STBTT_MAX_OVERSAMPLE cannot be > 255"
#endif

typedef int stbtt__test_oversample_pow2[(STBTT_MAX_OVERSAMPLE & (STBTT_MAX_OVERSAMPLE - 1)) == 0 ? 1 : -1];

#ifndef STBTT_RASTERIZER_VERSION
#define STBTT_RASTERIZER_VERSION 2
#endif

#ifdef _MSC_VER
#define STBTT__NOTUSED(v)  (void)(v)
#else
#define STBTT__NOTUSED(v)  (void)sizeof(v)
#endif

//////////////////////////////////////////////////////////////////////////
//
// stbtt__buf helpers to parse data from file
//

static stbtt_uint8 stbtt__buf_get8(stbtt__buf* b)
{
    if (b->cursor >= b->size)
        return 0;
    return b->data[b->cursor++];
}

static stbtt_uint8 stbtt__buf_peek8(stbtt__buf* b)
{
    if (b->cursor >= b->size)
        return 0;
    return b->data[b->cursor];
}

static void stbtt__buf_seek(stbtt__buf* b, int o)
{
    STBTT_assert(!(o > b->size || o < 0));
    b->cursor = (o > b->size || o < 0) ? b->size : o;
}

static void stbtt__buf_skip(stbtt__buf* b, int o)
{
    stbtt__buf_seek(b, b->cursor + o);
}

static stbtt_uint32 stbtt__buf_get(stbtt__buf* b, int n)
{
    stbtt_uint32 v = 0;
    int i;
    STBTT_assert(n >= 1 && n <= 4);
    for (i = 0; i < n; i++)
        v = (v << 8) | stbtt__buf_get8(b);
    return v;
}

static stbtt__buf stbtt__new_buf(const void* p, size_t size)
{
    stbtt__buf r;
    STBTT_assert(size < 0x40000000);
    r.data = (stbtt_uint8*)p;
    r.size = (int)size;
    r.cursor = 0;
    return r;
}

#define stbtt__buf_get16(b)  stbtt__buf_get((b), 2)
#define stbtt__buf_get32(b)  stbtt__buf_get((b), 4)

static stbtt__buf stbtt__buf_range(const stbtt__buf* b, int o, int s)
{
    stbtt__buf r = stbtt__new_buf(NULL, 0);
    if (o < 0 || s < 0 || o > b->size || s > b->size - o) return r;
    r.data = b->data + o;
    r.size = s;
    return r;
}

static stbtt__buf stbtt__cff_get_index(stbtt__buf* b)
{
    int count, start, offsize;
    start = b->cursor;
    count = stbtt__buf_get16(b);
    if (count) {
        offsize = stbtt__buf_get8(b);
        STBTT_assert(offsize >= 1 && offsize <= 4);
        stbtt__buf_skip(b, offsize * count);
        stbtt__buf_skip(b, stbtt__buf_get(b, offsize) - 1);
    }
    return stbtt__buf_range(b, start, b->cursor - start);
}

static stbtt_uint32 stbtt__cff_int(stbtt__buf* b)
{
    int b0 = stbtt__buf_get8(b);
    if (b0 >= 32 && b0 <= 246)       return b0 - 139;
    else if (b0 >= 247 && b0 <= 250) return (b0 - 247) * 256 + stbtt__buf_get8(b) + 108;
    else if (b0 >= 251 && b0 <= 254) return -(b0 - 251) * 256 - stbtt__buf_get8(b) - 108;
    else if (b0 == 28)               return stbtt__buf_get16(b);
    else if (b0 == 29)               return stbtt__buf_get32(b);
    STBTT_assert(0);
    return 0;
}

static void stbtt__cff_skip_operand(stbtt__buf* b) {
    int v, b0 = stbtt__buf_peek8(b);
    STBTT_assert(b0 >= 28);
    if (b0 == 30) {
        stbtt__buf_skip(b, 1);
        while (b->cursor < b->size) {
            v = stbtt__buf_get8(b);
            if ((v & 0xF) == 0xF || (v >> 4) == 0xF)
                break;
        }
    }
    else {
        stbtt__cff_int(b);
    }
}

static stbtt__buf stbtt__dict_get(stbtt__buf* b, int key)
{
    stbtt__buf_seek(b, 0);
    while (b->cursor < b->size) {
        int start = b->cursor, end, op;
        while (stbtt__buf_peek8(b) >= 28)
            stbtt__cff_skip_operand(b);
        end = b->cursor;
        op = stbtt__buf_get8(b);
        if (op == 12)  op = stbtt__buf_get8(b) | 0x100;
        if (op == key) return stbtt__buf_range(b, start, end - start);
    }
    return stbtt__buf_range(b, 0, 0);
}

static void stbtt__dict_get_ints(stbtt__buf* b, int key, int outcount, stbtt_uint32* out)
{
    int i;
    stbtt__buf operands = stbtt__dict_get(b, key);
    for (i = 0; i < outcount && operands.cursor < operands.size; i++)
        out[i] = stbtt__cff_int(&operands);
}

static int stbtt__cff_index_count(stbtt__buf* b)
{
    stbtt__buf_seek(b, 0);
    return stbtt__buf_get16(b);
}

static stbtt__buf stbtt__cff_index_get(stbtt__buf b, int i)
{
    int count, offsize, start, end;
    stbtt__buf_seek(&b, 0);
    count = stbtt__buf_get16(&b);
    offsize = stbtt__buf_get8(&b);
    STBTT_assert(i >= 0 && i < count);
    STBTT_assert(offsize >= 1 && offsize <= 4);
    stbtt__buf_skip(&b, i * offsize);
    start = stbtt__buf_get(&b, offsize);
    end = stbtt__buf_get(&b, offsize);
    return stbtt__buf_range(&b, 2 + (count + 1) * offsize + start, end - start);
}

//////////////////////////////////////////////////////////////////////////
//
// accessors to parse data from file
//

// on platforms that don't allow misaligned reads, if we want to allow
// truetype fonts that aren't padded to alignment, define ALLOW_UNALIGNED_TRUETYPE

#define ttBYTE(p)     (* (stbtt_uint8 *) (p))
#define ttCHAR(p)     (* (stbtt_int8 *) (p))
#define ttFixed(p)    ttLONG(p)

static stbtt_uint16 ttUSHORT(stbtt_uint8* p) { return p[0] * 256 + p[1]; }
static stbtt_int16 ttSHORT(stbtt_uint8* p) { return p[0] * 256 + p[1]; }
static stbtt_uint32 ttULONG(stbtt_uint8* p) { return (p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3]; }
static stbtt_int32 ttLONG(stbtt_uint8* p) { return (p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3]; }

#define stbtt_tag4(p,c0,c1,c2,c3) ((p)[0] == (c0) && (p)[1] == (c1) && (p)[2] == (c2) && (p)[3] == (c3))
#define stbtt_tag(p,str)           stbtt_tag4(p,str[0],str[1],str[2],str[3])

static int stbtt__isfont(stbtt_uint8* font)
{
    // check the version number
    if (stbtt_tag4(font, '1', 0, 0, 0))  return 1; // TrueType 1
    if (stbtt_tag(font, "typ1"))   return 1; // TrueType with type 1 font -- we don't support this!
    if (stbtt_tag(font, "OTTO"))   return 1; // OpenType with CFF
    if (stbtt_tag4(font, 0, 1, 0, 0)) return 1; // OpenType 1.0
    if (stbtt_tag(font, "true"))   return 1; // Apple specification for TrueType fonts
    return 0;
}

// @OPTIMIZE: binary search
static stbtt_uint32 stbtt__find_table(stbtt_uint8* data, stbtt_uint32 fontstart, const char* tag)
{
    stbtt_int32 num_tables = ttUSHORT(data + fontstart + 4);
    stbtt_uint32 tabledir = fontstart + 12;
    stbtt_int32 i;
    for (i = 0; i < num_tables; ++i) {
        stbtt_uint32 loc = tabledir + 16 * i;
        if (stbtt_tag(data + loc + 0, tag))
            return ttULONG(data + loc + 8);
    }
    return 0;
}

static int stbtt_GetFontOffsetForIndex_internal(unsigned char* font_collection, int index)
{
    // if it's just a font, there's only one valid index
    if (stbtt__isfont(font_collection))
        return index == 0 ? 0 : -1;

    // check if it's a TTC
    if (stbtt_tag(font_collection, "ttcf")) {
        // version 1?
        if (ttULONG(font_collection + 4) == 0x00010000 || ttULONG(font_collection + 4) == 0x00020000) {
            stbtt_int32 n = ttLONG(font_collection + 8);
            if (index >= n)
                return -1;
            return ttULONG(font_collection + 12 + index * 4);
        }
    }
    return -1;
}

static int stbtt_GetNumberOfFonts_internal(unsigned char* font_collection)
{
    // if it's just a font, there's only one valid font
    if (stbtt__isfont(font_collection))
        return 1;

    // check if it's a TTC
    if (stbtt_tag(font_collection, "ttcf")) {
        // version 1?
        if (ttULONG(font_collection + 4) == 0x00010000 || ttULONG(font_collection + 4) == 0x00020000) {
            return ttLONG(font_collection + 8);
        }
    }
    return 0;
}

static stbtt__buf stbtt__get_subrs(stbtt__buf cff, stbtt__buf fontdict)
{
    stbtt_uint32 subrsoff = 0, private_loc[2] = { 0, 0 };
    stbtt__buf pdict;
    stbtt__dict_get_ints(&fontdict, 18, 2, private_loc);
    if (!private_loc[1] || !private_loc[0]) return stbtt__new_buf(NULL, 0);
    pdict = stbtt__buf_range(&cff, private_loc[1], private_loc[0]);
    stbtt__dict_get_ints(&pdict, 19, 1, &subrsoff);
    if (!subrsoff) return stbtt__new_buf(NULL, 0);
    stbtt__buf_seek(&cff, private_loc[1] + subrsoff);
    return stbtt__cff_get_index(&cff);
}

// since most people won't use this, find this table the first time it's needed
static int stbtt__get_svg(stbtt_fontinfo* info)
{
    stbtt_uint32 t;
    if (info->svg < 0) {
        t = stbtt__find_table(info->data, info->fontstart, "SVG ");
        if (t) {
            stbtt_uint32 offset = ttULONG(info->data + t + 2);
            info->svg = t + offset;
        }
        else {
            info->svg = 0;
        }
    }
    return info->svg;
}

static int stbtt_InitFont_internal(stbtt_fontinfo* info, unsigned char* data, int fontstart)
{
    stbtt_uint32 cmap, t;
    stbtt_int32 i, numTables;

    info->data = data;
    info->fontstart = fontstart;
    info->cff = stbtt__new_buf(NULL, 0);

    cmap = stbtt__find_table(data, fontstart, "cmap");       // required
    info->loca = stbtt__find_table(data, fontstart, "loca"); // required
    info->head = stbtt__find_table(data, fontstart, "head"); // required
    info->glyf = stbtt__find_table(data, fontstart, "glyf"); // required
    info->hhea = stbtt__find_table(data, fontstart, "hhea"); // required
    info->hmtx = stbtt__find_table(data, fontstart, "hmtx"); // required
    info->kern = stbtt__find_table(data, fontstart, "kern"); // not required
    info->gpos = stbtt__find_table(data, fontstart, "GPOS"); // not required

    if (!cmap || !info->head || !info->hhea || !info->hmtx)
        return 0;
    if (info->glyf) {
        // required for truetype
        if (!info->loca) return 0;
    }
    else {
        // initialization for CFF / Type2 fonts (OTF)
        stbtt__buf b, topdict, topdictidx;
        stbtt_uint32 cstype = 2, charstrings = 0, fdarrayoff = 0, fdselectoff = 0;
        stbtt_uint32 cff;

        cff = stbtt__find_table(data, fontstart, "CFF ");
        if (!cff) return 0;

        info->fontdicts = stbtt__new_buf(NULL, 0);
        info->fdselect = stbtt__new_buf(NULL, 0);

        // @TODO this should use size from table (not 512MB)
        info->cff = stbtt__new_buf(data + cff, 512 * 1024 * 1024);
        b = info->cff;

        // read the header
        stbtt__buf_skip(&b, 2);
        stbtt__buf_seek(&b, stbtt__buf_get8(&b)); // hdrsize

        // @TODO the name INDEX could list multiple fonts,
        // but we just use the first one.
        stbtt__cff_get_index(&b);  // name INDEX
        topdictidx = stbtt__cff_get_index(&b);
        topdict = stbtt__cff_index_get(topdictidx, 0);
        stbtt__cff_get_index(&b);  // string INDEX
        info->gsubrs = stbtt__cff_get_index(&b);

        stbtt__dict_get_ints(&topdict, 17, 1, &charstrings);
        stbtt__dict_get_ints(&topdict, 0x100 | 6, 1, &cstype);
        stbtt__dict_get_ints(&topdict, 0x100 | 36, 1, &fdarrayoff);
        stbtt__dict_get_ints(&topdict, 0x100 | 37, 1, &fdselectoff);
        info->subrs = stbtt__get_subrs(b, topdict);

        // we only support Type 2 charstrings
        if (cstype != 2) return 0;
        if (charstrings == 0) return 0;

        if (fdarrayoff) {
            // looks like a CID font
            if (!fdselectoff) return 0;
            stbtt__buf_seek(&b, fdarrayoff);
            info->fontdicts = stbtt__cff_get_index(&b);
            info->fdselect = stbtt__buf_range(&b, fdselectoff, b.size - fdselectoff);
        }

        stbtt__buf_seek(&b, charstrings);
        info->charstrings = stbtt__cff_get_index(&b);
    }

    t = stbtt__find_table(data, fontstart, "maxp");
    if (t)
        info->numGlyphs = ttUSHORT(data + t + 4);
    else
        info->numGlyphs = 0xffff;

    info->svg = -1;

    // find a cmap encoding table we understand *now* to avoid searching
    // later. (todo: could make this installable)
    // the same regardless of glyph.
    numTables = ttUSHORT(data + cmap + 2);
    info->index_map = 0;
    for (i = 0; i < numTables; ++i) {
        stbtt_uint32 encoding_record = cmap + 4 + 8 * i;
        // find an encoding we understand:
        switch (ttUSHORT(data + encoding_record)) {
        case STBTT_PLATFORM_ID_MICROSOFT:
            switch (ttUSHORT(data + encoding_record + 2)) {
            case STBTT_MS_EID_UNICODE_BMP:
            case STBTT_MS_EID_UNICODE_FULL:
                // MS/Unicode
                info->index_map = cmap + ttULONG(data + encoding_record + 4);
                break;
            }
            break;
        case STBTT_PLATFORM_ID_UNICODE:
            // Mac/iOS has these
            // all the encodingIDs are unicode, so we don't bother to check it
            info->index_map = cmap + ttULONG(data + encoding_record + 4);
            break;
        }
    }
    if (info->index_map == 0)
        return 0;

    info->indexToLocFormat = ttUSHORT(data + info->head + 50);
    return 1;
}

STBTT_DEF int stbtt_FindGlyphIndex(const stbtt_fontinfo* info, int unicode_codepoint)
{
    stbtt_uint8* data = info->data;
    stbtt_uint32 index_map = info->index_map;

    stbtt_uint16 format = ttUSHORT(data + index_map + 0);
    if (format == 0) { // apple byte encoding
        stbtt_int32 bytes = ttUSHORT(data + index_map + 2);
        if (unicode_codepoint < bytes - 6)
            return ttBYTE(data + index_map + 6 + unicode_codepoint);
        return 0;
    }
    else if (format == 6) {
        stbtt_uint32 first = ttUSHORT(data + index_map + 6);
        stbtt_uint32 count = ttUSHORT(data + index_map + 8);
        if ((stbtt_uint32)unicode_codepoint >= first && (stbtt_uint32)unicode_codepoint < first + count)
            return ttUSHORT(data + index_map + 10 + (unicode_codepoint - first) * 2);
        return 0;
    }
    else if (format == 2) {
        STBTT_assert(0); // @TODO: high-byte mapping for japanese/chinese/korean
        return 0;
    }
    else if (format == 4) { // standard mapping for windows fonts: binary search collection of ranges
        stbtt_uint16 segcount = ttUSHORT(data + index_map + 6) >> 1;
        stbtt_uint16 searchRange = ttUSHORT(data + index_map + 8) >> 1;
        stbtt_uint16 entrySelector = ttUSHORT(data + index_map + 10);
        stbtt_uint16 rangeShift = ttUSHORT(data + index_map + 12) >> 1;

        // do a binary search of the segments
        stbtt_uint32 endCount = index_map + 14;
        stbtt_uint32 search = endCount;

        if (unicode_codepoint > 0xffff)
            return 0;

        // they lie from endCount .. endCount + segCount
        // but searchRange is the nearest power of two, so...
        if (unicode_codepoint >= ttUSHORT(data + search + rangeShift * 2))
            search += rangeShift * 2;

        // now decrement to bias correctly to find smallest
        search -= 2;
        while (entrySelector) {
            stbtt_uint16 end;
            searchRange >>= 1;
            end = ttUSHORT(data + search + searchRange * 2);
            if (unicode_codepoint > end)
                search += searchRange * 2;
            --entrySelector;
        }
        search += 2;

        {
            stbtt_uint16 offset, start;
            stbtt_uint16 item = (stbtt_uint16)((search - endCount) >> 1);

            STBTT_assert(unicode_codepoint <= ttUSHORT(data + endCount + 2 * item));
            start = ttUSHORT(data + index_map + 14 + segcount * 2 + 2 + 2 * item);
            if (unicode_codepoint < start)
                return 0;

            offset = ttUSHORT(data + index_map + 14 + segcount * 6 + 2 + 2 * item);
            if (offset == 0)
                return (stbtt_uint16)(unicode_codepoint + ttSHORT(data + index_map + 14 + segcount * 4 + 2 + 2 * item));

            return ttUSHORT(data + offset + (unicode_codepoint - start) * 2 + index_map + 14 + segcount * 6 + 2 + 2 * item);
        }
    }
    else if (format == 12 || format == 13) {
        stbtt_uint32 ngroups = ttULONG(data + index_map + 12);
        stbtt_int32 low, high;
        low = 0; high = (stbtt_int32)ngroups;
        // Binary search the right group.
        while (low < high) {
            stbtt_int32 mid = low + ((high - low) >> 1); // rounds down, so low <= mid < high
            stbtt_uint32 start_char = ttULONG(data + index_map + 16 + mid * 12);
            stbtt_uint32 end_char = ttULONG(data + index_map + 16 + mid * 12 + 4);
            if ((stbtt_uint32)unicode_codepoint < start_char)
                high = mid;
            else if ((stbtt_uint32)unicode_codepoint > end_char)
                low = mid + 1;
            else {
                stbtt_uint32 start_glyph = ttULONG(data + index_map + 16 + mid * 12 + 8);
                if (format == 12)
                    return start_glyph + unicode_codepoint - start_char;
                else // format == 13
                    return start_glyph;
            }
        }
        return 0; // not found
    }
    // @TODO
    STBTT_assert(0);
    return 0;
}

STBTT_DEF int stbtt_GetCodepointShape(const stbtt_fontinfo* info, int unicode_codepoint, stbtt_vertex** vertices)
{
    return stbtt_GetGlyphShape(info, stbtt_FindGlyphIndex(info, unicode_codepoint), vertices);
}

static void stbtt_setvertex(stbtt_vertex* v, stbtt_uint8 type, stbtt_int32 x, stbtt_int32 y, stbtt_int32 cx, stbtt_int32 cy)
{
    v->type = type;
    v->x = (stbtt_int16)x;
    v->y = (stbtt_int16)y;
    v->cx = (stbtt_int16)cx;
    v->cy = (stbtt_int16)cy;
}

static int stbtt__GetGlyfOffset(const stbtt_fontinfo* info, int glyph_index)
{
    int g1, g2;

    STBTT_assert(!info->cff.size);

    if (glyph_index >= info->numGlyphs) return -1; // glyph index out of range
    if (info->indexToLocFormat >= 2)    return -1; // unknown index->glyph map format

    if (info->indexToLocFormat == 0) {
        g1 = info->glyf + ttUSHORT(info->data + info->loca + glyph_index * 2) * 2;
        g2 = info->glyf + ttUSHORT(info->data + info->loca + glyph_index * 2 + 2) * 2;
    }
    else {
        g1 = info->glyf + ttULONG(info->data + info->loca + glyph_index * 4);
        g2 = info->glyf + ttULONG(info->data + info->loca + glyph_index * 4 + 4);
    }

    return g1 == g2 ? -1 : g1; // if length is 0, return -1
}

static int stbtt__GetGlyphInfoT2(const stbtt_fontinfo* info, int glyph_index, int* x0, int* y0, int* x1, int* y1);

STBTT_DEF int stbtt_GetGlyphBox(const stbtt_fontinfo* info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
{
    if (info->cff.size) {
        stbtt__GetGlyphInfoT2(info, glyph_index, x0, y0, x1, y1);
    }
    else {
        int g = stbtt__GetGlyfOffset(info, glyph_index);
        if (g < 0) return 0;

        if (x0) *x0 = ttSHORT(info->data + g + 2);
        if (y0) *y0 = ttSHORT(info->data + g + 4);
        if (x1) *x1 = ttSHORT(info->data + g + 6);
        if (y1) *y1 = ttSHORT(info->data + g + 8);
    }
    return 1;
}

STBTT_DEF int stbtt_GetCodepointBox(const stbtt_fontinfo* info, int codepoint, int* x0, int* y0, int* x1, int* y1)
{
    return stbtt_GetGlyphBox(info, stbtt_FindGlyphIndex(info, codepoint), x0, y0, x1, y1);
}

STBTT_DEF int stbtt_IsGlyphEmpty(const stbtt_fontinfo* info, int glyph_index)
{
    stbtt_int16 numberOfContours;
    int g;
    if (info->cff.size)
        return stbtt__GetGlyphInfoT2(info, glyph_index, NULL, NULL, NULL, NULL) == 0;
    g = stbtt__GetGlyfOffset(info, glyph_index);
    if (g < 0) return 1;
    numberOfContours = ttSHORT(info->data + g);
    return numberOfContours == 0;
}

static int stbtt__close_shape(stbtt_vertex* vertices, int num_vertices, int was_off, int start_off,
    stbtt_int32 sx, stbtt_int32 sy, stbtt_int32 scx, stbtt_int32 scy, stbtt_int32 cx, stbtt_int32 cy)
{
    if (start_off) {
        if (was_off)
            stbtt_setvertex(&vertices[num_vertices++], STBTT_vcurve, (cx + scx) >> 1, (cy + scy) >> 1, cx, cy);
        stbtt_setvertex(&vertices[num_vertices++], STBTT_vcurve, sx, sy, scx, scy);
    }
    else {
        if (was_off)
            stbtt_setvertex(&vertices[num_vertices++], STBTT_vcurve, sx, sy, cx, cy);
        else
            stbtt_setvertex(&vertices[num_vertices++], STBTT_vline, sx, sy, 0, 0);
    }
    return num_vertices;
}

static int stbtt__GetGlyphShapeTT(const stbtt_fontinfo* info, int glyph_index, stbtt_vertex** pvertices)
{
    stbtt_int16 numberOfContours;
    stbtt_uint8* endPtsOfContours;
    stbtt_uint8* data = info->data;
    stbtt_vertex* vertices = 0;
    int num_vertices = 0;
    int g = stbtt__GetGlyfOffset(info, glyph_index);

    *pvertices = NULL;

    if (g < 0) return 0;

    numberOfContours = ttSHORT(data + g);

    if (numberOfContours > 0) {
        stbtt_uint8 flags = 0, flagcount;
        stbtt_int32 ins, i, j = 0, m, n, next_move, was_off = 0, off, start_off = 0;
        stbtt_int32 x, y, cx, cy, sx, sy, scx, scy;
        stbtt_uint8* points;
        endPtsOfContours = (data + g + 10);
        ins = ttUSHORT(data + g + 10 + numberOfContours * 2);
        points = data + g + 10 + numberOfContours * 2 + 2 + ins;

        n = 1 + ttUSHORT(endPtsOfContours + numberOfContours * 2 - 2);

        m = n + 2 * numberOfContours;  // a loose bound on how many vertices we might need
        vertices = (stbtt_vertex*)STBTT_malloc(m * sizeof(vertices[0]), info->userdata);
        if (vertices == 0)
            return 0;

        next_move = 0;
        flagcount = 0;

        // in first pass, we load uninterpreted data into the allocated array
        // above, shifted to the end of the array so we won't overwrite it when
        // we create our final data starting from the front

        off = m - n; // starting offset for uninterpreted data, regardless of how m ends up being calculated

        // first load flags

        for (i = 0; i < n; ++i) {
            if (flagcount == 0) {
                flags = *points++;
                if (flags & 8)
                    flagcount = *points++;
            }
            else
                --flagcount;
            vertices[off + i].type = flags;
        }

        // now load x coordinates
        x = 0;
        for (i = 0; i < n; ++i) {
            flags = vertices[off + i].type;
            if (flags & 2) {
                stbtt_int16 dx = *points++;
                x += (flags & 16) ? dx : -dx; // ???
            }
            else {
                if (!(flags & 16)) {
                    x = x + (stbtt_int16)(points[0] * 256 + points[1]);
                    points += 2;
                }
            }
            vertices[off + i].x = (stbtt_int16)x;
        }

        // now load y coordinates
        y = 0;
        for (i = 0; i < n; ++i) {
            flags = vertices[off + i].type;
            if (flags & 4) {
                stbtt_int16 dy = *points++;
                y += (flags & 32) ? dy : -dy; // ???
            }
            else {
                if (!(flags & 32)) {
                    y = y + (stbtt_int16)(points[0] * 256 + points[1]);
                    points += 2;
                }
            }
            vertices[off + i].y = (stbtt_int16)y;
        }

        // now convert them to our format
        num_vertices = 0;
        sx = sy = cx = cy = scx = scy = 0;
        for (i = 0; i < n; ++i) {
            flags = vertices[off + i].type;
            x = (stbtt_int16)vertices[off + i].x;
            y = (stbtt_int16)vertices[off + i].y;

            if (next_move == i) {
                if (i != 0)
                    num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);

                // now start the new one
                start_off = !(flags & 1);
                if (start_off) {
                    // if we start off with an off-curve point, then when we need to find a point on the curve
                    // where we can start, and we need to save some state for when we wraparound.
                    scx = x;
                    scy = y;
                    if (!(vertices[off + i + 1].type & 1)) {
                        // next point is also a curve point, so interpolate an on-point curve
                        sx = (x + (stbtt_int32)vertices[off + i + 1].x) >> 1;
                        sy = (y + (stbtt_int32)vertices[off + i + 1].y) >> 1;
                    }
                    else {
                        // otherwise just use the next point as our start point
                        sx = (stbtt_int32)vertices[off + i + 1].x;
                        sy = (stbtt_int32)vertices[off + i + 1].y;
                        ++i; // we're using point i+1 as the starting point, so skip it
                    }
                }
                else {
                    sx = x;
                    sy = y;
                }
                stbtt_setvertex(&vertices[num_vertices++], STBTT_vmove, sx, sy, 0, 0);
                was_off = 0;
                next_move = 1 + ttUSHORT(endPtsOfContours + j * 2);
                ++j;
            }
            else {
                if (!(flags & 1)) { // if it's a curve
                    if (was_off) // two off-curve control points in a row means interpolate an on-curve midpoint
                        stbtt_setvertex(&vertices[num_vertices++], STBTT_vcurve, (cx + x) >> 1, (cy + y) >> 1, cx, cy);
                    cx = x;
                    cy = y;
                    was_off = 1;
                }
                else {
                    if (was_off)
                        stbtt_setvertex(&vertices[num_vertices++], STBTT_vcurve, x, y, cx, cy);
                    else
                        stbtt_setvertex(&vertices[num_vertices++], STBTT_vline, x, y, 0, 0);
                    was_off = 0;
                }
            }
        }
        num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);
    }
    else if (numberOfContours < 0) {
        // Compound shapes.
        int more = 1;
        stbtt_uint8* comp = data + g + 10;
        num_vertices = 0;
        vertices = 0;
        while (more) {
            stbtt_uint16 flags, gidx;
            int comp_num_verts = 0, i;
            stbtt_vertex* comp_verts = 0, * tmp = 0;
            float mtx[6] = { 1,0,0,1,0,0 }, m, n;

            flags = ttSHORT(comp); comp += 2;
            gidx = ttSHORT(comp); comp += 2;

            if (flags & 2) { // XY values
                if (flags & 1) { // shorts
                    mtx[4] = ttSHORT(comp); comp += 2;
                    mtx[5] = ttSHORT(comp); comp += 2;
                }
                else {
                    mtx[4] = ttCHAR(comp); comp += 1;
                    mtx[5] = ttCHAR(comp); comp += 1;
                }
            }
            else {
                // @TODO handle matching point
                STBTT_assert(0);
            }
            if (flags & (1 << 3)) { // WE_HAVE_A_SCALE
                mtx[0] = mtx[3] = ttSHORT(comp) / 16384.0f; comp += 2;
                mtx[1] = mtx[2] = 0;
            }
            else if (flags & (1 << 6)) { // WE_HAVE_AN_X_AND_YSCALE
                mtx[0] = ttSHORT(comp) / 16384.0f; comp += 2;
                mtx[1] = mtx[2] = 0;
                mtx[3] = ttSHORT(comp) / 16384.0f; comp += 2;
            }
            else if (flags & (1 << 7)) { // WE_HAVE_A_TWO_BY_TWO
                mtx[0] = ttSHORT(comp) / 16384.0f; comp += 2;
                mtx[1] = ttSHORT(comp) / 16384.0f; comp += 2;
                mtx[2] = ttSHORT(comp) / 16384.0f; comp += 2;
                mtx[3] = ttSHORT(comp) / 16384.0f; comp += 2;
            }

            // Find transformation scales.
            m = (float)STBTT_sqrt(mtx[0] * mtx[0] + mtx[1] * mtx[1]);
            n = (float)STBTT_sqrt(mtx[2] * mtx[2] + mtx[3] * mtx[3]);

            // Get indexed glyph.
            comp_num_verts = stbtt_GetGlyphShape(info, gidx, &comp_verts);
            if (comp_num_verts > 0) {
                // Transform vertices.
                for (i = 0; i < comp_num_verts; ++i) {
                    stbtt_vertex* v = &comp_verts[i];
                    stbtt_vertex_type x, y;
                    x = v->x; y = v->y;
                    v->x = (stbtt_vertex_type)(m * (mtx[0] * x + mtx[2] * y + mtx[4]));
                    v->y = (stbtt_vertex_type)(n * (mtx[1] * x + mtx[3] * y + mtx[5]));
                    x = v->cx; y = v->cy;
                    v->cx = (stbtt_vertex_type)(m * (mtx[0] * x + mtx[2] * y + mtx[4]));
                    v->cy = (stbtt_vertex_type)(n * (mtx[1] * x + mtx[3] * y + mtx[5]));
                }
                // Append vertices.
                tmp = (stbtt_vertex*)STBTT_malloc((num_vertices + comp_num_verts) * sizeof(stbtt_vertex), info->userdata);
                if (!tmp) {
                    if (vertices) STBTT_free(vertices, info->userdata);
                    if (comp_verts) STBTT_free(comp_verts, info->userdata);
                    return 0;
                }
                if (num_vertices > 0) STBTT_memcpy(tmp, vertices, num_vertices * sizeof(stbtt_vertex));
                STBTT_memcpy(tmp + num_vertices, comp_verts, comp_num_verts * sizeof(stbtt_vertex));
                if (vertices) STBTT_free(vertices, info->userdata);
                vertices = tmp;
                STBTT_free(comp_verts, info->userdata);
                num_vertices += comp_num_verts;
            }
            // More components ?
            more = flags & (1 << 5);
        }
    }
    else {
        // numberOfCounters == 0, do nothing
    }

    *pvertices = vertices;
    return num_vertices;
}

typedef struct
{
    int bounds;
    int started;
    float first_x, first_y;
    float x, y;
    stbtt_int32 min_x, max_x, min_y, max_y;

    stbtt_vertex* pvertices;
    int num_vertices;
} stbtt__csctx;

#define STBTT__CSCTX_INIT(bounds) {bounds,0, 0,0, 0,0, 0,0,0,0, NULL, 0}

static void stbtt__track_vertex(stbtt__csctx* c, stbtt_int32 x, stbtt_int32 y)
{
    if (x > c->max_x || !c->started) c->max_x = x;
    if (y > c->max_y || !c->started) c->max_y = y;
    if (x < c->min_x || !c->started) c->min_x = x;
    if (y < c->min_y || !c->started) c->min_y = y;
    c->started = 1;
}

static void stbtt__csctx_v(stbtt__csctx* c, stbtt_uint8 type, stbtt_int32 x, stbtt_int32 y, stbtt_int32 cx, stbtt_int32 cy, stbtt_int32 cx1, stbtt_int32 cy1)
{
    if (c->bounds) {
        stbtt__track_vertex(c, x, y);
        if (type == STBTT_vcubic) {
            stbtt__track_vertex(c, cx, cy);
            stbtt__track_vertex(c, cx1, cy1);
        }
    }
    else {
        stbtt_setvertex(&c->pvertices[c->num_vertices], type, x, y, cx, cy);
        c->pvertices[c->num_vertices].cx1 = (stbtt_int16)cx1;
        c->pvertices[c->num_vertices].cy1 = (stbtt_int16)cy1;
    }
    c->num_vertices++;
}

static void stbtt__csctx_close_shape(stbtt__csctx* ctx)
{
    if (ctx->first_x != ctx->x || ctx->first_y != ctx->y)
        stbtt__csctx_v(ctx, STBTT_vline, (int)ctx->first_x, (int)ctx->first_y, 0, 0, 0, 0);
}

static void stbtt__csctx_rmove_to(stbtt__csctx* ctx, float dx, float dy)
{
    stbtt__csctx_close_shape(ctx);
    ctx->first_x = ctx->x = ctx->x + dx;
    ctx->first_y = ctx->y = ctx->y + dy;
    stbtt__csctx_v(ctx, STBTT_vmove, (int)ctx->x, (int)ctx->y, 0, 0, 0, 0);
}

static void stbtt__csctx_rline_to(stbtt__csctx* ctx, float dx, float dy)
{
    ctx->x += dx;
    ctx->y += dy;
    stbtt__csctx_v(ctx, STBTT_vline, (int)ctx->x, (int)ctx->y, 0, 0, 0, 0);
}

static void stbtt__csctx_rccurve_to(stbtt__csctx* ctx, float dx1, float dy1, float dx2, float dy2, float dx3, float dy3)
{
    float cx1 = ctx->x + dx1;
    float cy1 = ctx->y + dy1;
    float cx2 = cx1 + dx2;
    float cy2 = cy1 + dy2;
    ctx->x = cx2 + dx3;
    ctx->y = cy2 + dy3;
    stbtt__csctx_v(ctx, STBTT_vcubic, (int)ctx->x, (int)ctx->y, (int)cx1, (int)cy1, (int)cx2, (int)cy2);
}

static stbtt__buf stbtt__get_subr(stbtt__buf idx, int n)
{
    int count = stbtt__cff_index_count(&idx);
    int bias = 107;
    if (count >= 33900)
        bias = 32768;
    else if (count >= 1240)
        bias = 1131;
    n += bias;
    if (n < 0 || n >= count)
        return stbtt__new_buf(NULL, 0);
    return stbtt__cff_index_get(idx, n);
}

static stbtt__buf stbtt__cid_get_glyph_subrs(const stbtt_fontinfo* info, int glyph_index)
{
    stbtt__buf fdselect = info->fdselect;
    int nranges, start, end, v, fmt, fdselector = -1, i;

    stbtt__buf_seek(&fdselect, 0);
    fmt = stbtt__buf_get8(&fdselect);
    if (fmt == 0) {
        // untested
        stbtt__buf_skip(&fdselect, glyph_index);
        fdselector = stbtt__buf_get8(&fdselect);
    }
    else if (fmt == 3) {
        nranges = stbtt__buf_get16(&fdselect);
        start = stbtt__buf_get16(&fdselect);
        for (i = 0; i < nranges; i++) {
            v = stbtt__buf_get8(&fdselect);
            end = stbtt__buf_get16(&fdselect);
            if (glyph_index >= start && glyph_index < end) {
                fdselector = v;
                break;
            }
            start = end;
        }
    }
    if (fdselector == -1) stbtt__new_buf(NULL, 0);
    return stbtt__get_subrs(info->cff, stbtt__cff_index_get(info->fontdicts, fdselector));
}

static int stbtt__run_charstring(const stbtt_fontinfo* info, int glyph_index, stbtt__csctx* c)
{
    int in_header = 1, maskbits = 0, subr_stack_height = 0, sp = 0, v, i, b0;
    int has_subrs = 0, clear_stack;
    float s[48];
    stbtt__buf subr_stack[10], subrs = info->subrs, b;
    float f;

#define STBTT__CSERR(s) (0)

    // this currently ignores the initial width value, which isn't needed if we have hmtx
    b = stbtt__cff_index_get(info->charstrings, glyph_index);
    while (b.cursor < b.size) {
        i = 0;
        clear_stack = 1;
        b0 = stbtt__buf_get8(&b);
        switch (b0) {
            // @TODO implement hinting
        case 0x13: // hintmask
        case 0x14: // cntrmask
            if (in_header)
                maskbits += (sp / 2); // implicit "vstem"
            in_header = 0;
            stbtt__buf_skip(&b, (maskbits + 7) / 8);
            break;

        case 0x01: // hstem
        case 0x03: // vstem
        case 0x12: // hstemhm
        case 0x17: // vstemhm
            maskbits += (sp / 2);
            break;

        case 0x15: // rmoveto
            in_header = 0;
            if (sp < 2) return STBTT__CSERR("rmoveto stack");
            stbtt__csctx_rmove_to(c, s[sp - 2], s[sp - 1]);
            break;
        case 0x04: // vmoveto
            in_header = 0;
            if (sp < 1) return STBTT__CSERR("vmoveto stack");
            stbtt__csctx_rmove_to(c, 0, s[sp - 1]);
            break;
        case 0x16: // hmoveto
            in_header = 0;
            if (sp < 1) return STBTT__CSERR("hmoveto stack");
            stbtt__csctx_rmove_to(c, s[sp - 1], 0);
            break;

        case 0x05: // rlineto
            if (sp < 2) return STBTT__CSERR("rlineto stack");
            for (; i + 1 < sp; i += 2)
                stbtt__csctx_rline_to(c, s[i], s[i + 1]);
            break;

            // hlineto/vlineto and vhcurveto/hvcurveto alternate horizontal and vertical
            // starting from a different place.

        case 0x07: // vlineto
            if (sp < 1) return STBTT__CSERR("vlineto stack");
            goto vlineto;
        case 0x06: // hlineto
            if (sp < 1) return STBTT__CSERR("hlineto stack");
            for (;;) {
                if (i >= sp) break;
                stbtt__csctx_rline_to(c, s[i], 0);
                i++;
            vlineto:
                if (i >= sp) break;
                stbtt__csctx_rline_to(c, 0, s[i]);
                i++;
            }
            break;

        case 0x1F: // hvcurveto
            if (sp < 4) return STBTT__CSERR("hvcurveto stack");
            goto hvcurveto;
        case 0x1E: // vhcurveto
            if (sp < 4) return STBTT__CSERR("vhcurveto stack");
            for (;;) {
                if (i + 3 >= sp) break;
                stbtt__csctx_rccurve_to(c, 0, s[i], s[i + 1], s[i + 2], s[i + 3], (sp - i == 5) ? s[i + 4] : 0.0f);
                i += 4;
            hvcurveto:
                if (i + 3 >= sp) break;
                stbtt__csctx_rccurve_to(c, s[i], 0, s[i + 1], s[i + 2], (sp - i == 5) ? s[i + 4] : 0.0f, s[i + 3]);
                i += 4;
            }
            break;

        case 0x08: // rrcurveto
            if (sp < 6) return STBTT__CSERR("rcurveline stack");
            for (; i + 5 < sp; i += 6)
                stbtt__csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2], s[i + 3], s[i + 4], s[i + 5]);
            break;

        case 0x18: // rcurveline
            if (sp < 8) return STBTT__CSERR("rcurveline stack");
            for (; i + 5 < sp - 2; i += 6)
                stbtt__csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2], s[i + 3], s[i + 4], s[i + 5]);
            if (i + 1 >= sp) return STBTT__CSERR("rcurveline stack");
            stbtt__csctx_rline_to(c, s[i], s[i + 1]);
            break;

        case 0x19: // rlinecurve
            if (sp < 8) return STBTT__CSERR("rlinecurve stack");
            for (; i + 1 < sp - 6; i += 2)
                stbtt__csctx_rline_to(c, s[i], s[i + 1]);
            if (i + 5 >= sp) return STBTT__CSERR("rlinecurve stack");
            stbtt__csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2], s[i + 3], s[i + 4], s[i + 5]);
            break;

        case 0x1A: // vvcurveto
        case 0x1B: // hhcurveto
            if (sp < 4) return STBTT__CSERR("(vv|hh)curveto stack");
            f = 0.0;
            if (sp & 1) { f = s[i]; i++; }
            for (; i + 3 < sp; i += 4) {
                if (b0 == 0x1B)
                    stbtt__csctx_rccurve_to(c, s[i], f, s[i + 1], s[i + 2], s[i + 3], 0.0);
                else
                    stbtt__csctx_rccurve_to(c, f, s[i], s[i + 1], s[i + 2], 0.0, s[i + 3]);
                f = 0.0;
            }
            break;

        case 0x0A: // callsubr
            if (!has_subrs) {
                if (info->fdselect.size)
                    subrs = stbtt__cid_get_glyph_subrs(info, glyph_index);
                has_subrs = 1;
            }
            // fallthrough
        case 0x1D: // callgsubr
            if (sp < 1) return STBTT__CSERR("call(g|)subr stack");
            v = (int)s[--sp];
            if (subr_stack_height >= 10) return STBTT__CSERR("recursion limit");
            subr_stack[subr_stack_height++] = b;
            b = stbtt__get_subr(b0 == 0x0A ? subrs : info->gsubrs, v);
            if (b.size == 0) return STBTT__CSERR("subr not found");
            b.cursor = 0;
            clear_stack = 0;
            break;

        case 0x0B: // return
            if (subr_stack_height <= 0) return STBTT__CSERR("return outside subr");
            b = subr_stack[--subr_stack_height];
            clear_stack = 0;
            break;

        case 0x0E: // endchar
            stbtt__csctx_close_shape(c);
            return 1;

        case 0x0C: { // two-byte escape
            float dx1, dx2, dx3, dx4, dx5, dx6, dy1, dy2, dy3, dy4, dy5, dy6;
            float dx, dy;
            int b1 = stbtt__buf_get8(&b);
            switch (b1) {
                // @TODO These "flex" implementations ignore the flex-depth and resolution,
                // and always draw beziers.
            case 0x22: // hflex
                if (sp < 7) return STBTT__CSERR("hflex stack");
                dx1 = s[0];
                dx2 = s[1];
                dy2 = s[2];
                dx3 = s[3];
                dx4 = s[4];
                dx5 = s[5];
                dx6 = s[6];
                stbtt__csctx_rccurve_to(c, dx1, 0, dx2, dy2, dx3, 0);
                stbtt__csctx_rccurve_to(c, dx4, 0, dx5, -dy2, dx6, 0);
                break;

            case 0x23: // flex
                if (sp < 13) return STBTT__CSERR("flex stack");
                dx1 = s[0];
                dy1 = s[1];
                dx2 = s[2];
                dy2 = s[3];
                dx3 = s[4];
                dy3 = s[5];
                dx4 = s[6];
                dy4 = s[7];
                dx5 = s[8];
                dy5 = s[9];
                dx6 = s[10];
                dy6 = s[11];
                //fd is s[12]
                stbtt__csctx_rccurve_to(c, dx1, dy1, dx2, dy2, dx3, dy3);
                stbtt__csctx_rccurve_to(c, dx4, dy4, dx5, dy5, dx6, dy6);
                break;

            case 0x24: // hflex1
                if (sp < 9) return STBTT__CSERR("hflex1 stack");
                dx1 = s[0];
                dy1 = s[1];
                dx2 = s[2];
                dy2 = s[3];
                dx3 = s[4];
                dx4 = s[5];
                dx5 = s[6];
                dy5 = s[7];
                dx6 = s[8];
                stbtt__csctx_rccurve_to(c, dx1, dy1, dx2, dy2, dx3, 0);
                stbtt__csctx_rccurve_to(c, dx4, 0, dx5, dy5, dx6, -(dy1 + dy2 + dy5));
                break;

            case 0x25: // flex1
                if (sp < 11) return STBTT__CSERR("flex1 stack");
                dx1 = s[0];
                dy1 = s[1];
                dx2 = s[2];
                dy2 = s[3];
                dx3 = s[4];
                dy3 = s[5];
                dx4 = s[6];
                dy4 = s[7];
                dx5 = s[8];
                dy5 = s[9];
                dx6 = dy6 = s[10];
                dx = dx1 + dx2 + dx3 + dx4 + dx5;
                dy = dy1 + dy2 + dy3 + dy4 + dy5;
                if (STBTT_fabs(dx) > STBTT_fabs(dy))
                    dy6 = -dy;
                else
                    dx6 = -dx;
                stbtt__csctx_rccurve_to(c, dx1, dy1, dx2, dy2, dx3, dy3);
                stbtt__csctx_rccurve_to(c, dx4, dy4, dx5, dy5, dx6, dy6);
                break;

            default:
                return STBTT__CSERR("unimplemented");
            }
        } break;

        default:
            if (b0 != 255 && b0 != 28 && (b0 < 32 || b0 > 254))
                return STBTT__CSERR("reserved operator");

            // push immediate
            if (b0 == 255) {
                f = (float)(stbtt_int32)stbtt__buf_get32(&b) / 0x10000;
            }
            else {
                stbtt__buf_skip(&b, -1);
                f = (float)(stbtt_int16)stbtt__cff_int(&b);
            }
            if (sp >= 48) return STBTT__CSERR("push stack overflow");
            s[sp++] = f;
            clear_stack = 0;
            break;
        }
        if (clear_stack) sp = 0;
    }
    return STBTT__CSERR("no endchar");

#undef STBTT__CSERR
}

static int stbtt__GetGlyphShapeT2(const stbtt_fontinfo* info, int glyph_index, stbtt_vertex** pvertices)
{
    // runs the charstring twice, once to count and once to output (to avoid realloc)
    stbtt__csctx count_ctx = STBTT__CSCTX_INIT(1);
    stbtt__csctx output_ctx = STBTT__CSCTX_INIT(0);
    if (stbtt__run_charstring(info, glyph_index, &count_ctx)) {
        *pvertices = (stbtt_vertex*)STBTT_malloc(count_ctx.num_vertices * sizeof(stbtt_vertex), info->userdata);
        output_ctx.pvertices = *pvertices;
        if (stbtt__run_charstring(info, glyph_index, &output_ctx)) {
            STBTT_assert(output_ctx.num_vertices == count_ctx.num_vertices);
            return output_ctx.num_vertices;
        }
    }
    *pvertices = NULL;
    return 0;
}

static int stbtt__GetGlyphInfoT2(const stbtt_fontinfo* info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
{
    stbtt__csctx c = STBTT__CSCTX_INIT(1);
    int r = stbtt__run_charstring(info, glyph_index, &c);
    if (x0)  *x0 = r ? c.min_x : 0;
    if (y0)  *y0 = r ? c.min_y : 0;
    if (x1)  *x1 = r ? c.max_x : 0;
    if (y1)  *y1 = r ? c.max_y : 0;
    return r ? c.num_vertices : 0;
}

STBTT_DEF int stbtt_GetGlyphShape(const stbtt_fontinfo* info, int glyph_index, stbtt_vertex** pvertices)
{
    if (!info->cff.size)
        return stbtt__GetGlyphShapeTT(info, glyph_index, pvertices);
    else
        return stbtt__GetGlyphShapeT2(info, glyph_index, pvertices);
}

#define STBTT_GPOS_TODO_assert(x)

STBTT_DEF float stbtt_ScaleForPixelHeight(const stbtt_fontinfo* info, float height)
{
    int fheight = ttSHORT(info->data + info->hhea + 4) - ttSHORT(info->data + info->hhea + 6);
    return (float)height / fheight;
}

STBTT_DEF void stbtt_GetGlyphBitmapBoxSubpixel(const stbtt_fontinfo* font, int glyph, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
{
    int x0 = 0, y0 = 0, x1, y1; // =0 suppresses compiler warning
    if (!stbtt_GetGlyphBox(font, glyph, &x0, &y0, &x1, &y1)) {
        // e.g. space character
        if (ix0) *ix0 = 0;
        if (iy0) *iy0 = 0;
        if (ix1) *ix1 = 0;
        if (iy1) *iy1 = 0;
    }
    else {
        // move to integral bboxes (treating pixels as little squares, what pixels get touched)?
        if (ix0) *ix0 = STBTT_ifloor(x0 * scale_x + shift_x);
        if (iy0) *iy0 = STBTT_ifloor(-y1 * scale_y + shift_y);
        if (ix1) *ix1 = STBTT_iceil(x1 * scale_x + shift_x);
        if (iy1) *iy1 = STBTT_iceil(-y0 * scale_y + shift_y);
    }
}

STBTT_DEF void stbtt_GetGlyphBitmapBox(const stbtt_fontinfo* font, int glyph, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1)
{
    stbtt_GetGlyphBitmapBoxSubpixel(font, glyph, scale_x, scale_y, 0.0f, 0.0f, ix0, iy0, ix1, iy1);
}

STBTT_DEF void stbtt_GetCodepointBitmapBoxSubpixel(const stbtt_fontinfo* font, int codepoint, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
{
    stbtt_GetGlyphBitmapBoxSubpixel(font, stbtt_FindGlyphIndex(font, codepoint), scale_x, scale_y, shift_x, shift_y, ix0, iy0, ix1, iy1);
}

STBTT_DEF void stbtt_GetCodepointBitmapBox(const stbtt_fontinfo* font, int codepoint, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1)
{
    stbtt_GetCodepointBitmapBoxSubpixel(font, codepoint, scale_x, scale_y, 0.0f, 0.0f, ix0, iy0, ix1, iy1);
}

//////////////////////////////////////////////////////////////////////////////
//
//  Rasterizer

typedef struct stbtt__hheap_chunk
{
    struct stbtt__hheap_chunk* next;
} stbtt__hheap_chunk;

typedef struct stbtt__hheap
{
    struct stbtt__hheap_chunk* head;
    void* first_free;
    int    num_remaining_in_head_chunk;
} stbtt__hheap;

static void* stbtt__hheap_alloc(stbtt__hheap* hh, size_t size, void* userdata)
{
    if (hh->first_free) {
        void* p = hh->first_free;
        hh->first_free = *(void**)p;
        return p;
    }
    else {
        if (hh->num_remaining_in_head_chunk == 0) {
            int count = (size < 32 ? 2000 : size < 128 ? 800 : 100);
            stbtt__hheap_chunk* c = (stbtt__hheap_chunk*)STBTT_malloc(sizeof(stbtt__hheap_chunk) + size * count, userdata);
            if (c == NULL)
                return NULL;
            c->next = hh->head;
            hh->head = c;
            hh->num_remaining_in_head_chunk = count;
        }
        --hh->num_remaining_in_head_chunk;
        return (char*)(hh->head) + sizeof(stbtt__hheap_chunk) + size * hh->num_remaining_in_head_chunk;
    }
}

static void stbtt__hheap_free(stbtt__hheap* hh, void* p)
{
    *(void**)p = hh->first_free;
    hh->first_free = p;
}

static void stbtt__hheap_cleanup(stbtt__hheap* hh, void* userdata)
{
    stbtt__hheap_chunk* c = hh->head;
    while (c) {
        stbtt__hheap_chunk* n = c->next;
        STBTT_free(c, userdata);
        c = n;
    }
}

typedef struct stbtt__edge {
    float x0, y0, x1, y1;
    int invert;
} stbtt__edge;


typedef struct stbtt__active_edge
{
    struct stbtt__active_edge* next;
#if STBTT_RASTERIZER_VERSION==1
    int x, dx;
    float ey;
    int direction;
#elif STBTT_RASTERIZER_VERSION==2
    float fx, fdx, fdy;
    float direction;
    float sy;
    float ey;
#else
#error "Unrecognized value of STBTT_RASTERIZER_VERSION"
#endif
} stbtt__active_edge;

static stbtt__active_edge* stbtt__new_active(stbtt__hheap* hh, stbtt__edge* e, int off_x, float start_point, void* userdata)
{
    stbtt__active_edge* z = (stbtt__active_edge*)stbtt__hheap_alloc(hh, sizeof(*z), userdata);
    float dxdy = (e->x1 - e->x0) / (e->y1 - e->y0);
    STBTT_assert(z != NULL);
    //STBTT_assert(e->y0 <= start_point);
    if (!z) return z;
    z->fdx = dxdy;
    z->fdy = dxdy != 0.0f ? (1.0f / dxdy) : 0.0f;
    z->fx = e->x0 + dxdy * (start_point - e->y0);
    z->fx -= off_x;
    z->direction = e->invert ? 1.0f : -1.0f;
    z->sy = e->y0;
    z->ey = e->y1;
    z->next = 0;
    return z;
}

// the edge passed in here does not cross the vertical line at x or the vertical line at x+1
// (i.e. it has already been clipped to those)
static void stbtt__handle_clipped_edge(float* scanline, int x, stbtt__active_edge* e, float x0, float y0, float x1, float y1)
{
    if (y0 == y1) return;
    STBTT_assert(y0 < y1);
    STBTT_assert(e->sy <= e->ey);
    if (y0 > e->ey) return;
    if (y1 < e->sy) return;
    if (y0 < e->sy) {
        x0 += (x1 - x0) * (e->sy - y0) / (y1 - y0);
        y0 = e->sy;
    }
    if (y1 > e->ey) {
        x1 += (x1 - x0) * (e->ey - y1) / (y1 - y0);
        y1 = e->ey;
    }

    if (x0 == x)
        STBTT_assert(x1 <= x + 1);
    else if (x0 == x + 1)
        STBTT_assert(x1 >= x);
    else if (x0 <= x)
        STBTT_assert(x1 <= x);
    else if (x0 >= x + 1)
        STBTT_assert(x1 >= x + 1);
    else
        STBTT_assert(x1 >= x && x1 <= x + 1);

    if (x0 <= x && x1 <= x)
        scanline[x] += e->direction * (y1 - y0);
    else if (x0 >= x + 1 && x1 >= x + 1)
        ;
    else {
        STBTT_assert(x0 >= x && x0 <= x + 1 && x1 >= x && x1 <= x + 1);
        scanline[x] += e->direction * (y1 - y0) * (1 - ((x0 - x) + (x1 - x)) / 2); // coverage = 1 - average x position
    }
}

static void stbtt__fill_active_edges_new(float* scanline, float* scanline_fill, int len, stbtt__active_edge* e, float y_top)
{
    float y_bottom = y_top + 1;

    while (e) {
        // brute force every pixel

        // compute intersection points with top & bottom
        STBTT_assert(e->ey >= y_top);

        if (e->fdx == 0) {
            float x0 = e->fx;
            if (x0 < len) {
                if (x0 >= 0) {
                    stbtt__handle_clipped_edge(scanline, (int)x0, e, x0, y_top, x0, y_bottom);
                    stbtt__handle_clipped_edge(scanline_fill - 1, (int)x0 + 1, e, x0, y_top, x0, y_bottom);
                }
                else {
                    stbtt__handle_clipped_edge(scanline_fill - 1, 0, e, x0, y_top, x0, y_bottom);
                }
            }
        }
        else {
            float x0 = e->fx;
            float dx = e->fdx;
            float xb = x0 + dx;
            float x_top, x_bottom;
            float sy0, sy1;
            float dy = e->fdy;
            STBTT_assert(e->sy <= y_bottom && e->ey >= y_top);

            // compute endpoints of line segment clipped to this scanline (if the
            // line segment starts on this scanline. x0 is the intersection of the
            // line with y_top, but that may be off the line segment.
            if (e->sy > y_top) {
                x_top = x0 + dx * (e->sy - y_top);
                sy0 = e->sy;
            }
            else {
                x_top = x0;
                sy0 = y_top;
            }
            if (e->ey < y_bottom) {
                x_bottom = x0 + dx * (e->ey - y_top);
                sy1 = e->ey;
            }
            else {
                x_bottom = xb;
                sy1 = y_bottom;
            }

            if (x_top >= 0 && x_bottom >= 0 && x_top < len && x_bottom < len) {
                // from here on, we don't have to range check x values

                if ((int)x_top == (int)x_bottom) {
                    float height;
                    // simple case, only spans one pixel
                    int x = (int)x_top;
                    height = sy1 - sy0;
                    STBTT_assert(x >= 0 && x < len);
                    scanline[x] += e->direction * (1 - ((x_top - x) + (x_bottom - x)) / 2) * height;
                    scanline_fill[x] += e->direction * height; // everything right of this pixel is filled
                }
                else {
                    int x, x1, x2;
                    float y_crossing, step, sign, area;
                    // covers 2+ pixels
                    if (x_top > x_bottom) {
                        // flip scanline vertically; signed area is the same
                        float t;
                        sy0 = y_bottom - (sy0 - y_top);
                        sy1 = y_bottom - (sy1 - y_top);
                        t = sy0, sy0 = sy1, sy1 = t;
                        t = x_bottom, x_bottom = x_top, x_top = t;
                        dx = -dx;
                        dy = -dy;
                        t = x0, x0 = xb, xb = t;
                    }

                    x1 = (int)x_top;
                    x2 = (int)x_bottom;
                    // compute intersection with y axis at x1+1
                    y_crossing = (x1 + 1 - x0) * dy + y_top;

                    sign = e->direction;
                    // area of the rectangle covered from y0..y_crossing
                    area = sign * (y_crossing - sy0);
                    // area of the triangle (x_top,y0), (x+1,y0), (x+1,y_crossing)
                    scanline[x1] += area * (1 - ((x_top - x1) + (x1 + 1 - x1)) / 2);

                    step = sign * dy;
                    for (x = x1 + 1; x < x2; ++x) {
                        scanline[x] += area + step / 2;
                        area += step;
                    }
                    y_crossing += dy * (x2 - (x1 + 1));

                    STBTT_assert(STBTT_fabs(area) <= 1.01f);

                    scanline[x2] += area + sign * (1 - ((x2 - x2) + (x_bottom - x2)) / 2) * (sy1 - y_crossing);

                    scanline_fill[x2] += sign * (sy1 - sy0);
                }
            }
            else {
                // if edge goes outside of box we're drawing, we require
                // clipping logic. since this does not match the intended use
                // of this library, we use a different, very slow brute
                // force implementation
                int x;
                for (x = 0; x < len; ++x) {
                    // cases:
                    //
                    // there can be up to two intersections with the pixel. any intersection
                    // with left or right edges can be handled by splitting into two (or three)
                    // regions. intersections with top & bottom do not necessitate case-wise logic.
                    //
                    // the old way of doing this found the intersections with the left & right edges,
                    // then used some simple logic to produce up to three segments in sorted order
                    // from top-to-bottom. however, this had a problem: if an x edge was epsilon
                    // across the x border, then the corresponding y position might not be distinct
                    // from the other y segment, and it might ignored as an empty segment. to avoid
                    // that, we need to explicitly produce segments based on x positions.

                    // rename variables to clearly-defined pairs
                    float y0 = y_top;
                    float x1 = (float)(x);
                    float x2 = (float)(x + 1);
                    float x3 = xb;
                    float y3 = y_bottom;

                    // x = e->x + e->dx * (y-y_top)
                    // (y-y_top) = (x - e->x) / e->dx
                    // y = (x - e->x) / e->dx + y_top
                    float y1 = (x - x0) / dx + y_top;
                    float y2 = (x + 1 - x0) / dx + y_top;

                    if (x0 < x1 && x3 > x2) {         // three segments descending down-right
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x1, y1);
                        stbtt__handle_clipped_edge(scanline, x, e, x1, y1, x2, y2);
                        stbtt__handle_clipped_edge(scanline, x, e, x2, y2, x3, y3);
                    }
                    else if (x3 < x1 && x0 > x2) {  // three segments descending down-left
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x2, y2);
                        stbtt__handle_clipped_edge(scanline, x, e, x2, y2, x1, y1);
                        stbtt__handle_clipped_edge(scanline, x, e, x1, y1, x3, y3);
                    }
                    else if (x0 < x1 && x3 > x1) {  // two segments across x, down-right
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x1, y1);
                        stbtt__handle_clipped_edge(scanline, x, e, x1, y1, x3, y3);
                    }
                    else if (x3 < x1 && x0 > x1) {  // two segments across x, down-left
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x1, y1);
                        stbtt__handle_clipped_edge(scanline, x, e, x1, y1, x3, y3);
                    }
                    else if (x0 < x2 && x3 > x2) {  // two segments across x+1, down-right
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x2, y2);
                        stbtt__handle_clipped_edge(scanline, x, e, x2, y2, x3, y3);
                    }
                    else if (x3 < x2 && x0 > x2) {  // two segments across x+1, down-left
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x2, y2);
                        stbtt__handle_clipped_edge(scanline, x, e, x2, y2, x3, y3);
                    }
                    else {  // one segment
                        stbtt__handle_clipped_edge(scanline, x, e, x0, y0, x3, y3);
                    }
                }
            }
        }
        e = e->next;
    }
}

// directly AA rasterize edges w/o supersampling
static void stbtt__rasterize_sorted_edges(stbtt__bitmap* result, stbtt__edge* e, int n, int vsubsample, int off_x, int off_y, void* userdata)
{
    stbtt__hheap hh = { 0, 0, 0 };
    stbtt__active_edge* active = NULL;
    int y, j = 0, i;
    float scanline_data[129], * scanline, * scanline2;

    STBTT__NOTUSED(vsubsample);

    if (result->w > 64)
        scanline = (float*)STBTT_malloc((result->w * 2 + 1) * sizeof(float), userdata);
    else
        scanline = scanline_data;

    scanline2 = scanline + result->w;

    y = off_y;
    e[n].y0 = (float)(off_y + result->h) + 1;

    while (j < result->h) {
        // find center of pixel for this scanline
        float scan_y_top = y + 0.0f;
        float scan_y_bottom = y + 1.0f;
        stbtt__active_edge** step = &active;

        STBTT_memset(scanline, 0, result->w * sizeof(scanline[0]));
        STBTT_memset(scanline2, 0, (result->w + 1) * sizeof(scanline[0]));

        // update all active edges;
        // remove all active edges that terminate before the top of this scanline
        while (*step) {
            stbtt__active_edge* z = *step;
            if (z->ey <= scan_y_top) {
                *step = z->next; // delete from list
                STBTT_assert(z->direction);
                z->direction = 0;
                stbtt__hheap_free(&hh, z);
            }
            else {
                step = &((*step)->next); // advance through list
            }
        }

        // insert all edges that start before the bottom of this scanline
        while (e->y0 <= scan_y_bottom) {
            if (e->y0 != e->y1) {
                stbtt__active_edge* z = stbtt__new_active(&hh, e, off_x, scan_y_top, userdata);
                if (z != NULL) {
                    if (j == 0 && off_y != 0) {
                        if (z->ey < scan_y_top) {
                            // this can happen due to subpixel positioning and some kind of fp rounding error i think
                            z->ey = scan_y_top;
                        }
                    }
                    STBTT_assert(z->ey >= scan_y_top); // if we get really unlucky a tiny bit of an edge can be out of bounds
                    // insert at front
                    z->next = active;
                    active = z;
                }
            }
            ++e;
        }

        // now process all active edges
        if (active)
            stbtt__fill_active_edges_new(scanline, scanline2 + 1, result->w, active, scan_y_top);

        {
            float sum = 0;
            for (i = 0; i < result->w; ++i) {
                float k;
                int m;
                sum += scanline2[i];
                k = scanline[i] + sum;
                k = (float)STBTT_fabs(k) * 255 + 0.5f;
                m = (int)k;
                if (m > 255) m = 255;
                result->pixels[j * result->stride + i] = (unsigned char)m;
            }
        }
        // advance all the edges
        step = &active;
        while (*step) {
            stbtt__active_edge* z = *step;
            z->fx += z->fdx; // advance to position for current scanline
            step = &((*step)->next); // advance through list
        }

        ++y;
        ++j;
    }

    stbtt__hheap_cleanup(&hh, userdata);

    if (scanline != scanline_data)
        STBTT_free(scanline, userdata);
}

#define STBTT__COMPARE(a,b)  ((a)->y0 < (b)->y0)

static void stbtt__sort_edges_ins_sort(stbtt__edge* p, int n)
{
    int i, j;
    for (i = 1; i < n; ++i) {
        stbtt__edge t = p[i], * a = &t;
        j = i;
        while (j > 0) {
            stbtt__edge* b = &p[j - 1];
            int c = STBTT__COMPARE(a, b);
            if (!c) break;
            p[j] = p[j - 1];
            --j;
        }
        if (i != j)
            p[j] = t;
    }
}

static void stbtt__sort_edges_quicksort(stbtt__edge* p, int n)
{
    /* threshold for transitioning to insertion sort */
    while (n > 12) {
        stbtt__edge t;
        int c01, c12, c, m, i, j;

        /* compute median of three */
        m = n >> 1;
        c01 = STBTT__COMPARE(&p[0], &p[m]);
        c12 = STBTT__COMPARE(&p[m], &p[n - 1]);
        /* if 0 >= mid >= end, or 0 < mid < end, then use mid */
        if (c01 != c12) {
            /* otherwise, we'll need to swap something else to middle */
            int z;
            c = STBTT__COMPARE(&p[0], &p[n - 1]);
            /* 0>mid && mid<n:  0>n => n; 0<n => 0 */
            /* 0<mid && mid>n:  0>n => 0; 0<n => n */
            z = (c == c12) ? 0 : n - 1;
            t = p[z];
            p[z] = p[m];
            p[m] = t;
        }
        /* now p[m] is the median-of-three */
        /* swap it to the beginning so it won't move around */
        t = p[0];
        p[0] = p[m];
        p[m] = t;

        /* partition loop */
        i = 1;
        j = n - 1;
        for (;;) {
            /* handling of equality is crucial here */
            /* for sentinels & efficiency with duplicates */
            for (;;++i) {
                if (!STBTT__COMPARE(&p[i], &p[0])) break;
            }
            for (;;--j) {
                if (!STBTT__COMPARE(&p[0], &p[j])) break;
            }
            /* make sure we haven't crossed */
            if (i >= j) break;
            t = p[i];
            p[i] = p[j];
            p[j] = t;

            ++i;
            --j;
        }
        /* recurse on smaller side, iterate on larger */
        if (j < (n - i)) {
            stbtt__sort_edges_quicksort(p, j);
            p = p + i;
            n = n - i;
        }
        else {
            stbtt__sort_edges_quicksort(p + i, n - i);
            n = j;
        }
    }
}

static void stbtt__sort_edges(stbtt__edge* p, int n)
{
    stbtt__sort_edges_quicksort(p, n);
    stbtt__sort_edges_ins_sort(p, n);
}

typedef struct
{
    float x, y;
} stbtt__point;

static void stbtt__rasterize(stbtt__bitmap* result, stbtt__point* pts, int* wcount, int windings, float scale_x, float scale_y, float shift_x, float shift_y, int off_x, int off_y, int invert, void* userdata)
{
    float y_scale_inv = invert ? -scale_y : scale_y;
    stbtt__edge* e;
    int n, i, j, k, m;
#if STBTT_RASTERIZER_VERSION == 1
    int vsubsample = result->h < 8 ? 15 : 5;
#elif STBTT_RASTERIZER_VERSION == 2
    int vsubsample = 1;
#else
#error "Unrecognized value of STBTT_RASTERIZER_VERSION"
#endif
    // vsubsample should divide 255 evenly; otherwise we won't reach full opacity

    // now we have to blow out the windings into explicit edge lists
    n = 0;
    for (i = 0; i < windings; ++i)
        n += wcount[i];

    e = (stbtt__edge*)STBTT_malloc(sizeof(*e) * (n + 1), userdata); // add an extra one as a sentinel
    if (e == 0) return;
    n = 0;

    m = 0;
    for (i = 0; i < windings; ++i) {
        stbtt__point* p = pts + m;
        m += wcount[i];
        j = wcount[i] - 1;
        for (k = 0; k < wcount[i]; j = k++) {
            int a = k, b = j;
            // skip the edge if horizontal
            if (p[j].y == p[k].y)
                continue;
            // add edge from j to k to the list
            e[n].invert = 0;
            if (invert ? p[j].y > p[k].y : p[j].y < p[k].y) {
                e[n].invert = 1;
                a = j, b = k;
            }
            e[n].x0 = p[a].x * scale_x + shift_x;
            e[n].y0 = (p[a].y * y_scale_inv + shift_y) * vsubsample;
            e[n].x1 = p[b].x * scale_x + shift_x;
            e[n].y1 = (p[b].y * y_scale_inv + shift_y) * vsubsample;
            ++n;
        }
    }

    // now sort the edges by their highest point (should snap to integer, and then by x)
    //STBTT_sort(e, n, sizeof(e[0]), stbtt__edge_compare);
    stbtt__sort_edges(e, n);

    // now, traverse the scanlines and find the intersections on each scanline, use xor winding rule
    stbtt__rasterize_sorted_edges(result, e, n, vsubsample, off_x, off_y, userdata);

    STBTT_free(e, userdata);
}

static void stbtt__add_point(stbtt__point* points, int n, float x, float y)
{
    if (!points) return; // during first pass, it's unallocated
    points[n].x = x;
    points[n].y = y;
}

// tessellate until threshold p is happy... @TODO warped to compensate for non-linear stretching
static int stbtt__tesselate_curve(stbtt__point* points, int* num_points, float x0, float y0, float x1, float y1, float x2, float y2, float objspace_flatness_squared, int n)
{
    // midpoint
    float mx = (x0 + 2 * x1 + x2) / 4;
    float my = (y0 + 2 * y1 + y2) / 4;
    // versus directly drawn line
    float dx = (x0 + x2) / 2 - mx;
    float dy = (y0 + y2) / 2 - my;
    if (n > 16) // 65536 segments on one curve better be enough!
        return 1;
    if (dx * dx + dy * dy > objspace_flatness_squared) { // half-pixel error allowed... need to be smaller if AA
        stbtt__tesselate_curve(points, num_points, x0, y0, (x0 + x1) / 2.0f, (y0 + y1) / 2.0f, mx, my, objspace_flatness_squared, n + 1);
        stbtt__tesselate_curve(points, num_points, mx, my, (x1 + x2) / 2.0f, (y1 + y2) / 2.0f, x2, y2, objspace_flatness_squared, n + 1);
    }
    else {
        stbtt__add_point(points, *num_points, x2, y2);
        *num_points = *num_points + 1;
    }
    return 1;
}

static void stbtt__tesselate_cubic(stbtt__point* points, int* num_points, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float objspace_flatness_squared, int n)
{
    // @TODO this "flatness" calculation is just made-up nonsense that seems to work well enough
    float dx0 = x1 - x0;
    float dy0 = y1 - y0;
    float dx1 = x2 - x1;
    float dy1 = y2 - y1;
    float dx2 = x3 - x2;
    float dy2 = y3 - y2;
    float dx = x3 - x0;
    float dy = y3 - y0;
    float longlen = (float)(STBTT_sqrt(dx0 * dx0 + dy0 * dy0) + STBTT_sqrt(dx1 * dx1 + dy1 * dy1) + STBTT_sqrt(dx2 * dx2 + dy2 * dy2));
    float shortlen = (float)STBTT_sqrt(dx * dx + dy * dy);
    float flatness_squared = longlen * longlen - shortlen * shortlen;

    if (n > 16) // 65536 segments on one curve better be enough!
        return;

    if (flatness_squared > objspace_flatness_squared) {
        float x01 = (x0 + x1) / 2;
        float y01 = (y0 + y1) / 2;
        float x12 = (x1 + x2) / 2;
        float y12 = (y1 + y2) / 2;
        float x23 = (x2 + x3) / 2;
        float y23 = (y2 + y3) / 2;

        float xa = (x01 + x12) / 2;
        float ya = (y01 + y12) / 2;
        float xb = (x12 + x23) / 2;
        float yb = (y12 + y23) / 2;

        float mx = (xa + xb) / 2;
        float my = (ya + yb) / 2;

        stbtt__tesselate_cubic(points, num_points, x0, y0, x01, y01, xa, ya, mx, my, objspace_flatness_squared, n + 1);
        stbtt__tesselate_cubic(points, num_points, mx, my, xb, yb, x23, y23, x3, y3, objspace_flatness_squared, n + 1);
    }
    else {
        stbtt__add_point(points, *num_points, x3, y3);
        *num_points = *num_points + 1;
    }
}

// returns number of contours
static stbtt__point* stbtt_FlattenCurves(stbtt_vertex* vertices, int num_verts, float objspace_flatness, int** contour_lengths, int* num_contours, void* userdata)
{
    stbtt__point* points = 0;
    int num_points = 0;

    float objspace_flatness_squared = objspace_flatness * objspace_flatness;
    int i, n = 0, start = 0, pass;

    // count how many "moves" there are to get the contour count
    for (i = 0; i < num_verts; ++i)
        if (vertices[i].type == STBTT_vmove)
            ++n;

    *num_contours = n;
    if (n == 0) return 0;

    *contour_lengths = (int*)STBTT_malloc(sizeof(**contour_lengths) * n, userdata);

    if (*contour_lengths == 0) {
        *num_contours = 0;
        return 0;
    }

    // make two passes through the points so we don't need to realloc
    for (pass = 0; pass < 2; ++pass) {
        float x = 0, y = 0;
        if (pass == 1) {
            points = (stbtt__point*)STBTT_malloc(num_points * sizeof(points[0]), userdata);
            if (points == NULL) goto error;
        }
        num_points = 0;
        n = -1;
        for (i = 0; i < num_verts; ++i) {
            switch (vertices[i].type) {
            case STBTT_vmove:
                // start the next contour
                if (n >= 0)
                    (*contour_lengths)[n] = num_points - start;
                ++n;
                start = num_points;

                x = vertices[i].x, y = vertices[i].y;
                stbtt__add_point(points, num_points++, x, y);
                break;
            case STBTT_vline:
                x = vertices[i].x, y = vertices[i].y;
                stbtt__add_point(points, num_points++, x, y);
                break;
            case STBTT_vcurve:
                stbtt__tesselate_curve(points, &num_points, x, y,
                    vertices[i].cx, vertices[i].cy,
                    vertices[i].x, vertices[i].y,
                    objspace_flatness_squared, 0);
                x = vertices[i].x, y = vertices[i].y;
                break;
            case STBTT_vcubic:
                stbtt__tesselate_cubic(points, &num_points, x, y,
                    vertices[i].cx, vertices[i].cy,
                    vertices[i].cx1, vertices[i].cy1,
                    vertices[i].x, vertices[i].y,
                    objspace_flatness_squared, 0);
                x = vertices[i].x, y = vertices[i].y;
                break;
            }
        }
        (*contour_lengths)[n] = num_points - start;
    }

    return points;
error:
    STBTT_free(points, userdata);
    STBTT_free(*contour_lengths, userdata);
    *contour_lengths = 0;
    *num_contours = 0;
    return NULL;
}

STBTT_DEF void stbtt_Rasterize(stbtt__bitmap* result, float flatness_in_pixels, stbtt_vertex* vertices, int num_verts, float scale_x, float scale_y, float shift_x, float shift_y, int x_off, int y_off, int invert, void* userdata)
{
    float scale = scale_x > scale_y ? scale_y : scale_x;
    int winding_count = 0;
    int* winding_lengths = NULL;
    stbtt__point* windings = stbtt_FlattenCurves(vertices, num_verts, flatness_in_pixels / scale, &winding_lengths, &winding_count, userdata);
    if (windings) {
        stbtt__rasterize(result, windings, winding_lengths, winding_count, scale_x, scale_y, shift_x, shift_y, x_off, y_off, invert, userdata);
        STBTT_free(winding_lengths, userdata);
        STBTT_free(windings, userdata);
    }
}

STBTT_DEF void stbtt_FreeBitmap(unsigned char* bitmap, void* userdata)
{
    STBTT_free(bitmap, userdata);
}

STBTT_DEF unsigned char* stbtt_GetGlyphBitmapSubpixel(const stbtt_fontinfo* info, float scale_x, float scale_y, float shift_x, float shift_y, int glyph, int* width, int* height, int* xoff, int* yoff)
{
    int ix0, iy0, ix1, iy1;
    stbtt__bitmap gbm;
    stbtt_vertex* vertices;
    int num_verts = stbtt_GetGlyphShape(info, glyph, &vertices);

    if (scale_x == 0) scale_x = scale_y;
    if (scale_y == 0) {
        if (scale_x == 0) {
            STBTT_free(vertices, info->userdata);
            return NULL;
        }
        scale_y = scale_x;
    }

    stbtt_GetGlyphBitmapBoxSubpixel(info, glyph, scale_x, scale_y, shift_x, shift_y, &ix0, &iy0, &ix1, &iy1);

    // now we get the size
    gbm.w = (ix1 - ix0);
    gbm.h = (iy1 - iy0);
    gbm.pixels = NULL; // in case we error

    if (width) *width = gbm.w;
    if (height) *height = gbm.h;
    if (xoff) *xoff = ix0;
    if (yoff) *yoff = iy0;

    if (gbm.w && gbm.h) {
        gbm.pixels = (unsigned char*)STBTT_malloc(gbm.w * gbm.h, info->userdata);
        if (gbm.pixels) {
            gbm.stride = gbm.w;

            stbtt_Rasterize(&gbm, 0.35f, vertices, num_verts, scale_x, scale_y, shift_x, shift_y, ix0, iy0, 1, info->userdata);
        }
    }
    STBTT_free(vertices, info->userdata);
    return gbm.pixels;
}

STBTT_DEF void stbtt_MakeGlyphBitmapSubpixel(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph)
{
    int ix0, iy0;
    stbtt_vertex* vertices;
    int num_verts = stbtt_GetGlyphShape(info, glyph, &vertices);
    stbtt__bitmap gbm;

    stbtt_GetGlyphBitmapBoxSubpixel(info, glyph, scale_x, scale_y, shift_x, shift_y, &ix0, &iy0, 0, 0);
    gbm.pixels = output;
    gbm.w = out_w;
    gbm.h = out_h;
    gbm.stride = out_stride;

    if (gbm.w && gbm.h)
        stbtt_Rasterize(&gbm, 0.35f, vertices, num_verts, scale_x, scale_y, shift_x, shift_y, ix0, iy0, 1, info->userdata);

    STBTT_free(vertices, info->userdata);
}

STBTT_DEF void stbtt_MakeGlyphBitmap(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph)
{
    stbtt_MakeGlyphBitmapSubpixel(info, output, out_w, out_h, out_stride, scale_x, scale_y, 0.0f, 0.0f, glyph);
}

STBTT_DEF void stbtt_MakeCodepointBitmapSubpixel(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint)
{
    stbtt_MakeGlyphBitmapSubpixel(info, output, out_w, out_h, out_stride, scale_x, scale_y, shift_x, shift_y, stbtt_FindGlyphIndex(info, codepoint));
}

STBTT_DEF void stbtt_MakeCodepointBitmap(const stbtt_fontinfo* info, unsigned char* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int codepoint)
{
    stbtt_MakeCodepointBitmapSubpixel(info, output, out_w, out_h, out_stride, scale_x, scale_y, 0.0f, 0.0f, codepoint);
}

static int equal(float* a, float* b)
{
    return (a[0] == b[0] && a[1] == b[1]);
}


#if defined(__GNUC__) || defined(__clang__)
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wcast-qual"
#endif

STBTT_DEF int stbtt_InitFont(stbtt_fontinfo* info, const unsigned char* data, int offset)
{
    return stbtt_InitFont_internal(info, (unsigned char*)data, offset);
}

STBTT_DEF stbtt_fontinfo helper_getfontinfo(unsigned char* data, int offset)
{
    stbtt_fontinfo fontinfo;
    stbtt_InitFont(&fontinfo, data, offset);
    return fontinfo;
}
#if defined(__GNUC__) || defined(__clang__)
#pragma GCC diagnostic pop
#endif