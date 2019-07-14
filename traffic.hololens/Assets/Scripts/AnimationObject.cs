using UnityEngine;

public class AnimationObject : MonoBehaviour
{
    public void AnimationDoneCommand()
    {
        Debug.Log("AnimationDoneCommand!");
        SendMessageUpwards("AnimationDone", SendMessageOptions.DontRequireReceiver);
    }
}
