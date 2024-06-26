/****************************************
	Simple Mesh Combine v1.5		
	Copyright 2015 Unluck Software	
 	www.chemicalbliss.com 																																													
*****************************************/
//Add script to the parent gameObject, then click combine

@script AddComponentMenu("Simple Mesh Combine")
#pragma strict
#pragma downcast
var combinedGameOjects:GameObject[];		//Stores gameObjects that has been merged, mesh renderer disabled
var combined:GameObject;					//Stores the combined mesh gameObject
var meshName:String = "Combined_Meshes";	//Asset name when saving as prefab
var _savedPrefab:boolean;					//Used when checking if this mesh has been saved to prefab (saving the same mesh twice generates error)
var _canGenerateLightmapUV:boolean;
var vCount:int;

function EnableRenderers(e:boolean) {	
	for (var i:int = 0; i < combinedGameOjects.length; i++){
    	combinedGameOjects[i].GetComponent.<Renderer>().enabled = e;
	}  
}
//Returns a meshFilter[] list of all renderer enabled meshfilters(so that it does not merge disabled meshes, useful when there are invisible box colliders)
function FindEnabledMeshes(){
	var renderers:MeshFilter[];
	var count:int;
	renderers = transform.GetComponentsInChildren.<MeshFilter>();
	//count all the enabled meshrenderers in children		
	for (var i:int = 0; i < renderers.length; i++)
	{
		if(renderers[i].GetComponent(MeshRenderer) && renderers[i].GetComponent(MeshRenderer).enabled)
			count++;
	}
	var meshfilters = new MeshFilter[count];//creates a new array with the correct length
	count = 0;
	//adds all enabled meshes to the array
	for (var ii:int = 0; ii < renderers.length; ii++)
	{
		if(renderers[ii].GetComponent(MeshRenderer) && renderers[ii].GetComponent(MeshRenderer).enabled){
			meshfilters[count] = renderers[ii];
			count++;
		}
	}
	return meshfilters;
}

function CombineMeshes() {	
	var combo:GameObject = new GameObject();
	combo.name = "_Combined Mesh [" + name + "]";
	combo.gameObject.AddComponent(MeshFilter);
	combo.gameObject.AddComponent(MeshRenderer);
	var meshFilters:MeshFilter[];
	meshFilters = FindEnabledMeshes();
	var materials: ArrayList = new ArrayList();
	var combineInstanceArrays: ArrayList = new ArrayList();
	combinedGameOjects = new GameObject[meshFilters.length];	
	for (var i = 0; i < meshFilters.length; i++) {
		var meshFilterss: MeshFilter[] = meshFilters[i].GetComponentsInChildren.<MeshFilter>();	
		combinedGameOjects[i] = meshFilters[i].gameObject;	
		for (var meshFilter: MeshFilter in meshFilterss) {
			var meshRenderer: MeshRenderer = meshFilter.GetComponent(MeshRenderer);
			meshFilters[i].transform.gameObject.GetComponent(Renderer).enabled = false;
			for (var o: int = 0; o < meshFilter.sharedMesh.subMeshCount; o++) {
				if(o < meshRenderer.sharedMaterials.Length && o < meshFilter.sharedMesh.subMeshCount){
					var materialArrayIndex: int = Contains(materials, meshRenderer.sharedMaterials[o]);
					if (materialArrayIndex == -1) {
						materials.Add(meshRenderer.sharedMaterials[o]);
						materialArrayIndex = materials.Count - 1;
					}
					combineInstanceArrays.Add(new ArrayList());
					var combineInstance: CombineInstance = new CombineInstance();
					combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
					combineInstance.subMeshIndex = o;
					combineInstance.mesh = meshFilter.sharedMesh;
					(combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
				}
				#if UNITY_EDITOR
				else{
					Debug.LogWarning("Simple Mesh Combine: GameObject [ " +meshRenderer.gameObject.name + " ] is missing a material (Mesh or sub-mesh ignored from combine)");
				}
				#endif
			}

		}
		#if UNITY_EDITOR      
		EditorUtility.DisplayProgressBar("Combining", "", i);	
		#endif
	}
	
	var meshes: Mesh[] = new Mesh[materials.Count];
	var combineInstances: CombineInstance[] = new CombineInstance[materials.Count];
	for (var m: int = 0; m < materials.Count; m++) {
		var combineInstanceArray: CombineInstance[] = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
		meshes[m] = new Mesh();
		meshes[m].CombineMeshes(combineInstanceArray, true, true);
		combineInstances[m] = new CombineInstance();
		combineInstances[m].mesh = meshes[m];
		combineInstances[m].subMeshIndex = 0;
	}		
	combo.GetComponent(MeshFilter).sharedMesh = new Mesh();
	combo.GetComponent(MeshFilter).sharedMesh.CombineMeshes(combineInstances, false, false);
	for (var mesh: Mesh in meshes) {
		mesh.Clear();
		DestroyImmediate(mesh);
	}
	var meshRendererCombine: MeshRenderer = combo.GetComponent(MeshFilter).GetComponent(MeshRenderer);
	if (!meshRendererCombine) meshRendererCombine = gameObject.AddComponent(MeshRenderer);
	var materialsArray: Material[] = materials.ToArray(typeof(Material)) as Material[];
	meshRendererCombine.materials = materialsArray;	
	combined = combo.gameObject;	
    EnableRenderers(false);
    combo.transform.parent = transform;
    combo.GetComponent(MeshFilter).sharedMesh.uv2 = null;
	vCount = combo.GetComponent(MeshFilter).sharedMesh.vertexCount;
	if(vCount > 65536) { 
      Debug.LogWarning("Vertex Count: " +vCount + "- Vertex Count too high, please divide mesh combine into more groups. Max 65536 for each mesh" );
    	_canGenerateLightmapUV = false;
    }else{
    	_canGenerateLightmapUV = true;
    }
    #if UNITY_EDITOR    
    EditorUtility.ClearProgressBar();
    #endif
}

function GenerateLightmapUV (){
	#if UNITY_EDITOR
		EditorUtility.DisplayProgressBar("Creating Lightmap UV's", "This might take some time", .5);
		Unwrapping.GenerateSecondaryUVSet(combined.GetComponent(MeshFilter).sharedMesh);
   		combined.isStatic = true;
   		EditorUtility.ClearProgressBar();
   	#endif	
}

function Contains(l: ArrayList, n: Material) {
	for (var i: int = 0; i < l.Count; i++) {
		if ((l[i] as Material) == n) {
			return i;
		}
	}
	return -1;
}