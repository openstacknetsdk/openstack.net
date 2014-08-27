namespace OpenStack.IO
{
    using System;
    using SeekOrigin = System.IO.SeekOrigin;
    using Stream = System.IO.Stream;

#if !NET40PLUS
    // Uses IProgress<T>
    using Rackspace.Threading;
#endif

#if NET45PLUS
    // Uses CoreTaskExtensions.Select
    using Rackspace.Threading;
    using System.Threading.Tasks;
    using CancellationToken = System.Threading.CancellationToken;
#endif

    /// <summary>
    /// This class operates as a wrapper around an underlying <see cref="Stream"/> instance
    /// which reports progress to an <see cref="IProgress{T}"/> instance.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ProgressStream : DelegatingStream
    {
        private readonly IProgress<long> _progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStream"/> class from
        /// the specified <see cref="Stream"/> and <see cref="IProgress{T}"/> handler.
        /// </summary>
        /// <param name="underlyingStream">The stream to wrap.</param>
        /// <param name="progress">The handler to report progress updates to.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="underlyingStream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="progress"/> is <see langword="null"/>.</para>
        /// </exception>
        public ProgressStream(Stream underlyingStream, IProgress<long> progress)
            : base(underlyingStream)
        {
            if (progress == null)
                throw new ArgumentNullException("progress");

            _progress = progress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStream"/> class from
        /// the specified <see cref="Stream"/> and <see cref="IProgress{T}"/> handler.
        /// </summary>
        /// <param name="underlyingStream">The stream to wrap.</param>
        /// <param name="progress">The handler to report progress updates to.</param>
        /// <param name="ownsStream">
        /// <para><see langword="true"/> if this object owns the wrapped stream, and should dispose
        /// of it when this instance is closed or disposed.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if this object should not dispose of the wrapped stream.</para>
        /// <para>The default value is <see langword="true"/>.</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="underlyingStream"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="progress"/> is <see langword="null"/>.</para>
        /// </exception>
        public ProgressStream(Stream underlyingStream, IProgress<long> progress, bool ownsStream)
            : base(underlyingStream, ownsStream)
        {
            if (progress == null)
                throw new ArgumentNullException("progress");

            _progress = progress;
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                base.Position = value;
                _progress.Report(Position);
            }
        }

#if NET45PLUS
        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return
                base.CopyToAsync(destination, bufferSize, cancellationToken)
                .Select(task => _progress.Report(Position));
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return
                base.FlushAsync(cancellationToken)
                .Select(task => _progress.Report(Position));
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return
                base.ReadAsync(buffer, offset, count, cancellationToken)
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
                base.WriteAsync(buffer, offset, count, cancellationToken)
                .Select(task => _progress.Report(Position));
        }
#endif

#if !PORTABLE
        /// <inheritdoc/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            int result = base.EndRead(asyncResult);
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            base.EndWrite(asyncResult);
            _progress.Report(Position);
        }
#endif

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = base.Read(buffer, offset, count);
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            int result = base.ReadByte();
            _progress.Report(Position);
            return result;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long result = base.Seek(offset, origin);
            _progress.Report(result);
            return result;
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            _progress.Report(Position);
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            _progress.Report(Position);
        }
    }
}
