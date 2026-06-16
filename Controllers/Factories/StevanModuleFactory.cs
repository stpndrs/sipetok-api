using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public interface StevanModuleFactory
    {
        abstract IStevanMethod CreateMethod(string actionType);
    }
}