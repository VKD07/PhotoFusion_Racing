using Fusion;

namespace Code
{
    public interface IAttachmentProvider
    {
        public NetworkObject CurrentAttachedObj { get; set; }
    }
}