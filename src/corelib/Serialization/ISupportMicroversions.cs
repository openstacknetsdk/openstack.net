namespace OpenStack.Serialization
{
    internal interface ISupportMicroversions
    {
        string MicroversionHeader { get; }
        string Microversion { get; }
    }
}