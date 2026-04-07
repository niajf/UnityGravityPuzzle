using UnityEngine;

public class TagScript : MonoBehaviour
{
    void Start()
    {
        string parentTag = gameObject.tag;

        // 親自身を除くすべての子オブジェクト（孫含む）を取得
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
        {
            if (child != transform)
            {
                child.gameObject.tag = parentTag;
            }
        }
    }
}
