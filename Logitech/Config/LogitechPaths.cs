using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.Config {
    internal static class LogitechPaths {
        public const string ProfileFilename = "{F752A6C0-5D8A-4EB1-A25A-3CC275A060C1}.xml";
        private const string DefaultProfileFilename = "{09D92D75-3C8C-4723-B06C-4090BCB899C0}.xml"; // The global config

        private static string ProfileFolder {
            get {
                var path = Path.Combine(AppPaths.LocalAppdata, "Logitech", "Logitech Gaming Software", "profiles");
                Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string Profile => Path.Combine(ProfileFolder, ProfileFilename);
    }
}
