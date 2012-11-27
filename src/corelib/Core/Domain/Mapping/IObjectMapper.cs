namespace net.openstack.Core.Domain.Mapping
{
    public interface IObjectMapper<TFrom, TTo>
    {
        TTo Map(TFrom from);

        TFrom Map(TTo to);
    }
}