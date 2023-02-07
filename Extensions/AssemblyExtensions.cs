using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reaper1121.SharpToolbox.Extensions;

[SkipLocalsInit]
public static class AssemblyExtensions {

    public static string ReadEmbeddedTextResource(this Assembly Arg_Assembly, string Arg_ResourceName) => ReadEmbeddedTextResource(Arg_Assembly, Arg_ResourceName, null, true);

    public static string ReadEmbeddedTextResource(this Assembly Arg_Assembly, string Arg_ResourceName, Encoding? Arg_TextEncoding) => ReadEmbeddedTextResource(Arg_Assembly, Arg_ResourceName, Arg_TextEncoding, Arg_TextEncoding == null);

    public static string ReadEmbeddedTextResource(this Assembly Arg_Assembly, string Arg_ResourceName, Encoding? Arg_TextEncoding, bool Arg_DetectEncodingFromBOM) {
        using StreamReader Func_ResourceStream = new StreamReader(OpenEmbeddedResource(Arg_Assembly, Arg_ResourceName), encoding: Arg_TextEncoding, detectEncodingFromByteOrderMarks: Arg_DetectEncodingFromBOM, leaveOpen: false);
        return Func_ResourceStream.ReadToEnd();
    }

    public static byte[] ReadEmbeddedResource(this Assembly Arg_Assembly, string Arg_ResourceName) {
        using Stream Func_ResourceStream = OpenEmbeddedResource(Arg_Assembly, Arg_ResourceName);
        byte[]? Func_ResourceBinary = new byte[Func_ResourceStream.Length];
        int Func_ReadByteCount = Func_ResourceStream.Read(Func_ResourceBinary, 0, Func_ResourceBinary.Length);
        if (Func_ReadByteCount != Func_ResourceBinary.Length) {
            Array.Resize(ref Func_ResourceBinary, Func_ReadByteCount);
        }
        return Func_ResourceBinary;
    }

    public static Stream OpenEmbeddedResource(this Assembly Arg_Assembly, string Arg_ResourceName) {
        ArgumentNullException.ThrowIfNull(Arg_Assembly);
        ManifestResourceInfo? Func_ResourceInfo = Arg_Assembly.GetManifestResourceInfo(Arg_ResourceName ?? throw new ArgumentNullException(nameof(Arg_ResourceName)));
        if (Func_ResourceInfo != null && Func_ResourceInfo.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly) {
            return Arg_Assembly.GetManifestResourceStream(Arg_ResourceName) ?? throw new FileNotFoundException("The embedded resource was not found!");
        } else {
            throw new FileNotFoundException("The embedded resource was not found!");
        }
    }

}
