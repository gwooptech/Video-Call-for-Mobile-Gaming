﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using System.Runtime.InteropServices;
using System.Threading;

// this is an example of using Agora unity sdk
// It demonstrates:
// How to enable video
// How to join/leave channel
//
public class exampleApp : MonoBehaviour
{

    public AudioPlaybackDeviceManager audioPlaybackDeviceManager;
    public AudioRecordingoDeviceManager audioRecordingoDeviceManager;
    public VideoDeviceManager videoDeviceManager;
    public MetaDataObserver metaDataObserver;
    public PacketObserver packetObserver;
    public AudioRawDataManager audioRawDataManager;
    public VideoRawDataManager videoRawDataManager;

    public static void logAPICall(string message)
    {
        DebugLog("APICALL  : " + message);
    }

    // load agora engine
    public void loadEngine()
    {
        // start sdk
        logAPICall("initializeEngine");
        if (mRtcEngine != null)
        {
            logAPICall("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.getEngine(mVendorKey);
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.GAME_COMMAND_MODE);
        mRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
        mRtcEngine.SetParameters("{\"rtc.log_filter\": 65535}");
        audioPlaybackDeviceManager = AudioPlaybackDeviceManager.GetInstance(mRtcEngine);
        audioRecordingoDeviceManager = AudioRecordingoDeviceManager.GetInstance(mRtcEngine);
        videoDeviceManager = VideoDeviceManager.GetInstance(mRtcEngine);
        metaDataObserver = MetaDataObserver.GetInstance(mRtcEngine);
        packetObserver = PacketObserver.GetInstance(mRtcEngine);
        audioRawDataManager = AudioRawDataManager.GetInstance(mRtcEngine);
        videoRawDataManager = VideoRawDataManager.GetInstance(mRtcEngine);
        //mRtcEngine.EnableSoundPositionIndication(true);
        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }


    public void join(string channel)
    {
        logAPICall("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnReJoinChannelSuccess = ReJoinChannelSuccessHandler;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;
        mRtcEngine.OnAudioQuality = OnAudioQuality;
        mRtcEngine.OnStreamInjectedStatus = OnStreamInjectedStatus;
        mRtcEngine.OnStreamUnpublished = OnStreamUnpublished;
        mRtcEngine.OnStreamMessageError = OnStreamMessageError;
        mRtcEngine.OnStreamMessage = OnStreamMessage;
        mRtcEngine.OnConnectionBanned = OnConnectionBanned;
        mRtcEngine.OnRtcStats =  RtcStatsHandler;
        mRtcEngine.OnConnectionLost = ConnectionLostHandler;
        mRtcEngine.OnConnectionInterrupted = ConnectionInterruptedHandler;
        mRtcEngine.OnUserOffline = UserOfflineHandler;
        mRtcEngine.OnVolumeIndication = VolumeIndicationHandler;
        mRtcEngine.OnUserMutedAudio = UserMutedAudioHandler;
        mRtcEngine.OnWarning = onWarning;
        mRtcEngine.OnError = SDKErrorHandler;
        mRtcEngine.OnAudioMixingFinished = AudioMixingFinishedHandler;
        mRtcEngine.OnAudioRouteChanged = AudioRouteChangedHandler;
        mRtcEngine.OnFirstRemoteVideoDecoded = OnFirstRemoteVideoDecodedHandler;
        mRtcEngine.OnVideoSizeChanged = OnVideoSizeChangedHandler;
        mRtcEngine.OnClientRoleChanged = OnClientRoleChangedHandler;
        mRtcEngine.OnUserMuteVideo = OnUserMuteVideoHandler;
        mRtcEngine.OnMicrophoneEnabled = OnMicrophoneEnabledHandler;
        mRtcEngine.OnApiExecuted = OnApiExecutedHandler;
        mRtcEngine.OnLastmileQuality = OnLastmileQualityHandler;
        mRtcEngine.OnFirstLocalAudioFrame = OnFirstLocalAudioFrameHandler;
        mRtcEngine.OnFirstRemoteAudioFrame = OnFirstRemoteAudioFrameHandler;
        mRtcEngine.OnAudioQuality = OnAudioQualityHandler;
        mRtcEngine.OnStreamInjectedStatus = OnStreamInjectedStatusHandler;
        mRtcEngine.OnStreamUnpublished = OnStreamUnpublishedHandler;
        mRtcEngine.OnStreamPublished = OnStreamPublishedHandler;
        mRtcEngine.OnStreamMessage = OnStreamMessageHandler;
        mRtcEngine.OnStreamMessageError = OnStreamMessageErrorHandler;
        mRtcEngine.OnConnectionBanned = OnConnectionBannedHandler;
        mRtcEngine.OnConnectionStateChanged = OnConnectionStateChangedHandler;
        mRtcEngine.OnActiveSpeaker = OnActiveSpeakerHandler;
        mRtcEngine.OnVideoStopped = OnVideoStoppedHandler;
        mRtcEngine.OnFirstLocalVideoFrame = OnFirstLocalVideoFrameHandler;
        mRtcEngine.OnFirstRemoteVideoFrame = OnFirstRemoteVideoFrameHandler;
        mRtcEngine.OnUserEnableVideo = OnUserEnableVideoHandler;
        mRtcEngine.OnUserEnableLocalVideo = OnUserEnableLocalVideoHandler;
        mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChangedHandler;
        mRtcEngine.OnLocalPublishFallbackToAudioOnly = OnLocalPublishFallbackToAudioOnlyHandler;
        mRtcEngine.OnRemoteSubscribeFallbackToAudioOnly = OnRemoteSubscribeFallbackToAudioOnlyHandler;
        mRtcEngine.OnNetworkQuality = OnNetworkQualityHandler;
        mRtcEngine.OnLocalVideoStats = OnLocalVideoStatsHandler;
        mRtcEngine.OnRemoteVideoStats = OnRemoteVideoStatsHandler;
        mRtcEngine.OnRemoteAudioStats = OnRemoteAudioStatsHandler;
        mRtcEngine.OnAudioDeviceStateChanged = OnAudioDeviceStateChangedHandler;
        videoRawDataManager.SetOnCaptureVideoFrameCallback(OnCaptureVideoFrameHandler);
        videoRawDataManager.SetOnRenderVideoFrameCallback(OnRenderVideoFrameHandler);
        audioRawDataManager.SetOnMixedAudioFrameCallback(OnMixedAudioFrameHandler);
        audioRawDataManager.SetOnPlaybackAudioFrameBeforeMixingCallback(OnPlaybackAudioFrameBeforeMixingHandler);
        audioRawDataManager.SetOnPlaybackAudioFrameCallback(OnPlaybackAudioFrameHandler);
        audioRawDataManager.SetOnRecordAudioFrameCallback(OnRecordAudioFrameHandler);
        audioRawDataManager.SetOnPullAudioFrameCallback(OnPullAudioFrameHandler);
        metaDataObserver.SetOnOnMediaMetaDataReceivedCallback(OnMediaMetaDataReceivedHandler);
        packetObserver.SetOnReceiveAudioPacketCallback(OnReceiveAudioPacketHandler);
        packetObserver.SetOnReceiveVideoPacketCallback(OnReceiveVideoPacketHandler);
        // enable video
        mRtcEngine.EnableVideo();
        // mRtcEngine.EnableVideoObserver();

        // allow camera output callback
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG);

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);

        logAPICall("initializeEngine done");
    }

