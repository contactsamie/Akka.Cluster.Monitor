﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Akka.Actor;
using Akka.Cluster;
using Shared;
using Website.Actors;

namespace Worker.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClusterStatusController : ApiController
    {
        public CurrentClusterStatus CurrentClusterState;

        public ClusterStatusController()
        {
            if (Program.ClusterStatus.IsNobody())
            {
                Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus()),
                    ActorPaths.ClusterStatusActor.Name);
            }
        }

        public HttpResponseMessage Get(string id)
        {
            var t1 = Program.ClusterStatus.Ask(new ClusterStatus.GetClusterState(), TimeSpan.FromSeconds(2))
                .ContinueWith(
                        tr =>
                        {
                            CurrentClusterState = (CurrentClusterStatus)tr.Result;
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);

            t1.Wait(TimeSpan.FromSeconds(2));


            var retval = Request.CreateResponse(HttpStatusCode.OK, new
            {
                clusterState = CurrentClusterState
            });
            return retval;
        }
        
        public HttpResponseMessage GetClusterState()
        {
            var t1 = Program.ClusterStatus.Ask(new ClusterStatus.GetClusterState(), TimeSpan.FromSeconds(2))
                .ContinueWith(
                        tr =>
                        {
                            CurrentClusterState = (CurrentClusterStatus)tr.Result;
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);

            t1.Wait(TimeSpan.FromSeconds(2));


            var retval = Request.CreateResponse(HttpStatusCode.OK, new
            {
                clusterState = CurrentClusterState
            });
            return retval;
        }

        [HttpPost]
        public void MemberLeaveCluster(string host, int? port, string protocol, string system)
        {
            SystemActors.ClusterHelper.Tell(new ClusterHelper.MemberLeave(host, port, protocol, system));
        }

        [HttpPost]
        public void DownMember(string host, int? port, string protocol, string system)
        {
            SystemActors.ClusterHelper.Tell(new ClusterHelper.MemberDown(host, port, protocol, system));
        }
    }
}
