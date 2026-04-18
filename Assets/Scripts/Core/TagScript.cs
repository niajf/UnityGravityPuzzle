using UnityEngine;

/// <summary>
/// 親オブジェクトのタグをすべての子・孫オブジェクトに再帰的に適用するスクリプト。
/// 複数の子メッシュを持つオブジェクトでタグ判定を統一したい場合に使用する。
/// </summary>
public class TagScript : MonoBehaviour
{
    void Start()
    {
        string parentTag = gameObject.tag;

        // 親自身を除くすべての子オブジェクト（孫含む）を取得し、同じタグを設定する
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
        {
            if (child != transform)
            {
                child.gameObject.tag = parentTag;
            }
        }
    }
}
