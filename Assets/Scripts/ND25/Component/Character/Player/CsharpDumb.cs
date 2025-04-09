namespace ND25.Component.Character.Player
{
    internal abstract class BaseClass<T>
    {
    }

    internal class BaseContext
    {
    }

    internal class BaseContext1 : BaseContext
    {
    }

    internal class BaseClassImpl : BaseClass<BaseContext>
    {
    }

    internal abstract class CsharpDumbBase
    {
        public abstract BaseClass<T> GetBaseClass<T>();
    }

    class CsharpDumbBaseImpl : CsharpDumbBase
    {
        public override BaseClass<T> GetBaseClass<T>()
        {
            return new BaseClassImpl() as BaseClass<T>;
        }
    }

    class Test
    {
        Test()
        {
            var CsharpDumbBaseImpl = new CsharpDumbBaseImpl();
            var baseClass = CsharpDumbBaseImpl.GetBaseClass<BaseContext>();
        }
    }
}