    void OnReceiveVideoPacketHandler(Packet packet)
    {
        string str = System.Text.Encoding.Default.GetString (packet.buffer);
        logCallback("OnReceiveVideoPacketHandler  buffer = " + str + "  ,size = " + packet.size);
    }

    void OnReceiveAudioPacketHandler(Packet packet)
    {
        string str = System.Text.Encoding.Default.GetString (packet.buffer);
        logCallback("OnReceiveAudioPacketHandler  buffer = " + str + "  ,size = " + packet.size);
    }

    void OnMediaMetaDataReceivedHandler(Metadata metadata)
    {
        
        string str = System.Text.Encoding.Default.GetString (metadata.buffer);
        logCallback("OnMediaMetaDataReceivedHandler  buffer = " + str + "  ,uid = " + metadata.uid + "  ,length = " + metadata.size);
    }
    void OnPullAudioFrameHandler(AudioFrame audioFrame)
    {
        logCallback("OnPullAudioFrameHandler");
    }

    void OnRenderVideoFrameHandler(uint uid, VideoFrame videoFrame)
    {
        logCallback("OnRenderVideoFrameHandler uid = " + uid + "  ,videoFrame = " + videoFrame.width + "  height = " + videoFrame.height);
    }

    void OnCaptureVideoFrameHandler(VideoFrame videoFrame)
    {
        logCallback("OnCaptureVideoFrameHandler  videoFrame  width =  " + videoFrame.width + " ,height = " + videoFrame.height);
    }

