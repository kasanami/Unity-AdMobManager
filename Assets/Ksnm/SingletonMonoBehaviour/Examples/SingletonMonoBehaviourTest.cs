/*
The zlib License

Copyright (c) 2014-2015 Takahiro Kasanami

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

3. This notice may not be removed or altered from any source distribution.
*/
using UnityEngine;

namespace Ksnm.Examples
{
    /// <summary>
    /// SingletonMonoBehaviourをテストする。
    /// </summary>
    public class SingletonMonoBehaviourTest : MonoBehaviour
    {
        public Transform hogePrefab;
        void OnGUI()
        {
            GUI.skin.label.fontSize =
            GUI.skin.button.fontSize = 20;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.Label("シングルトンなMonoBehaviourをテスト");
            GUILayout.Label("HogeSingletonMonoBehaviour.IsInstantiated=" + HogeSingletonMonoBehaviour.IsInstantiated);
            GUILayout.Label("HogeSingletonMonoBehaviour.IsActiveAndEnabled=" + HogeSingletonMonoBehaviour.IsActiveAndEnabled);
            if (HogeSingletonMonoBehaviour.IsInstantiated)
            {
                GUILayout.Label("HogeSingletonMonoBehaviour.Instance.gameObject.activeSelf=" + HogeSingletonMonoBehaviour.Instance.gameObject.activeSelf);
            }
            else
            {
                GUILayout.Label("HogeSingletonMonoBehaviour.Instance.gameObject.activeSelf=?(インスタンス作成後に表示されます)");
            }
            if (GUILayout.Button("HogeSingletonMonoBehaviourオブジェクトの実体を追加"))
            {
                Instantiate(hogePrefab);
            }
            if (GUILayout.Button("HogeSingletonMonoBehaviourオブジェクトの実体を削除"))
            {
                if (HogeSingletonMonoBehaviour.IsInstantiated)
                {
                    Destroy(HogeSingletonMonoBehaviour.Instance.gameObject);
                }
            }
            if (GUILayout.Button("HogeSingletonMonoBehaviour.Instanceを参照\n（実体が自動で作られる）"))
            {
                Debug.Log("HogeSingletonMonoBehaviour.Instance.name=" + HogeSingletonMonoBehaviour.Instance.name);
            }
            GUILayout.Label("コンソールに結果が表示されます。");
            GUILayout.EndArea();
        }
    }
}