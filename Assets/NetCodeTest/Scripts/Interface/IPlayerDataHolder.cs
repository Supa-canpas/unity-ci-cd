using Unity.Netcode;

namespace NetCodeTest.Scripts.Interface
{
    public interface IPlayerDataHolder
    {
        NetworkVariable<int> HP { get; set; }
    }
}