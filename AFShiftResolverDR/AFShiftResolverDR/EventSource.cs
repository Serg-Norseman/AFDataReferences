using System;
using System.Collections.Generic;
using System.Linq;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;

namespace AFShiftResolverDR
{
    /*****************************************************************************************
     * The EventSource class implements AFEventSource to specify how to get data pipe events 
     * from the system of record. 
     *****************************************************************************************/
    internal class EventSource : AFEventSource
    {
        // Last timestamps for each AF Attribute
        private Dictionary<AFAttribute, AFTime> fLastTimes = new Dictionary<AFAttribute, AFTime>();

        // Start time when the pipe is initiated
        private AFTime fStartTime;

        // Initialize the start time for the event source
        public EventSource()
        {
            fStartTime = new AFTime("*");
        }

        // Get new events for the pipe from the last timestamps till current time of evaluation
        protected override bool GetEvents()
        {
            // Set evaluation time to current time
            AFTime evalTime = AFTime.Now;

            // Get the list of AF Attributes signed up on the data pipe
            IEnumerable<AFAttribute> signupList = base.Signups;

            // Get values for each AF Attribute, one at a time
            foreach (AFAttribute att in signupList) {
                if (!ReferenceEquals(att, null)) {
                    // Add AF Attribute if it hasn't been added to the fLastTimes dictionary yet
                    if (!fLastTimes.ContainsKey(att)) {
                        fLastTimes.Add(att, fStartTime);
                    }

                    // Set time range to get all values between last timestamps to current evaluation time
                    var timeRange = new AFTimeRange(fLastTimes[att], evalTime);

                    /* Note: Use RecordedValues if supported. GetValues call return interpolated values at the start and end time,
                     * which can be problematic in a data pipe implementation. GetValues is used here for this simple example because
                     * the implementation of GetValues in my custom DR does not return interpolated values at the start and end time. */
                    AFValues vals = att.GetValues(timeRange, 0, att.DefaultUOM);

                    // Store old last time for the AF Attribute
                    AFTime lastTime = fLastTimes[att];

                    // Publish each value to the data pipe
                    foreach (AFValue val in vals) {
                        // Record latest timestamp
                        if (val.Timestamp > lastTime) {
                            lastTime = val.Timestamp;
                        }
                        AFDataPipeEvent ev = new AFDataPipeEvent(AFDataPipeAction.Add, val);
                        PublishEvent(att, ev);
                    }

                    // Add a tick to the latest time stamp to prevent the next GetValues call from returning value at the same time
                    fLastTimes[att] = lastTime + TimeSpan.FromTicks(1);
                }
            }

            return false;
        }

        // Dispose resources
        protected override void Dispose(bool disposing)
        {
            fLastTimes = null;
        }
    }
}
