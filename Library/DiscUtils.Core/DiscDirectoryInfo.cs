//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BitMagic.DiscUtils;

/// <summary>
/// Provides information about a directory on a disc.
/// </summary>
/// <remarks>
/// This class allows navigation of the disc directory/file hierarchy.
/// </remarks>
public class DiscDirectoryInfo : DiscFileSystemInfo
{
    /// <summary>
    /// Initializes a new instance of the DiscDirectoryInfo class.
    /// </summary>
    /// <param name="fileSystem">The file system the directory info relates to.</param>
    /// <param name="path">The path within the file system of the directory.</param>
    public DiscDirectoryInfo(DiscFileSystem fileSystem, string path)
        : base(fileSystem, path) {}

    /// <summary>
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    public override bool Exists
    {
        get { return FileSystem.DirectoryExists(Path); }
    }

    /// <summary>
    /// Gets the full path of the directory.
    /// </summary>
    public override string FullName
    {
        get { return base.FullName + System.IO.Path.DirectorySeparatorChar; }
    }

    /// <summary>
    /// Creates a directory.
    /// </summary>
    public virtual void Create()
    {
        FileSystem.CreateDirectory(Path);
    }

    /// <summary>
    /// Deletes a directory, even if it's not empty.
    /// </summary>
    public override void Delete()
    {
        FileSystem.DeleteDirectory(Path, false);
    }

    /// <summary>
    /// Deletes a directory, with the caller choosing whether to recurse.
    /// </summary>
    /// <param name="recursive"><c>true</c> to delete all child node, <c>false</c> to fail if the directory is not empty.</param>
    public void Delete(bool recursive)
    {
        FileSystem.DeleteDirectory(Path, recursive);
    }

    /// <summary>
    /// Moves a directory and it's contents to a new path.
    /// </summary>
    /// <param name="destinationDirName">The destination directory name.</param>
    public void MoveTo(string destinationDirName)
    {
        FileSystem.MoveDirectory(Path, destinationDirName);
    }

    /// <summary>
    /// Gets all child directories.
    /// </summary>
    /// <returns>An array of child directories.</returns>
    public IEnumerable<DiscDirectoryInfo> GetDirectories()
    {
        return FileSystem.GetDirectories(Path)
            .Select(p => new DiscDirectoryInfo(FileSystem, p));
    }

    /// <summary>
    /// Gets all child directories matching a search pattern.
    /// </summary>
    /// <param name="pattern">The search pattern.</param>
    /// <returns>An array of child directories, or empty if none match.</returns>
    /// <remarks>The search pattern can include the wildcards * (matching 0 or more characters)
    /// and ? (matching 1 character).</remarks>
    public IEnumerable<DiscDirectoryInfo> GetDirectories(string pattern)
    {
        return GetDirectories(pattern, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// Gets all descendant directories matching a search pattern.
    /// </summary>
    /// <param name="pattern">The search pattern.</param>
    /// <param name="searchOption">Whether to search just this directory, or all children.</param>
    /// <returns>An array of descendant directories, or empty if none match.</returns>
    /// <remarks>The search pattern can include the wildcards * (matching 0 or more characters)
    /// and ? (matching 1 character).  The option parameter determines whether only immediate
    /// children, or all children are returned.</remarks>
    public IEnumerable<DiscDirectoryInfo> GetDirectories(string pattern, SearchOption searchOption)
    {
        return FileSystem.GetDirectories(Path, pattern, searchOption)
            .Select(p => new DiscDirectoryInfo(FileSystem, p));
    }

    /// <summary>
    /// Gets all files.
    /// </summary>
    /// <returns>An array of files.</returns>
    public IEnumerable<DiscFileInfo> GetFiles()
    {
        return FileSystem.GetFiles(Path).Select(p => new DiscFileInfo(FileSystem, p));
    }

    /// <summary>
    /// Gets all files matching a search pattern.
    /// </summary>
    /// <param name="pattern">The search pattern.</param>
    /// <returns>An array of files, or empty if none match.</returns>
    /// <remarks>The search pattern can include the wildcards * (matching 0 or more characters)
    /// and ? (matching 1 character).</remarks>
    public IEnumerable<DiscFileInfo> GetFiles(string pattern)
    {
        return GetFiles(pattern, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// Gets all descendant files matching a search pattern.
    /// </summary>
    /// <param name="pattern">The search pattern.</param>
    /// <param name="searchOption">Whether to search just this directory, or all children.</param>
    /// <returns>An array of descendant files, or empty if none match.</returns>
    /// <remarks>The search pattern can include the wildcards * (matching 0 or more characters)
    /// and ? (matching 1 character).  The option parameter determines whether only immediate
    /// children, or all children are returned.</remarks>
    public IEnumerable<DiscFileInfo> GetFiles(string pattern, SearchOption searchOption)
    {
        return FileSystem.GetFiles(Path, pattern, searchOption)
            .Select(p => new DiscFileInfo(FileSystem, p));
    }

    /// <summary>
    /// Gets all files and directories in this directory.
    /// </summary>
    /// <returns>An array of files and directories.</returns>
    public IEnumerable<DiscFileSystemInfo> GetFileSystemInfos()
    {
        return FileSystem.GetFileSystemEntries(Path)
            .Select(p => new DiscFileSystemInfo(FileSystem, p));
    }

    /// <summary>
    /// Gets all files and directories in this directory.
    /// </summary>
    /// <param name="pattern">The search pattern.</param>
    /// <returns>An array of files and directories.</returns>
    /// <remarks>The search pattern can include the wildcards * (matching 0 or more characters)
    /// and ? (matching 1 character).</remarks>
    public IEnumerable<DiscFileSystemInfo> GetFileSystemInfos(string pattern)
    {
        return FileSystem.GetFileSystemEntries(Path, pattern)
            .Select(p => new DiscFileSystemInfo(FileSystem, p));
    }
}