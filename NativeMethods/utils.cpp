#include <iostream>
#ifdef  _WINDOWS
#define dllExport extern "C" __declspec(dllexport)
#else
#define dllExport extern "C"
#endif //  _WINDOWS
dllExport void MakeFontBitmapOutline(unsigned char* bitmap, bool* bitmap_, int width, int height)
{
    for (int j = 0; j < height; j++)
    {
        for (int i = 0; i < width; i++)
        {
            if (bitmap[j * width + i] != 0)
            {
                if (i == 0 || i == width - 1) bitmap_[j * width + i] = true;
                else if (bitmap[j * width + i + 1] < 100) bitmap_[j * width + i + 1] = true;
                else if (bitmap[j * width + i - 1] < 100) bitmap_[j * width + i - 1] = true;
                if (j == 0 || j == height - 1) bitmap_[j * width + i] = true;
                else if (bitmap[j * width + i + width] < 100) bitmap_[j * width + i + width] = true;
                else if (bitmap[j * width + i - width] < 100) bitmap_[j * width + i - width] = true;
            }
        }
    }
}
