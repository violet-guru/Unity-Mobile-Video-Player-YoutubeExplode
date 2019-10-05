using JetBrains.Annotations;
using MainLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace Infinity
{
    public class UnityLog : IBasicLog
    {
        public bool HasExtras()
        {
            return true;
        }

        public void Log(string message, [CanBeNull] IDictionary<string, object> analyzeFields, string finalMessage)
        {
            Debug.Log(finalMessage);
        }
    }

    public class YoutubeHelper
    {
        public async Task<List<MuxedStreamInfo>> GetVideos(string videoId, SuperLog log)
        {
            var client = new YoutubeClient
            {
                log = log
            };

            MediaStreamInfoSet streamInfoSet;
            try
            {
                streamInfoSet = await client.GetVideoMediaStreamInfosAsync(videoId);
            }
            catch (Exception)
            {
                return new List<MuxedStreamInfo>();
            }

            var streamInfos = new List<MuxedStreamInfo>(streamInfoSet.Muxed);
            var mobileVideos = streamInfos.FindAll(s => s.Container == Container.Mp4);
            mobileVideos.Sort((v, t) => t.Resolution.Height.CompareTo(v.Resolution.Height));
            log.Send(true, Hi.AutoTestNumberFinished, false, mobileVideos[0].Url);
            return mobileVideos;
        }
    }
}