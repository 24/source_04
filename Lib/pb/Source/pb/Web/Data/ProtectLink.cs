namespace pb.Web.Data
{
    public abstract class ProtectLink
    {
        public abstract string[] UnprotectLink(string protectLink);
        public abstract bool IsLinkProtected(string link);
    }
}
