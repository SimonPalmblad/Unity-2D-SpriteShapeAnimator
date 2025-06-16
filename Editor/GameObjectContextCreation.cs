using U2DSpriteShapeAnimator.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.ComponentModel.Design;
using UnityEditor.Presets;
using System.Linq;

namespace U2DSpriteShapeAnimator
{
    public class GameObjectContextCreation: MonoBehaviour
    {
        private static string animatorName = "Sprite Shape Animator";
        private static string skinName = "Sprite Shape Skin";

        private static string openPackagePath = "Packages/com.unity.2d.spriteshape/Editor/ObjectMenuCreation/DefaultAssets/Sprite Shapes/Open Sprite Shape.prefab";
        private static string closedPackagePath = "Packages/com.unity.2d.spriteshape/Editor/ObjectMenuCreation/DefaultAssets/Sprite Shapes/Closed Sprite Shape.prefab";

        [MenuItem("GameObject/2D Object/Sprite Shape/Open Sprite Shape Animator", false)]
        static void CreateOpenSpriteShapeAnimator()
        {
            CreateSpriteShapeAnimator(openPackagePath, isOpenEnded: true);
        }

        [MenuItem("GameObject/2D Object/Sprite Shape/Closed Sprite Shape Animator", false)]
        static void CreateClosedSpriteShapeAnimator()
        {
            CreateSpriteShapeAnimator(closedPackagePath, isOpenEnded: false);
        }

        static void CreateSpriteShapeAnimator(string packagePath, bool isOpenEnded)
        {
            // Get 2D Sprite Shape default profiles
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(packagePath) as GameObject;
            var preset = new PresetType(asset.GetComponent<SpriteShapeController>());
            var defaults = Preset.GetDefaultPresetsForType(preset).Count(x => x.enabled);

            GameObject parent = ObjectFactory.CreateGameObject(animatorName, typeof(SpriteShapeAnimator), typeof(Animator));
            var child = ObjectFactory.CreateGameObject(skinName, typeof(SpriteShapeController), typeof(AnimationShape));
            
            // Set option in params instead
            child.GetComponent<SpriteShapeController>().spline.isOpenEnded = isOpenEnded;

            // If there are no default presets for the sprite shape controller, set from package default
            if (defaults == 0)
                EditorUtility.CopySerialized(asset.GetComponent<SpriteShapeController>(), child.GetComponent<SpriteShapeController>());

            SpriteShapeAnimator controller = parent.GetComponent<SpriteShapeAnimator>();
            controller.AssignSpriteShapeController(child.GetComponent<SpriteShapeController>(), child.GetComponent<AnimationShape>());

            SetParent(child, parent);
            controller.CheckJointSynchronization();
        }

        private static void SetParent(GameObject child, GameObject parent)
        {
            child.transform.parent = parent.transform;
        }

    }

}
