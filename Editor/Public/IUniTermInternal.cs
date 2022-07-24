using System.Threading.Tasks;

namespace SimpleAndLazy.Editor.Public
{
    public interface IUniTermInternal
    {
        bool IsProcessing();
        Task<string> Input(string input);
    }
}