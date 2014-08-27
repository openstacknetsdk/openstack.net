namespace OpenStack.IO
{
    using System;
    using SeekOrigin = System.IO.SeekOrigin;
    using Stream = System.IO.Stream;

#if NET45PLUS
    using System.Threading.Tasks;
    using CancellationToken = System.Threading.CancellationToken;
#endif

    /// <summary>
    /// This class operates as a wrapper around an underlying <see cref="Stream"/> instance.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class DelegatingStream : Stream
    {
        private readonly Stream _underlyingStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingStream"/> class from the specified
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="underlyingStream">The stream to wrap.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="underlyingStream"/> is <see langword="null"/>.
        /// </exception>
        public DelegatingStream(Stream underlyingStream)
        {
            if (underlyingStream == null)
                throw new ArgumentNullException("underlyingStream");

            _underlyingStream = underlyingStream;
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

        /// <summary>
        /// Gets the underlying <see cref="Stream"/> instance which is wrapped by this object.
        /// </summary>
        /// <value>
        /// The underlying <see cref="Stream"/> instance which is wrapped by this object.
        /// </value>
        protected Stream UnderlyingStream
        {
            get
            {
                return _underlyingStream;
            }
        }

#if NET45PLUS
        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _underlyingStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _underlyingStream.FlushAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _underlyingStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _underlyingStream.WriteAsync(buffer, offset, count, cancellationToken);
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
            return _underlyingStream.EndRead(asyncResult);
        }

        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            _underlyingStream.EndWrite(asyncResult);
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
            return _underlyingStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            return _underlyingStream.ReadByte();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _underlyingStream.Seek(offset, origin);
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
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            _underlyingStream.WriteByte(value);
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
