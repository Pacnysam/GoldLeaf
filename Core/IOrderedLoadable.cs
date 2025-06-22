namespace GoldLeaf.Core
{
    interface IOrderedLoadable
    {
        void Load();
        void Unload();
        float Priority { get; }
    }
}
