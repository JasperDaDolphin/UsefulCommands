using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System.Text;
using UsefulCommands.Common;

namespace BinCache
{
    public class BinCacheCommand
    {
        private readonly ILogger logger;

        private string root;
        private string binPath;
        private string targetBranch;

        public BinCacheCommand(ILogger logger, string root, string binPath, string targetBranch)
        {
            this.logger = logger;
            this.root = root;
            this.binPath = binPath;
            this.targetBranch = targetBranch;
        }
    
        public void Run()
        {
            try
            {
                using (var repo = new Repository(root))
                {
                    if (binPath == null)
                    {
                        binPath = Path.Combine(root, "Bin");
                    }

                    var binDirectory = new DirectoryInfo(binPath);
                    if (!binDirectory.Exists)
                    {
                        logger.LogWarning("Bin does not exist.");
                        return;
                    }

                    var previousBranch = repo.Head;
                    logger.LogInformation($"Previous Branch: {previousBranch.FriendlyName}");

                    var currentBranch = Checkout(repo, targetBranch);
                    logger.LogInformation($"Current Branch: {currentBranch.FriendlyName}");

                    CacheBranch(previousBranch, currentBranch, binDirectory);
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                logger.LogWarning(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        Branch Checkout(Repository repo, string targetBranch)
        {
            var branch = repo.Branches[targetBranch];
            if (branch == null)
            {
                logger.LogInformation($"Branch {targetBranch} created.");
                branch = repo.CreateBranch(targetBranch);
            }

            var checkoutOptions = new CheckoutOptions
            {
                CheckoutModifiers = CheckoutModifiers.Force,
                CheckoutNotifyFlags = CheckoutNotifyFlags.Updated,
            };

            Commands.Checkout(repo, branch, checkoutOptions);

            return branch;
        }

        DirectoryInfo CacheDirectory()
        {
            var tempDir = new DirectoryInfo(Path.GetTempPath());
            return tempDir.CreateSubdirectory("BinCache");
        }

        void CacheBranch(Branch previousBranch, Branch currentBranch, DirectoryInfo binDirectory)
        {
            var cacheDir = CacheDirectory();

            string binKey = GetHash(binDirectory.FullName);
            string previousBranchKey = GetHash(previousBranch.FriendlyName);
            string currentBranchKey = GetHash(currentBranch.FriendlyName);

            var repositoryDir = cacheDir.CreateSubdirectory(binKey);
            logger.LogInformation($"Bin Cache Key {binKey}");

            var currentBranchDir = repositoryDir.CreateSubdirectory(currentBranchKey);

            logger.LogInformation($"Creating Bin Folder {currentBranchKey} for {currentBranch.FriendlyName} in BinCache Folder...");
            FileManager.CopyDirectory(binDirectory.FullName, currentBranchDir.FullName);

            if (!Directory.Exists(Path.Combine(repositoryDir.FullName, previousBranchKey)))
            {
                var previousBranchDir = repositoryDir.CreateSubdirectory(previousBranchKey);

                logger.LogInformation($"Deleting {previousBranch.FriendlyName} in BinCache Folder...");
                FileManager.DeleteDirectory(previousBranchDir);

                logger.LogInformation($"Archiving Bin Folder for {previousBranch.FriendlyName} in BinCache...");
                Directory.Move(binDirectory.FullName, previousBranchDir.FullName);

                logger.LogInformation($"Restoring Bin Folder for {currentBranch.FriendlyName} from BinCache...");
                Directory.Move(currentBranchDir.FullName, binDirectory.FullName);
            }
        }

        string GetHash(string value)
        {
            byte[] byteArr = new Crc32().ComputeHash(Encoding.ASCII.GetBytes(value));
            return BitConverter.ToString(byteArr);
        }
    }
}
