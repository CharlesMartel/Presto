
namespace Presto {

    /// <summary>
    /// The entry point for PrestoServer
    /// </summary>
    class MainClass {

        /// <summary>
        /// The entry point for presto server
        /// </summary>
        public static void Main() {
            Application.Initialize();
            while (true) {
                System.Threading.Thread.Sleep(60000);
            }
        }
    }
}
