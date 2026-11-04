using GameProject.Repositories;
using GameProject.Services;
using GameProject.UI;

namespace GameProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // FLOW: create file repository -> game service -> console UI -> start menu.
            // Main only connects the layers; it has no game rules or screen logic.
            ScoreRepository repository = new ScoreRepository("scores.txt");
            GameService service = new GameService(repository);
            GameUI ui = new GameUI(service);
            ui.Start();
        }
    }
}
