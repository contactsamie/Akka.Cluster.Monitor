using System.Threading;
using Akka.Actor;
using Shared;
using Topshelf;
using ClusterHelper = Shared.ClusterHelper;

namespace Worker
{
    internal class WorkerService : ServiceControl
    {
        public HostControl _hostControl;
        
        public bool Start(HostControl hostControl)
        {
            _hostControl = hostControl;
            InitializeCluster();
            return true;
        }
        
        public bool Stop(HostControl hostControl)
        {
            //do your cleanup here
            Program.ClusterHelper.Tell(new ClusterHelper.RemoveMember());
            Program.ClusterSystem.Shutdown();

            Thread.Sleep(5000); // Give the Remove time to actually remove before totally shutting down system...
            
            return true;
        }

        public void InitializeCluster()
        {
            Program.ClusterSystem = ActorSystem.Create(ActorPaths.ActorSystem);
            Program.ClusterHelper = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterHelper()),
                ActorPaths.ClusterHelperActor.Name);
            Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus(_hostControl)),
                ActorPaths.ClusterStatusActor.Name);
        }
    }
}