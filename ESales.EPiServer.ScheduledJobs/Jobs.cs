using System;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.Import;
using EPiServer.PlugIn;

namespace Apptus.ESales.EPiServer.ScheduledJobs
{
    ///<summary>
    /// Job for defragmentation of query processor indexes. As updated products are introduced into the eSales Servers,
    /// the disk and memory areas that are in use become fragmented and grows. To avoid that system performance degrades
    /// over time you need to run a defragment command regularly. The frequency with which you should issue defragment
    /// commands is dependent on how often and how big imports you send to the system, as well as your performance demands
    /// and the amount of memory and disk available to the installed eSales Servers. As a rule of thumb, weekly defragment
    /// operations are strongly recommended.
    ///</summary>
    [ScheduledPlugIn(
        DisplayName = "eSales Defrag",
        Description = "Job for defragmentation of indexes. " +
                      "As updated products are introduced into the eSales Servers, " +
                      "the disk and memory areas that are in use become fragmented " +
                      "and grows. To avoid that system performance degrades over " +
                      "time you need to run a defragment command regularly. The " +
                      "frequency with which you should issue defragment commands " +
                      "is dependent on how often and how big imports you send to " +
                      "the system, as well as your performance demands and the " +
                      "amount of memory and disk available to the installed eSales " +
                      "Servers. As a rule of thumb, weekly defragment operations " +
                      "are strongly recommended." )]
    public class DefragJob
    {
        ///<summary>
        /// Executes the job.
        ///</summary>
        public static string Execute()
        {
            Defrag();
            return "Defrag was successful.";
        }

        private static void Defrag()
        {
            var appConfig = new AppConfig();
            if ( appConfig.Saas )
            {
                throw new InvalidOperationException( "Manual defrag is not supported on a saas configuration" );
            }
            OnPremConnector.GetOrCreate( appConfig.ClusterUrl ).Defragment();
        }
    }
}
