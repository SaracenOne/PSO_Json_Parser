#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PSOJsonParserScriptableWizard : UnityEditor.ScriptableWizard
{
    public TextAsset textAsset = null;

    [Serializable]
    public class PSOJsonVec3
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
    }

    [Serializable]
    public class PSOSpawnJsonInstance
    {
        public PSOJsonVec3 pos = new PSOJsonVec3();
        public PSOJsonVec3 rot = new PSOJsonVec3();
        public int sectionId = -1;
    }

    [Serializable]
    public class PSOJSONSpawn
    {
        public List<PSOSpawnJsonInstance> start = new List<PSOSpawnJsonInstance>();
        public List<PSOSpawnJsonInstance> end = new List<PSOSpawnJsonInstance>();
    }

    [Serializable]
    public class PSOJSONMeta
    {
        public string stage = "";
        public PSOJSONSpawn spawn = null;
    }

    [Serializable]
    public class PSOJSONSectionInstance
    {
        public int id = -1;
        public PSOJsonVec3 pos = new PSOJsonVec3();
    }

    [Serializable]
    public class PSOJSONObjectInstanceParamsCoords
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
    }

    [Serializable]
    public class PSOJSONObjectInstanceParams
    {   public int color = 0;
        public int number = 0;
        public int id = -1;
        public int index = 0;
        public int symbol = 0;
        public bool isDoor = false;
        public bool locked = false;
        public string direction = "";
        public string set = "";
        public PSOJSONObjectInstanceParamsCoords coords = new PSOJSONObjectInstanceParamsCoords();

    }
    [Serializable]
    public class PSOJSONObjectInstance
    {
        public PSOJsonVec3 pos = new PSOJsonVec3();
        public PSOJsonVec3 rot = new PSOJsonVec3();
        public string type = "";
        public int sectionId = -1;
        public PSOJSONObjectInstanceParams parameters = new PSOJSONObjectInstanceParams();
    }

    [Serializable]
    public class PSOJSONEnemyInstance
    {
        public PSOJsonVec3 pos = new PSOJsonVec3();
        public PSOJsonVec3 rot = new PSOJsonVec3();
        public string type = "";
        public int sectionId = -1;
        public int wave = 0;
    }

    [Serializable]
    public class PSOJSONRoot
    {
        public PSOJSONMeta meta = new PSOJSONMeta();
        public List<PSOJSONSectionInstance> sections = new List<PSOJSONSectionInstance>();
        public List<PSOJSONObjectInstance> objects = new List<PSOJSONObjectInstance>();
        public List<PSOJSONEnemyInstance> enemies = new List<PSOJSONEnemyInstance>();
    }

    private void SetGameObjectParent(GameObject child, GameObject parent)
    {
        if (parent == null)
        {
            child.transform.parent = null;
        }
        else
        {
            child.transform.parent = parent.transform;
        }
    }

    private Transform FindGameObjectChildTransform(GameObject root, string gameObjectName)
    {
        Transform childTransform = null;

        if (root != null)
        {
            childTransform = root.transform.Find(gameObjectName);
        }
        else
        {
            GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject currentGameObject in rootGameObjects)
            {
                if (currentGameObject.name == gameObjectName)
                {
                    childTransform = currentGameObject.transform;
                    break;
                }
            }
        }

        return childTransform;
    }

    private GameObject GetOrCreateNamedGameObject(string gameObjectName, GameObject gameObjectParent)
    {
        Transform childTransform = null;

        childTransform = FindGameObjectChildTransform(gameObjectParent, gameObjectName);

        if (childTransform)
        {
            return childTransform.gameObject;
        }
        else
        {
            GameObject emptyGameObject = new GameObject(gameObjectName);
            SetGameObjectParent(emptyGameObject, gameObjectParent);

            return emptyGameObject;
        }
    }

    private void ParseEntities()
    {
        if(textAsset)
        {
            var root = JsonUtility.FromJson<PSOJSONRoot>(textAsset.ToString());
            GameObject objectInstances = GetOrCreateNamedGameObject("PSO_Objects", null);
            int object_id = 0;
            foreach(PSOJSONObjectInstance instance in root.objects)
            {
                GameObject sectionID = GetOrCreateNamedGameObject("SectionID_" + instance.sectionId, objectInstances);
                GameObject objectInstance = GetOrCreateNamedGameObject("object_" + object_id + "(" + instance.type + ")", sectionID);
                objectInstance.transform.localPosition = new Vector3(
                    -instance.pos.x * 0.1f,
                    instance.pos.y * 0.1f,
                    instance.pos.z * 0.1f);
                objectInstance.transform.localEulerAngles = new Vector3(
                    instance.rot.x * Mathf.Rad2Deg,
                    instance.rot.y * Mathf.Rad2Deg,
                    instance.rot.z * Mathf.Rad2Deg);
                object_id++;
            }
        }
    }

    [UnityEditor.MenuItem("Tools/PSO JSON Parser/Parse Json File")]
    static void CreateWizard()
    {
        UnityEditor.ScriptableWizard.DisplayWizard<PSOJsonParserScriptableWizard>("Parse Json File", "Apply");
    }

    void OnWizardCreate()
    {
        ParseEntities();
    }

    void OnWizardUpdate()
    {

    }
};

#endif