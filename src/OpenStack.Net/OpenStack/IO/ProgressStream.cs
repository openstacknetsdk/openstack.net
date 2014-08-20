namespace OpenStack.IO
{
    using System;
    using System.Threading.Tasks;
    using SeekOrigin = System.IO.SeekOrigin;
    using Stream = System.IO.Stream;

#if !NET40PLUS
    // Uses IProgress<T>
    using Rackspace.Threading;
#endif

#if NET45PLUS
    // Uses CoreTaskExtensions.Select
    using Rackspace.Threading;
    using CancellationToken = System.Threading.CancellationToken;
#endif

    /// <summary>
    /// This class operates as a wrapper around an underlying <see cref="Stream"/> instance
    /// which reports progress to an <see cref="IProgress{T}"/> instance.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ProgressStream : Stream
    {
        private readonly Stream _underlyingStream;
        private readonly IProgress<long> _progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStream"/> class from
        /// the specified <see cref="Stream"/> and <see cref="IProgress{T}"/> handler.
        /// </summary>
        /// <param name="underlyingStream">The stream to wrap.</param>
        /// <param name="progress">The handler to report progress updates to.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="underlyingStream"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="progress"/> is <see langword="null"/>.</para>
        /// </exception>
        public ProgressStream(Stream underlyingStream, IProgress<long> progress)
        {
            if (underlyingStream == null)
                throw new ArgumentNullException("underlyingStream");
            if (progress == null)
                throw new ArgumentNullException("progress");

            _underlyingStream = underlyingStream;
            _progress = progress;
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                return _underlyingStream.CanRead;
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                return _underlyingStream.CanSeek;
            }
        }

        /// <inheritdoc/>
        public override bool CanTimeout
        {
            get
            {
                return _underlyingStream.CanTimeout;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                return _underlyingStream.CanWrite;
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                return _underlyingStream.Length;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                return _underlyingStream.Position;
            }

            set
            {
                _underlyingStream.Position = value;
                _progress.Report(Position);
            }
        }

        /// <inheritdoc/>
        public override int ReadTimeout
        {
            get
            {
                return _underlyingStream.ReadTimeout;
            }

            set
            {
                _underlyingStream.ReadTimeout = value;
            }
        }

        /// <inheritdoc/>
        public override int WriteTimeout
        {
            get
            {
                return _underlyingStream.WriteTimeout;
            }

            set
            {
                _underlyingStream.WriteTimeout = value;
            }
        }

#if NET45PLUS
        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return
                _underlyingStream.CopyToAsync(destination, bufferSize, cancellationToken)
                .Select(task => _progress.Report(Position));
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return
                _underlyingStream.FlushAsync(cancellationToken)
                .Select(task => _progress.Report(Position));
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return
                _underlyingStream.ReadAsync(buffer, offset, count, cancellationToken)
                .Select(
                    task =>
                    {
                        _progress.Report(Position);
                        return task.Result;
                    });
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return
                _underlyingStream.WriteAsync(buffer, offset, count, cancellationToken)
                .Select(task => _progress.Report(Position));
        }
#endif

#if !PORTABLE
        /// <inheritdoc/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _underlyingStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _underlyingStream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            _underlyingStream.Close();
        }

        /// <inheritdoc/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            int result = _underlyingStream.EndRead(asyncResult);
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            _underlyingStream.EndWrite(asyncResult);
            _progress.Report(Position);
        }
#endif

        /// <inheritdoc/>
        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = _underlyingStream.Read(buffer, offset, count);
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            int result = _underlyingStream.ReadByte();
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long result = _underlyingStream.Seek(offset, origin);
            _progress.Report(result);
            return result;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _underlyingStream.Write(buffer, offset, count);
            _progress.Report(Position);
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            _underlyingStream.WriteByte(value);
            _progress.Report(Position);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _underlyingStream.Dispose();

            base.Dispose(disposing);
        }
    }
}
