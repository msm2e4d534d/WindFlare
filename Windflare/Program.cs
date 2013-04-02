using System;
namespace Windflare {
    static class Program {
        static void Main(string[] args) {
            using (Windflare game = new Windflare()) {
                game.Run();
            }
        }
    }
}