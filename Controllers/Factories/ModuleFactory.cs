using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public interface IModuleFactory
    {
        abstract IMethod CreateMethod(string actionType);
    }
}