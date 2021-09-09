/*
    MIT License

    Copyright (C) 2021 Martynas Skirmantas https://github.com/Reaper1121/SharpToolbox

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

namespace Reaper1121.SharpToolbox.Utilities {

    public static class StringUtils {
    
        public static int FindFirstDigit(string Arg_String) { // TODO: Change into using a enumerator (Find first digit, symbol, letter, etc...)
            int Func_DigitIndex = -1;
            if (string.IsNullOrEmpty(Arg_String) == false) {
                int Func_StringLength = Arg_String.Length;
                for (int Loop_Index = 0; Loop_Index < Func_StringLength; ++Loop_Index) {
                    if (char.IsDigit(Arg_String[Loop_Index]) == true) {
                        Func_DigitIndex = Loop_Index;
                        break;
                    }
                }
            }
            return Func_DigitIndex;
        }

    }

}
