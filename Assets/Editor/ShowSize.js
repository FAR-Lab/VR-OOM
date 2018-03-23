#pragma strict

class ShowSize extends EditorWindow
{

     @MenuItem ("Window/Show Size")
    static function Init()
    {
        var sizeWindow = new ShowSize();
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.Show();
    }

         function OnGUI()
         {
             var thisObject = Selection.activeObject as GameObject;
             if (!thisObject) { return; }

             var mf : MeshFilter = thisObject.GetComponent(MeshFilter) as MeshFilter;
             if (!mf) { return; }

             var mesh = mf.sharedMesh;
             if (!mesh) { return; }

             var size = mesh.bounds.size;
             var scale = thisObject.transform.localScale;
             GUILayout.Label("Size\nX: " + size.x * scale.x + "   Y: " + size.y * scale.y + "   Z: " + size.z * scale.z);
         }
     }