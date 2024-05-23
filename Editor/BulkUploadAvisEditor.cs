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

public class BulkUploadAvis : MonoBehaviour
{

	[MenuItem("Tools/Star Utils/Upload All Models")]
    public static async void UploadAllAvatars()
    {
        if (!VRCSdkControlPanel.TryGetBuilder<IVRCSdkAvatarBuilderApi>(out var builder)) return;

        var avatarObjects = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<VRCAvatarDescriptor>(true)).ToArray();
        Debug.Log("Found " + avatarObjects.Length + " avatar objects.");

        var sdkBuilder = (IVRCSdkBuilderApi)builder;


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

        Debug.Log("Upload complete.");
    }

}
#endif