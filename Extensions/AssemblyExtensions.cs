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

using System;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Extensions {

    [SkipLocalsInit]
    public static class AssemblyExtensions {

        public static string ReadEmbeddedResource(this Assembly Arg_Assembly, string Arg_ResourceName, Encoding Arg_TextEncoding) => Arg_TextEncoding != null ? Arg_TextEncoding.GetString(ReadEmbeddedResource(Arg_Assembly, Arg_ResourceName)) : throw new ArgumentNullException(nameof(Arg_TextEncoding));

        public static byte[] ReadEmbeddedResource(this Assembly Arg_Assembly, string Arg_ResourceName) {
            _ = Arg_Assembly ?? throw new ArgumentNullException(nameof(Arg_Assembly));
            _ = Arg_ResourceName ?? throw new ArgumentNullException(nameof(Arg_ResourceName));
            ManifestResourceInfo? Func_ResourceInfo = Arg_Assembly.GetManifestResourceInfo(Arg_ResourceName);
            if (Func_ResourceInfo != null && Func_ResourceInfo.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly) {
                using System.IO.Stream Func_ResourceStream = Arg_Assembly.GetManifestResourceStream(Arg_ResourceName)!;
                byte[]? Func_ResourceBinary = new byte[Func_ResourceStream.Length];
                int Func_ReadByteCount = Func_ResourceStream.Read(Func_ResourceBinary, 0, Func_ResourceBinary.Length);
                if (Func_ReadByteCount != Func_ResourceBinary.Length && Func_ReadByteCount != 0) {
                    Array.Resize(ref Func_ResourceBinary, Func_ReadByteCount);
                }
                return Func_ResourceBinary;
            } else { throw new System.IO.FileNotFoundException("The embedded resource was not found!"); }
        }

    }

}
