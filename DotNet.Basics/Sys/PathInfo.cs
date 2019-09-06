﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public abstract class PathInfo
    {
        private static readonly char[] _separatorDetectors = { PathSeparator.Backslash, PathSeparator.Slash };

        protected PathInfo(string path, params string[] segments)
            : this(path, PathType.Unknown, segments)
        { }

        protected PathInfo(string path, PathType pathType, params string[] segments)
            : this(path, pathType, PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, PathType pathType, char pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            Separator = pathSeparator != PathSeparator.Unknown ? pathSeparator : DetectPathSeparator(path, segments);

            //Clean segments
            Segments = Tokenize(path, segments);
            PathType = pathType == PathType.Unknown ? DetectPathType(path, segments) : pathType;

            //Set rawpath
            RawPath = Flatten(null, PathType, Segments.ToArray());
            RawPath = OverridePathSeparator(RawPath, Separator);

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Separator));
        }

        public string RawPath { get; }
        public string Name { get; }
        
        [JsonIgnore]
        [IgnoreDataMember]
        public string NameWoExtension => Path.GetFileNameWithoutExtension(Name);
        [JsonIgnore]
        [IgnoreDataMember]
        public string Extension => Path.GetExtension(Name);

        public PathType PathType { get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        [JsonIgnore]
        [IgnoreDataMember]
        public DirPath Directory => PathType == PathType.File ? Parent : (DirPath)this;

        public char Separator { get; }
        public IReadOnlyCollection<string> Segments;

        public static IReadOnlyCollection<string> Tokenize(string path, params string[] segments)
        {
            var tokens = new List<string>();
            tokens.AddRange(Tokenize(path));
            foreach (var segment in segments)
                tokens.AddRange(Tokenize(segment));
            return tokens;
        }

        private static IEnumerable<string> Tokenize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Enumerable.Empty<string>();

            path = OverridePathSeparator(path, PathSeparator.Backslash);

            return path.Split(new[] { PathSeparator.Backslash }, StringSplitOptions.RemoveEmptyEntries)
                .Where(seg => string.IsNullOrWhiteSpace(seg) == false);
        }

        private static string OverridePathSeparator(string path, char separator)
        {
            //conform separators
            path = path.Replace(PathSeparator.Slash, separator);
            path = path.Replace(PathSeparator.Backslash, separator);
            path = path.Replace(PathSeparator.Unknown, separator);

            return path;
        }

        public static string Flatten(string path, PathType pathType, params string[] segments)
        {
            var tokenized = Tokenize(path, segments);
            var separator = DetectPathSeparator(path, segments);
            var flattened = string.Join(separator.ToString(), tokenized);
            if (pathType == PathType.Dir)
                flattened = flattened.EnsureSuffix(separator);
            return flattened;
        }

        public static PathType DetectPathType(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return PathType.Unknown;

            if (lookingAt.EndsWith(PathSeparator.Backslash.ToString()) || lookingAt.EndsWith(PathSeparator.Slash.ToString()))
                return PathType.Dir;
            return PathType.File;
        }

        private static char DetectPathSeparator(string path, IEnumerable<string> segments)
        {
            //auto detect supported separators
            foreach (var segment in path.ToEnumerable(segments))
            {
                if (segment == null)
                    continue;
                //first separator wins!
                var separatorIndex = segment.IndexOfAny(_separatorDetectors);
                if (separatorIndex >= 0)
                    return segment[separatorIndex];
            }

            return PathSeparator.Backslash;//default
        }

        public override string ToString()
        {
            return RawPath;
        }

        protected bool Equals(PathInfo other)
        {
            return RawPath.Equals(other.RawPath, StringComparison.OrdinalIgnoreCase);//ignore case on comparison since we're mainly Windows. To bad (L)inux systems
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathInfo)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
