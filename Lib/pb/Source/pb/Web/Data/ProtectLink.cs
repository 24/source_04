namespace pb.Web
{
    public abstract class ProtectLink
    {
        public abstract string[] UnprotectLink(string protectLink);
        public abstract bool IsLinkProtected(string link);
    }
}