    void OnCameraFocusAreaChangedHandler(int x, int y, int width, int height)
    {
        logCallback("OnCameraFocusAreaChangedHandler ");
    }

    void OnCameraReadyHandler()
    {
        logCallback("OnCameraReadyHandler");
    }

    void OnAudioDeviceStateChangedHandler(string deviceId, int deviceType, int deviceState)
    {
        logCallback("OnAudioDeviceStateChangedHandler deviceId = " + deviceId + " ,deviceType = " + deviceType + " ,deviceState = " + deviceState);
    }
   

    void OnRemoteAudioStatsHandler(RemoteAudioStats remoteAudioStats)
    {
        logCallback("OnRemoteAudioStatsHandler");
    }


    void OnRemoteVideoStatsHandler(RemoteVideoStats remoteVideoStats)
    {
        logCallback("OnRemoteVideoStatsHandler");
    }

    void OnLocalVideoStatsHandler(LocalVideoStats localVideoStats)
    {
        logCallback("OnLocalVideoStatsHandler");
    }

    void OnNetworkQualityHandler(uint uid, int txQuality, int rxQuality)
    {
        logCallback("OnNetworkQualityHandler uid = " + uid + " ,txQuality = " + txQuality + " ,rxQuality = " + rxQuality);
    }

    void OnRemoteSubscribeFallbackToAudioOnlyHandler(uint uid, bool isFallbackOrRecover)
    {
        logCallback("OnRemoteSubscribeFallbackToAudioOnlyHandler uid = " + uid + " ,isFallbackOrRecover " + isFallbackOrRecover);
    }

    void OnLocalPublishFallbackToAudioOnlyHandler(bool isFallbackOrRecover)
    {
        logCallback("OnLocalPublishFallbackToAudioOnlyHandler isFallbackOrRecover = " + isFallbackOrRecover);
    }


    void OnRemoteVideoStateChangedHandler(uint uid, int state)
    {
        logCallback("OnRemoteVideoStateChangedHandler  uid = " + uid + " ,state = " + state);
    }


    void OnUserEnableLocalVideoHandler(uint uid, bool enabled)
    {
        logCallback("OnUserEnableLocalVideoHandler  uid =  " + uid + " ,enabled = " + enabled);
    }


    void OnUserEnableVideoHandler(uint uid, bool enabled)
    {
        logCallback("OnUserEnableVideoHandler  uid = " + uid + " , enabled = " + enabled);
    }


