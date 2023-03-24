using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitTree;

public class GitTreePrinter
{
    private const string SPACE          = "    ";
    private const string LINE           = "│   ";
    private const string MIDDLEBRANCH   = "├───";
    private const string ENDBRANCH      = "└───";

    private readonly ILogger logger;

    private string root;
    private string path;
    private int maxDepth;
    private bool hideFiles;
    private bool showFileCount;

    public GitTreePrinter(ILogger logger, string root, string path, int maxDepth, bool hideFiles, bool showFileCount)
    {
        this.logger = logger;
        this.root = root;
        this.path = path;
        this.maxDepth = maxDepth;
        this.hideFiles = hideFiles;
        this.showFileCount = showFileCount;
    }

    public void Run()
    {
        try
        {
            using (var repo = new Repository(root))
            {
                var count = PrintDirectoryStructure(repo, path, "", true, true, 0);
                if (showFileCount)
                {
                    logger.LogInformation($"Count: {count}");
                }
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

    private int PrintDirectoryStructure(Repository repo, string path, string prefix, bool isRoot, bool isLastSibling, int currentDepth)
    {
        var dir = new DirectoryInfo(path);
        var subDirs = dir.GetDirectories().Where(a => !IsIgnored(repo, a.FullName)).ToArray();
        var files = dir.GetFiles().Where(a => !IsIgnored(repo, a.FullName)).ToArray();
        var fileCount = files.Length;

        if (!isRoot && dir.Name == ".git")
        {
            return 0;
        }

        logger.LogInformation(prefix + (!isLastSibling ? MIDDLEBRANCH : ENDBRANCH) + dir.Name + (showFileCount ? $" [{fileCount}]" : ""));
        var newPrefix = prefix + (isLastSibling ? SPACE : LINE);

        if (maxDepth != 0 && currentDepth >= maxDepth)
        {
            return 0;
        }

        if (hideFiles)
        {
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var isCurrentLastSibling = i == files.Length - 1 && subDirs.Length == 0;
                logger.LogInformation(newPrefix + (!isCurrentLastSibling ? MIDDLEBRANCH : ENDBRANCH) + file.Name);
            }
        }

        for (var i = 0; i < subDirs.Length; i++)
        {
            var subDir = subDirs[i];
            fileCount += PrintDirectoryStructure(repo, subDir.FullName, newPrefix, false, i == subDirs.Length - 1, currentDepth + 1);
        }

        return fileCount;
    }

    private bool IsIgnored(Repository repo, string path)
    {
        var relativePath = path.Substring(repo.Info.WorkingDirectory.Length).Replace("\\", "/");
        return repo.Ignore.IsPathIgnored(relativePath);
    }
}