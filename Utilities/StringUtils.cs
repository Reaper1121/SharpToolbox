namespace Reaper1121.SharpToolbox.Utilities;

public static class StringUtils {

    /// <summary>
    /// Calculates a fast 32-bit non-cryptographic string hash using FNV-1a algorithm
    /// </summary>
    /// <param name="Arg_String"></param>
    /// <returns></returns>
    public static uint ComputeHash(string Arg_String) { // LICENSE NOTE: This bit of source code was taken directly from https://github.com/dotnet/roslyn, available under MIT
        uint Func_HashCode = 0;
        if (Arg_String != null) {
            Func_HashCode = 2166136261U;
            int Index = 0;
            goto Label_Start;
        Label_Again:
            Func_HashCode = unchecked((Arg_String[Index] ^ Func_HashCode) * 16777619);
            ++Index;
        Label_Start:
            if (Index < Arg_String.Length) {
                goto Label_Again;
            }
        }
        return Func_HashCode;
    }

}