    void OnFirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed)
    {
        logCallback("OnFirstRemoteVideoFrameHandler  uid = " + uid + "  ,width = " + width + "  ,height = " + height);
    }


     void OnFirstLocalVideoFrameHandler(int width, int height, int elapsed)
     {
         logCallback("OnFirstLocalVideoFrameHandler  width = " + width + "  ,height = " + height);
     }


    void OnVideoStoppedHandler()
    {
        logCallback("OnVideoStoppedHandler");
    }

    void OnActiveSpeakerHandler(uint uid)
    {
        logCallback("OnActiveSpeakerHandler  uid = " + uid);
    }

    void OnConnectionStateChangedHandler(int state, int reason)
    {
        logCallback("OnConnectionStateChangedHandler  state = " + state + " ,reason = " + reason);
    }


    void OnConnectionBannedHandler()
    {
        logCallback("OnConnectionBannedHandler");
    }

    void OnStreamMessageHandler(uint userId, int streamId, string data, int length)
    {
        logCallback("OnStreamMessageHandler  userId = " + userId + " ,streamId = " + streamId + " ,data = " + data + " ,length = " + length);
    }

    void OnStreamMessageErrorHandler(uint userId, int streamId, int code, int missed, int cached)
    {
        logCallback("OnStreamMessageErrorHandler  userId = " + userId + " ,streamId = " + streamId + " ,code = " + code + " ,missed = " + missed + " ,cached = " + cached);
    }

    void OnStreamPublishedHandler(string url, int error)
    {
        logCallback("OnStreamPublishedHandler  url = " + url + " ,error = " + error);
    }


    void OnStreamUnpublishedHandler(string url)
    {
        logCallback("OnStreamUnpublishedHandler  url = " + url);
    }

    void OnStreamInjectedStatusHandler(string url, uint userId, int status)
    {
        logCallback("OnStreamInjectedStatusHandler  url = " + url + "  ,userId = " + userId + " ,status = " + status);
    }


    void OnAudioQualityHandler(uint userId, int quality, ushort delay, ushort lost)
    {
        logCallback("OnAudioQualityHandler  userId = " + userId + " ,quality = " + quality + " ,delay = " + delay + " ,lost = " + lost);
    }

    void OnFirstRemoteAudioFrameHandler(uint userId, int elapsed)
    {
        logCallback("OnFirstRemoteAudioFrameHandler  userId = " + userId + "  ,elapsed = " + elapsed);
    }


    void OnFirstLocalAudioFrameHandler(int elapsed)
    {
        logCallback("OnFirstLocalAudioFrameHandler  elapsed = " + elapsed);
    }

     void OnLastmileQualityHandler(int quality)
     {
         logCallback("OnLastmileQualityHandler  quality = " + quality);
     }

    void OnApiExecutedHandler(int err, string api, string result)
    {
        logCallback("OnApiExecutedHandler err = " + err + "  ,api = " + api + "  ,result = " + result);
    }

    void OnMicrophoneEnabledHandler(bool isEnabled)
    {
        logCallback("OnMicrophoneEnabledHandler isEnabled = " + isEnabled);
    }

    void OnUserMuteVideoHandler(uint uid, bool muted)
    {
        logCallback("OnUserMuteVideoHandler  uid = " + uid + "  ,muted = " + muted);
    }

    void OnClientRoleChangedHandler(int oldRole, int newRole)
    {
        logCallback("OnClientRoleChangedHandler oldRole = " + oldRole + " ,newRole = " + newRole);
    }

    void OnVideoSizeChangedHandler(uint uid, int width, int height, int elapsed)
    {
        logCallback("OnVideoSizeChangedHandler  uid = " + uid + " ,width = " + width + " ,height = " + height);
    }

    void AudioRouteChangedHandler(AUDIO_ROUTE route)
    {
        logCallback("OnAudioRouteChangedHandler  route = " + (int)route);
    }

    void OnFirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed)
    {
        logCallback("OnFirstRemoteVideoDecodedHandler  uid = " + uid + "  ,width = " + width + "  ,height = " + height);
    }

     void AudioMixingFinishedHandler()
     {
         logCallback("OnAudioMixingFinishedHandler ");
     }

    void UserMutedAudioHandler(uint uid, bool muted)
    {
        logCallback("OnUserMutedAudioHandler uid = " + uid + "  ,muted = " + muted);
    }

    void SDKErrorHandler(int error, string msg)
    {
        logCallback("OnSDKErrorHandler error = " + error + " ,msg = " + msg);
    }

    void UserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
    {
        logCallback("OnUserOfflineHandler uid = " + uid + "  ,reason = " + (int)reason);
    }

    void VolumeIndicationHandler(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
    {
        if (speakerNumber > 0)
        {
            for (int i = 0; i<speakerNumber; i++)
            {
                logCallback("OnVolumeIndication  uid = " + speakers[i].uid + "  ,volume = " + speakers[i].volume);
            }
        }
    }

    void ConnectionInterruptedHandler()
    {
        logCallback("OnConnectionInterruptedHandler");
    }

    public static void logCallback(string message)
    {
        DebugLog("callback  " + message);
    }

    public static void DebugLog(string message)
    {
        Debug.Log("AgoraTest : " + message);
    }

    void ConnectionLostHandler()
    {
        logCallback("OnConnectionLostHandler");
    }

    public void ReJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        logCallback("OnReJoinChannelSuccessHandler  channelName = " + channelName + " ,uid = " + uid + " ,elapsed = " + elapsed);
    }

    public void OnRecordAudioFrameHandler(AudioFrame audioFrame)
	{
		Debug.Log("AgoraTest  OnRecordAudioFrameHandler  ThreadId = " + Thread.CurrentThread.ManagedThreadId.ToString() + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
	}

    public void OnPlaybackAudioFrameHandler(AudioFrame audioFrame)
	{
		Debug.Log("AgoraTest  OnPlaybackAudioFrameHandler ThreadId = " + Thread.CurrentThread.ManagedThreadId.ToString() + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
	}

    public void OnMixedAudioFrameHandler(AudioFrame audioFrame)
	{
		Debug.Log("AgoraTest  OnMixedAudioFrameHandler  ThreadId = " + Thread.CurrentThread.ManagedThreadId.ToString() +  " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
	}


    public void OnPlaybackAudioFrameBeforeMixingHandler(uint uid, AudioFrame audioFrame)
	{
		Debug.Log("AgoraTest  OnPlaybackAudioFrameBeforeMixingHandler  ThreadId = " + Thread.CurrentThread.ManagedThreadId.ToString() + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
	}

    public void RtcStatsHandler(RtcStats stats)
    {
        logCallback("OnRtcStats " + " ,duration = " + stats.duration + " ,txBytes = " + stats.txBytes + " ,rxBytes = " + stats.rxBytes 
        + " ,txKBitRate = " + stats.txKBitRate + " ,rxKBitRate = " + stats.rxKBitRate + " ,txAudioKBitRate = " + stats.txAudioKBitRate + " ,rxAudioKBitRate = " + stats.rxAudioKBitRate
        + " ,txVideoKBitRate = " + stats.txVideoKBitRate + " ,rxVideoKBitRate = " + stats.rxVideoKBitRate +
        " ,lastmileQuality = " + stats.lastmileDelay + " ,users = " + stats.userCount + " ,cpuAppUsage = " + stats.cpuAppUsage + " ,cpuTotalUsage = " + stats.cpuTotalUsage
        + " ,txPacketLossRate = " + " ,rxPacketLossRate = " + stats.rxPacketLossRate);
    }

    public void OnStreamInjectedStatus(string url, uint userId, int status)
    {
        logCallback("OnStreamInjectedStatus  url = " + url + "  userId = " + userId);
    }

    public void OnStreamUnpublished(string url) 
    {
        logCallback("OnStreamUnpublished  url = " + url);
    }

    public void OnConnectionBanned()
    {
        logCallback("OnConnectionBanned  ");
    }

    public void OnStreamPublished (string url, int error)
    {
        logCallback("OnStreamPublished url = " + url + "  error = " + error);
    }

    public void OnStreamMessageError(uint userId, int streamId, int code, int missed, int cached)
    {
        logCallback("OnStreamMessageError  userId = " + userId + "  streamId = " + streamId);
    }

    public void OnStreamMessage (uint userId, int streamId, string data, int length)
    {
        logCallback("OnStreamMessage  userId = " + userId + "  streamId = " + streamId + "  data = " + data);
    }

    public void OnAudioQuality(uint userId, int quality, ushort delay, ushort lost){
       // logCallback("OnAudioQuality  userId = " + userId + "  quality = " + quality + "  delay = " + delay);
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        mRtcEngine.LeaveChannel();
       // audioRawDataManager.UnRegisteAudioRawDataObserver();
    }

    // unload agora engine
    public void unloadEngine()
    {
        logAPICall("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onScene1Loaded()
    {
        GameObject go = GameObject.Find("Cylinder");
        if (ReferenceEquals(go, null))
        {
            logAPICall("BBBB: failed to find Cylinder");
            return;
        }
        // VideoSurface o = go.GetComponent<VideoSurface> ();
        // o.mAdjustTransfrom += onTransformDelegate;
    }

    // instance of agora engine
    public IRtcEngine mRtcEngine;
    public static string mVendorKey = #YOUR_APPID;

    // implement engine callbacks

    public uint mRemotePeer = 0; // insignificant. only record one peer

    //private AudioRawDataManager audioRawDataManager = null;


    private void onWarning(int warningCode, string message)
    {
        logCallback("onWarning  warningCode = " + warningCode + "  ,message = " + message);
    }

    private void OnAudioVolumeIndication(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
    {
    }
        
    public Texture2D mTexture;
    public Rect mRect;
	void cutScreen()
	{
		//yield return new WaitForEndOfFrame();
        //videoBytes = Marshal.AllocHGlobal(Screen.width * Screen.height * 4);
        mTexture.ReadPixels(mRect, 0, 0);
		mTexture.Apply();  
		byte[] bytes = mTexture.GetRawTextureData();
		int size = Marshal.SizeOf(bytes[0]) * bytes.Length;	
        IRtcEngine rtc = IRtcEngine.QueryEngine();

        if (rtc != null)
        {
            ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
            externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_BGRA;
            externalVideoFrame.buffer = bytes;
            externalVideoFrame.stride = (int)mRect.width;
            externalVideoFrame.height = (int)mRect.height;
            externalVideoFrame.cropLeft = 10;
            externalVideoFrame.cropTop = 10;
            externalVideoFrame.cropRight = 10;
            externalVideoFrame.cropBottom = 10;
            externalVideoFrame.rotation = 10;
            externalVideoFrame.timestamp =100;
            int a = rtc.PushVideoFrame(externalVideoFrame);
            Debug.Log(" pushVideoFrame =       "  + a);
        }
        }  
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        // mRect = new Rect(0, 0, Screen.width, Screen.height);
        // mTexture = new Texture2D((int)mRect.width, (int)mRect.height,TextureFormat.RGBA32 ,false);  
        // cutScreen();
        ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
        mRtcEngine.PushVideoFrame(externalVideoFrame);
        logCallback("JoinChannelSuccessHandler: uid = " + uid);
        AudioFrame audioFrame = new AudioFrame();
        mRtcEngine.PushAudioFrame(audioFrame);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        logCallback("onUserJoined: uid = " + uid);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }


        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if (!ReferenceEquals(go, null))
        {
            go.name = uid.ToString();

            VideoSurface o = go.AddComponent<VideoSurface>();
            o.SetForUser(uid);
            o.mAdjustTransfrom += onTransformDelegate;
            o.SetEnable(true);
            o.transform.Rotate(-90.0f, 0.0f, 0.0f);
            float r = Random.Range(-5.0f, 5.0f);
            o.transform.position = new Vector3(0f, r, 0f);
            o.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }
        mRemotePeer = uid;
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        logCallback("onUserOffline: uid = " + uid);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }

    // delegate: adjust transfrom for game object 'objName' connected with user 'uid'
    // you could save information for 'uid' (e.g. which GameObject is attached)
    private void onTransformDelegate(uint uid, string objName, ref Transform transform)
    {
        if (uid == 0)
        {
            transform.position = new Vector3(0f, 2f, 0f);
            transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
            transform.Rotate(0f, 1f, 0f);
        }
        else
        {
            transform.Rotate(0.0f, 1.0f, 0.0f);
        }
    }
}
