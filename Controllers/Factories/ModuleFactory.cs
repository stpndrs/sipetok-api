using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public interface ModuleFactory
    {
        abstract IMethod CreateMethod(string actionType);
    }
}