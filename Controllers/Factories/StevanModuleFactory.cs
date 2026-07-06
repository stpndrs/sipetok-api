using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public interface IStevanModuleFactory
    {
        abstract IStevanMethod CreateMethod(string actionType);
    }
}