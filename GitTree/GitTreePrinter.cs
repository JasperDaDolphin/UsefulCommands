using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitTree;

public class GitTreePrinter
{
    private const string ENDBRANCH = "└───";
    private const string MIDDLEBRANCH = "├───";
    private const string LINE = "│   ";
    private const string SPACE = "    ";

    private readonly ILogger logger;

    public GitTreePrinter(ILogger logger)
    {
        this.logger = logger;
    }

    public void Run(string root, string path)
    {
        try
        {
            using (var repo = new Repository(root))
            {
                PrintDirectoryStructure(repo, path, "", true, true);
            }
        }
        catch (RepositoryNotFoundException ex)
        {
            logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }

    private void PrintDirectoryStructure(Repository repo, string path, string prefix, bool isRoot, bool isLastSibling)
    {
        var dir = new DirectoryInfo(path);
        var subDirs = dir.GetDirectories().Where(a => !IsIgnored(repo, a.FullName)).ToArray();
        var files = dir.GetFiles().Where(a => !IsIgnored(repo, a.FullName)).ToArray();

        if (!isRoot && dir.Name == ".git")
        {
            return;
        }

        logger.LogInformation(prefix + (!isLastSibling ? MIDDLEBRANCH : ENDBRANCH) + dir.Name);
        var newPrefix = prefix + (isLastSibling ? SPACE : LINE);

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var isCurrentLastSibling = i == files.Length - 1 && subDirs.Length == 0;
            logger.LogInformation(newPrefix + (!isCurrentLastSibling ? MIDDLEBRANCH : ENDBRANCH) + file.Name);
        }

        for (var i = 0; i < subDirs.Length; i++)
        {
            var subDir = subDirs[i];
            PrintDirectoryStructure(repo, subDir.FullName, newPrefix, false, i == subDirs.Length - 1);
        }
    }

    private bool IsIgnored(Repository repo, string path)
    {
        var relativePath = path.Substring(repo.Info.WorkingDirectory.Length).Replace("\\", "/");
        return repo.Ignore.IsPathIgnored(relativePath);
    }
}