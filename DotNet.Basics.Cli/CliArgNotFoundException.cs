using System;

namespace DotNet.Basics.Cli
{
    public class CliArgNotFoundException(string? message) : Exception(message);
}
