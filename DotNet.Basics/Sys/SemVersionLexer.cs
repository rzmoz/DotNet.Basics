using System;
using System.Collections.Generic;

namespace DotNet.Basics.Sys
{
    public class SemVersionLexer
    {
        public const char VersionSeparator = '.';
        public const char PreReleaseSeparator = '-';
        public const char MetadataSeparator = '+';

        public IReadOnlyList<string> Lex(string semVer)
        {
            if (semVer == null) throw new ArgumentNullException(nameof(semVer));
            var tokens = new List<string> { "0", "0", "0", string.Empty, string.Empty };
            var stateIndex = 0;

            foreach (var @char in semVer)
            {
                if (ShouldAddToToken(stateIndex, @char))
                    tokens[stateIndex] += @char;

                stateIndex = GetStateIndex(stateIndex, @char);
            }

            return tokens;
        }

        private int GetStateIndex(int currentStateIndex, char @char)
        {
            if (currentStateIndex < 3 && @char == VersionSeparator)
                return currentStateIndex + 1;
            if (currentStateIndex < 3 && @char == PreReleaseSeparator)
                return 3;
            if (currentStateIndex < 4 && @char == MetadataSeparator)
                return 4;
            return currentStateIndex;
        }

        private bool ShouldAddToToken(int currentStateIndex, char @char)
        {
            if (currentStateIndex <3 && @char == VersionSeparator)
                return false;
            if (currentStateIndex == 2 && @char == PreReleaseSeparator)
                return false;
            if ((currentStateIndex == 2 || currentStateIndex == 3) && @char == MetadataSeparator)
                return false;
            return true;
        }
    }
}
