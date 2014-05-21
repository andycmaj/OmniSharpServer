using System.IO;
using OmniSharp.Solution;

namespace OmniSharp.Build {
    public class BuildCommandBuilder {
        private readonly ISolution _solution;

        public BuildCommandBuilder(ISolution solution) {
            _solution = solution;
        }

        public string Executable {
            get {
                return "ssh";
            }
        }

        //private string GetRemoteBuildCommand() {
        //    return "ssh";
        //}

        //private string GetLocalBuildCommand() {
        //    return Path.Combine(
        //        System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
        //        "Msbuild.exe").ApplyPathReplacementsForClient();
        //}

        public string Arguments {
            get {
                return "10.211.55.3 -i ~/.ssh/acunningham ''cmd /c msbuild /nologo /v:q /property:GenerateFullPaths=true \""
                       + _solution.FileName + "\"''";
            }
        }
    }
}
