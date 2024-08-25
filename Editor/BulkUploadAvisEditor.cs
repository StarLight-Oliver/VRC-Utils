#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor;
using VRC.SDK3A.Editor;
using VRC.SDKBase.Editor.Api;
using VRC.SDK3.Avatars.Components;
using System;
using System.Collections.Generic;

using VRC.Core;

using System.Linq;

namespace StarUtils
{

	public class BulkUploadAvis : MonoBehaviour
	{

		public async static bool UploadAvatar( GameObject avatarObject) {
			if (!VRCSdkControlPanel.TryGetBuilder<IVRCSdkAvatarBuilderApi>(out var builder)) return false;
			var sdkBuilder = (IVRCSdkBuilderApi)builder;

			var pipelineManager = avatarObject.GetComponent<PipelineManager>();

			if (pipelineManager.blueprintId == "")
			{
				Debug.Log("Skipping avatar object: " + avatarObject.name + " - blueprintId is not set");
				return false;
			}

			sdkBuilder.OnSdkBuildStart += OnSdkBuildStart;
			sdkBuilder.OnSdkBuildError += OnSdkBuildError;
			sdkBuilder.OnSdkBuildSuccess += OnSdkBuildSuccess;
			
			sdkBuilder.OnSdkUploadStart += OnSdkUploadStart;
			sdkBuilder.OnSdkUploadProgress += OnSdkUploadProgress;
			sdkBuilder.OnSdkUploadError += OnSdkUploadError;
			sdkBuilder.OnSdkUploadSuccess += OnSdkUploadSuccess;
			sdkBuilder.OnSdkUploadFinish += OnSdkUploadFinish;

			avatarObject.gameObject.SetActive(true);

			var avatarData = await VRCApi.GetAvatar(pipelineManager.blueprintId);

			try {
				await builder.BuildAndUpload(avatarObject.gameObject, avatarData);
				Debug.Log("Finished uploading avatar object: " + avatarObject.name);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to upload avatar object: " + avatarObject.name + " - " + ex.Message);
				return false;
			}
			finally
			{
				avatarObject.gameObject.SetActive(false);
			}

			return true;
		}

		[MenuItem("Tools/Star Utils/Upload All Models")]
		public static async void UploadAllAvatars()
		{
			if (!VRCSdkControlPanel.TryGetBuilder<IVRCSdkAvatarBuilderApi>(out var builder)) return;

			var avatarObjects = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<VRCAvatarDescriptor>(true)).ToArray();
			Debug.Log("Found " + avatarObjects.Length + " avatar objects.");

			var sdkBuilder = (IVRCSdkBuilderApi)builder;

			sdkBuilder.OnSdkBuildStart += OnSdkBuildStart;
			sdkBuilder.OnSdkBuildError += OnSdkBuildError;
			sdkBuilder.OnSdkBuildSuccess += OnSdkBuildSuccess;
			
			sdkBuilder.OnSdkUploadStart += OnSdkUploadStart;
			sdkBuilder.OnSdkUploadProgress += OnSdkUploadProgress;
			sdkBuilder.OnSdkUploadError += OnSdkUploadError;
			sdkBuilder.OnSdkUploadSuccess += OnSdkUploadSuccess;
			sdkBuilder.OnSdkUploadFinish += OnSdkUploadFinish;

			foreach (var avatarObject in avatarObjects)
			{
				Debug.Log("Uploading avatar object: " + avatarObject.name);

				var pipelineManager = avatarObject.GetComponent<PipelineManager>();

				if (pipelineManager.blueprintId == "")
				{
					Debug.Log("Skipping avatar object: " + avatarObject.name + " - blueprintId is not set");
					continue;
				}

				var avatarData = await VRCApi.GetAvatar(pipelineManager.blueprintId);
				avatarObject.gameObject.SetActive(true);

				try
				{
					await builder.BuildAndUpload(avatarObject.gameObject, avatarData);
					Debug.Log("Finished uploading avatar object: " + avatarObject.name);
				}
				catch (Exception ex)
				{
					Debug.LogError("Failed to upload avatar object: " + avatarObject.name + " - " + ex.Message);
					break;
				}
				finally
				{
					avatarObject.gameObject.SetActive(false);
				}
			}

			sdkBuilder.OnSdkBuildStart -= OnSdkBuildStart;
			sdkBuilder.OnSdkBuildError -= OnSdkBuildError;
			sdkBuilder.OnSdkBuildSuccess -= OnSdkBuildSuccess;
			
			sdkBuilder.OnSdkUploadStart -= OnSdkUploadStart;
			sdkBuilder.OnSdkUploadProgress -= OnSdkUploadProgress;
			sdkBuilder.OnSdkUploadError -= OnSdkUploadError;
			sdkBuilder.OnSdkUploadSuccess -= OnSdkUploadSuccess;
			sdkBuilder.OnSdkUploadFinish -= OnSdkUploadFinish;

			Debug.Log("Upload complete.");
		}

		public static void OnSdkBuildStart(object sender, object target)
		{
			Debug.Log("OnSdkBuildStart");
		}
		
		public static void OnSdkBuildError(object sender,string error)
		{
			Debug.LogError("OnSdkBuildProgress: " + error);
		}

		public static void OnSdkBuildSuccess(object sender, object target)
		{
			Debug.Log("OnSdkBuildSuccess");
		}

		public static void OnSdkUploadStart(object sender, object target)
		{
			Debug.Log("OnSdkUploadStart");
		}

		public static void OnSdkUploadProgress(object sender, (string status, float percentage) progress)
		{
			Debug.Log("OnSdkUploadProgress: " + progress.percentage);
		}

		public static void OnSdkUploadError(object sender, string error)
		{
			Debug.LogError("OnSdkUploadError: " + error);
		}

		public static void OnSdkUploadSuccess(object sender, object target)
		{
			Debug.Log("OnSdkUploadSuccess");
		}

		public static void OnSdkUploadFinish(object sender, object target)
		{
			Debug.Log("OnSdkUploadFinish");
		}
	}
}
#endif