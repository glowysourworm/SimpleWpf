using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace SimpleWpf.Native.IO
{
    /// <summary>
    /// Manages a MemoryMappedStream to take small pieces of a file; and read lines without much other 
    /// buffering.
    /// </summary>
    public class FastFileStream : IDisposable
    {
        private string _fileName;
        private long _fileLength;
        private long _defaultBufferSize;

        private MemoryMappedFile _memoryMappedFile;

        public FastFileStream(string fileName, long defaultBufferReadLength = 4096)
        {
            // Need length of file to prepare stream / buffering
            _fileLength = new FileInfo(fileName).Length;
            _fileName = fileName;
            _defaultBufferSize = defaultBufferReadLength;

            _memoryMappedFile = MemoryMappedFile.CreateFromFile(_fileName, FileMode.Open);
        }

        /// <summary>
        /// Attempts to read the stream until reaching the next end-line character '\n' (N-times). The position is updated; and the
        /// actual bytes read indicate whether the end of the stream is near. You may alter the behavior of the stream by
        /// adaptively changing the buffer read length - which is set to your default. Taking chunks of the file is an easy
        /// way to minimize managed memory loading for large files. Why can't we do this using UnmanagedMemoryStream?
        /// </summary>
        /// <param name="position">Current position of the file (to read the line)</param>
        /// <param name="bytesUntilEndOfStream">Bytes read before reaching the '\n' character (including this character)</param>
        /// <param name="endOfStream">Indicates that the end of stream was reached (based on Win32 file info)</param>
        /// <param name="bufferReadLength">The size of the internal buffer allocated for the MemoryMappedFile (for this call)</param>
        /// <returns>Single line of the text file</returns>
        public List<string> ReadLines(ref long position, out long bytesUntilEndOfStream, out bool endOfStream)
        {

            if (_memoryMappedFile.SafeMemoryMappedFileHandle.IsInvalid ||
                _memoryMappedFile.SafeMemoryMappedFileHandle.IsClosed)
                throw new IOException("FastFileStream internal file handle is not valid. The file was not opened for reading successfully; or it has already been closed.");

            if (position >= _fileLength ||
                position < 0)
                throw new ArgumentException("Position of the buffer read must be within the bounds of the file");

            // We are going to assume that the MemoryMappedFile can handle the native calls without too much trouble. What would
            // be a problem is if there are TPL related issues creating these tiny stream buffers. I've already had trouble finding
            // out how to access file data natively / simply. There are land-mines everywhere! And, trying to wrap unmanaged code
            // has gotten more difficult for new .NET framework releases!

            // See if we hit the last chunk of the stream
            endOfStream = false;

            // Have to calculate for the user's code
            bytesUntilEndOfStream = _fileLength - position;

            // Actual Buffer Read
            long bufferReadLength = _defaultBufferSize;

            // Set our buffer read length based on proximity to the end of stream
            if (position + bufferReadLength > _fileLength)
            {
                bufferReadLength = _fileLength - position - 1;
            }

            using (var stream = _memoryMappedFile.CreateViewAccessor(position, bufferReadLength))
            {
                var result = new List<string>();
                char currentChar;
                string currentString = string.Empty;
                long startPosition = position;
                long lastEndLinePosition = -1;

                do
                {
                    // Hopefully, this comes pretty close to native performance!
                    currentChar = (char)stream.ReadByte(position - startPosition);

                    if (currentChar == -1)
                        endOfStream = true;

                    else if (currentChar == '\n')
                    {
                        lastEndLinePosition = position;
                        position++;

                        if (currentString != string.Empty)
                        {
                            // Next String
                            result.Add(currentString);
                            currentString = string.Empty;
                        }
                    }


                    else
                    {
                        position++;
                        currentString += currentChar;
                    }

                    // NOT SEEING EOF CHARACTER
                    if (position == _fileLength - 1)
                        endOfStream = true;

                } while (!endOfStream &&                                        // EOF Character
                         !(position - startPosition >= bufferReadLength) &&     // End of the current memory mapped stream view
                         !(position >= _fileLength));                           // End of total file

                // Set stream position:  
                //
                // 1) if EOF:  {file length} - 1
                // 2) else:    Min({last end of line} + 1, {file length} - 1)
                // 3) No EOL; but EOF:  
                //      -> Result will be missing the final line. This could be
                //         mitigated in several ways. Best to flesh out this class
                //         if needed to locate string tokens, if performance wins
                //         out over FileStream.
                //
                if (endOfStream)
                    position = _fileLength - 1;

                else if (lastEndLinePosition != -1)
                    position = Math.Min(lastEndLinePosition, _fileLength - 1);

                return result;
            }
        }

        public void Dispose()
        {
            if (_memoryMappedFile != null)
            {
                _memoryMappedFile.Dispose();
                _memoryMappedFile = null;
            }
        }
    }
}
