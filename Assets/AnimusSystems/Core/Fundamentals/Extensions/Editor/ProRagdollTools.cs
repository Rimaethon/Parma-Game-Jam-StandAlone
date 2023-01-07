using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

class ProRagdollBuilder : ScriptableWizard
{
    public GameObject HumanoidModel = null;
    public float Thickness = 0.08f;
    public float totalMass = 100;
    public float jointStrength = Mathf.Infinity;
    public JointType jointType = JointType.Character;

    Dictionary<string, BodyPartConfiguration> Parts = new Dictionary<string, BodyPartConfiguration>() {
        {"Hips", new BodyPartConfiguration(0.15625f, new Vector3(2,0,3.1f), typeof(BoxCollider)) },
        {"Spine", new BodyPartConfiguration(0.15625f, new Vector3(4,1,3),typeof(BoxCollider),Vector3.right,Vector3.back,-70,20,30,30) },
        {"UpperChest", new BodyPartConfiguration(0.015625f,new Vector3(4f,1.5f,3.5f),typeof(BoxCollider),Vector3.right,Vector3.back,-20,20,20,20) },
        {"Head", new BodyPartConfiguration(0.0625f, Vector3.one*1.6f,typeof(SphereCollider),Vector3.right,Vector3.back,-45,45,15,35) },

        {"LeftUpperLeg", new BodyPartConfiguration( 0.09375f, Vector3.one,typeof(CapsuleCollider), Vector3.right, Vector3.forward, -45, 120, 60, 30 ) },
        {"RightUpperLeg", new BodyPartConfiguration( 0.09375f, Vector3.one,typeof(CapsuleCollider), Vector3.right, Vector3.forward, -45, 120, 60, 30 ) },

        {"LeftLowerLeg", new BodyPartConfiguration( 0.07375f, Vector3.one*0.9f, typeof(CapsuleCollider), Vector3.right, Vector3.forward,  -130, 0, 0, 0 )},
        {"RightLowerLeg", new BodyPartConfiguration( 0.07375f, Vector3.one*0.9f, typeof(CapsuleCollider), Vector3.right, Vector3.forward,  -130, 0, 0, 0 )},

        {"LeftFoot", new BodyPartConfiguration(0.02f,new Vector3(1.5f,0,1),typeof(BoxCollider),Vector3.right,Vector3.forward,-30,30,20,20, 10) },
        {"RightFoot", new BodyPartConfiguration(0.02f,new Vector3(1.5f,0,1),typeof(BoxCollider),Vector3.right,Vector3.forward,-30,30,20,20, 10) },

        {"LeftUpperArm",new BodyPartConfiguration(0.0625f, Vector3.one*0.8f, typeof(CapsuleCollider), Vector3.up, Vector3.back, -120, 45, 90, 10) },
        {"RightUpperArm",new BodyPartConfiguration(0.0625f, Vector3.one*0.8f, typeof(CapsuleCollider), Vector3.down, Vector3.back, -120, 45, 90, 10) },

        {"LeftLowerArm", new BodyPartConfiguration(0.0525f,Vector3.one*0.7f,typeof(CapsuleCollider), Vector3.up, Vector3.back, -120, 0, 0, 0) },
        {"RightLowerArm", new BodyPartConfiguration(0.0525f,Vector3.one*0.7f,typeof(CapsuleCollider), Vector3.down, Vector3.back, -120, 0, 0, 0) },

        {"LeftHand", new BodyPartConfiguration(0.01f,new Vector3(1f,0.7f,0.7f), typeof(BoxCollider), Vector3.up, Vector3.back, -10, 20, 80, 0, 10) },
        {"RightHand", new BodyPartConfiguration(0.01f,new Vector3(1f,0.7f,0.7f), typeof(BoxCollider), Vector3.down, Vector3.back, -10, 20, 80, 0, 10) }
    };


    [MenuItem("Instarion tools/Pro ragdoll")]
    static void CreateWizard()
    {
        DisplayWizard("Build Pro Ragdoll", typeof(ProRagdollBuilder));
    }

    private void OnWizardCreate()
    {
        //0. Get model human description
        HumanDescription humanDescription = new HumanDescription();
        var isModelOK = GetHumanDescription(HumanoidModel, ref humanDescription);
        if (!isModelOK)
        {
            Debug.LogError("Ragdoll builder error: Please Select Imported Model in Project View not prefab or other things.");
            return;
        }
        if (humanDescription.human.Length==0)
        {
            Debug.LogError("Ragdoll builder error: Check if your model Animation type is Humanoid.");
            return;
        }

        //1. Get position for future ragdoll
        var editorCam = SceneView.lastActiveSceneView.camera.transform;
        RaycastHit hit;
        var dist = 4;
        Vector3 Position = Physics.Raycast(editorCam.position, editorCam.forward, out hit, dist) ? hit.point : editorCam.position + editorCam.forward * dist;

        //2. Instantiate ragdoll
        var Ragdoll = (GameObject)PrefabUtility.InstantiatePrefab(HumanoidModel);
        Ragdoll.transform.position = Position;
        var RagdollParts = Ragdoll.GetComponentsInChildren<Transform>();
        //3. Find bone transforms from humanoid description
        foreach (var bone in humanDescription.human)
        {
            if (Parts.ContainsKey(bone.humanName))
                Parts[bone.humanName].Bone = RagdollParts.FirstOrDefault(t => t.name == bone.boneName).gameObject;
        }
        //4. Attach physics for every bone
        foreach (var pair in Parts)
        {
            if (pair.Value.Bone) CreateBodyPart(pair.Value);
        }

        if (jointType == JointType.Configurable) JointConverter.CharToConf(RagdollParts[0]);
        Debug.Log("Ragdoll created");
    }
    void CreateBodyPart(BodyPartConfiguration conf)
    {
        //1. Setup rigidbody
        conf.Bone.AddComponent<Rigidbody>().mass = conf.massProportion * totalMass;

        //2. Setup collider
        if (conf.ColliderType == typeof(BoxCollider))
        {
            var box = conf.Bone.AddComponent<BoxCollider>();
            var bounds = new Bounds();

            for (int i = 0; i < conf.Bone.transform.childCount; i++)
            {
                bounds.Encapsulate(conf.Bone.transform.GetChild(i).localPosition);
            }
            box.center = bounds.center;
            box.size = bounds.size+conf.thicknessProportion*Thickness;

        }
        else if (conf.ColliderType == typeof(SphereCollider))
        {
            var sphere = conf.Bone.AddComponent<SphereCollider>();
            sphere.radius = Thickness * conf.thicknessProportion.x;
            sphere.center += Vector3.up * sphere.radius*0.5f;
        }
        else if (conf.ColliderType == typeof(CapsuleCollider))
        {
            var capsule = conf.Bone.AddComponent<CapsuleCollider>();
            capsule.radius = Thickness * conf.thicknessProportion.x;
            capsule.height = conf.Bone.transform.GetChild(0).localPosition.magnitude;
            capsule.center += conf.Bone.transform.GetChild(0).localPosition * 0.5f;
            var localPosition = conf.Bone.transform.GetChild(0).localPosition;
            var AbsX = Mathf.Abs(localPosition.x);
            var AbsY = Mathf.Abs(localPosition.y);
            var AbsZ = Mathf.Abs(localPosition.z);
            if (AbsX > AbsY && AbsX > AbsZ) capsule.direction = 0;
            else if (AbsY > AbsX && AbsY > AbsZ) capsule.direction = 1;
            else capsule.direction = 2;
        }

        if (conf.PrimaryAxis == Vector3.zero) return;
        //3. Setup joint
        var joint = conf.Bone.AddComponent<CharacterJoint>();
        joint.connectedBody = conf.Bone.transform.parent.GetComponentInParent<Rigidbody>();
        joint.axis = conf.PrimaryAxis;
        joint.swingAxis = conf.SecondaryAxis;

        var limit = joint.lowTwistLimit;
        limit.limit = conf.LoLimit;
        joint.lowTwistLimit = limit;

        limit = joint.highTwistLimit;
        limit.limit = conf.HiLimit;
        joint.highTwistLimit = limit;

        limit = joint.swing1Limit;
        limit.limit = conf.Swing1Limit;
        joint.swing1Limit = limit;

        limit = joint.swing2Limit;
        limit.limit = conf.Swing2Limit;
        joint.swing2Limit = limit;

        joint.breakForce = jointStrength;
        joint.breakTorque = jointStrength;

        joint.connectedMassScale = conf.connectedMassScale;
    }
    public static bool GetHumanDescription(GameObject target, ref HumanDescription des)
    {
        //1. Check target
        if (target == null) return false;
        //2. Try get importer
        AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(target));
        if (importer == null) return false;
        //3. Try cast importer to ModelImporter
        ModelImporter modelImporter = importer as ModelImporter;
        if (modelImporter == null) return false;
        //4. Return human description
        des = modelImporter.humanDescription;
        return true;
    }
    private void OnWizardUpdate()
    {
        helpString =
            "Select humanoid model in Project View to create pro ragdoll.\n" +
             "Make sure that:\n" +
             "1. Your character in T-pose\n" +
             "2. Y-axis of every bone looks up\n" +
             "3. Z-axis of every bone looks forward";
    }
    public enum JointType { Character, Configurable }
    class BodyPartConfiguration
    {
        public float massProportion = 1;
        public Vector3 thicknessProportion = Vector3.one;
        public Type ColliderType;

        public Vector3 PrimaryAxis;
        public Vector3 SecondaryAxis;
        public float LoLimit;
        public float HiLimit;
        public float Swing1Limit;
        public float Swing2Limit;
        public float connectedMassScale = 1;

        public GameObject Bone;

        public BodyPartConfiguration(float massProportion, Vector3 thicknessProportion, Type ColliderType)
        {
            this.massProportion = massProportion;
            this.ColliderType = ColliderType;
            this.thicknessProportion = thicknessProportion;
        }
        public BodyPartConfiguration(float massProportion, Vector3 thicknessProportion, Type ColliderType, Vector3 PrimaryAxis, Vector3 SecondaryAxis, float LoLimit, float HiLimit, float Swing1Limit, float Swing2Limit)
        {
            this.massProportion = massProportion;
            this.thicknessProportion = thicknessProportion;
            this.ColliderType = ColliderType;
            this.PrimaryAxis = PrimaryAxis;
            this.SecondaryAxis = SecondaryAxis;
            this.LoLimit = LoLimit;
            this.HiLimit = HiLimit;
            this.Swing1Limit = Swing1Limit;
            this.Swing2Limit = Swing2Limit;
        }
        public BodyPartConfiguration(float massProportion, Vector3 thicknessProportion, Type ColliderType, Vector3 PrimaryAxis, Vector3 SecondaryAxis, float LoLimit, float HiLimit, float Swing1Limit, float Swing2Limit,float connectedMassScale)
        {
            this.massProportion = massProportion;
            this.thicknessProportion = thicknessProportion;
            this.ColliderType = ColliderType;
            this.PrimaryAxis = PrimaryAxis;
            this.SecondaryAxis = SecondaryAxis;
            this.LoLimit = LoLimit;
            this.HiLimit = HiLimit;
            this.Swing1Limit = Swing1Limit;
            this.Swing2Limit = Swing2Limit;
            this.connectedMassScale = connectedMassScale;
        }
    }
}

public class JointConverter
{
    public static void CharToConf(Transform root)
    {
        var CharacterJoints = root.GetComponentsInChildren<CharacterJoint>();
        foreach (var CharacterJoint in CharacterJoints)
        {
            var ConfigurableJoint = CharacterJoint.gameObject.AddComponent<ConfigurableJoint>();
            ConfigurableJoint.connectedBody = CharacterJoint.connectedBody;
            ConfigurableJoint.anchor = CharacterJoint.anchor;
            ConfigurableJoint.axis = CharacterJoint.axis;
            ConfigurableJoint.autoConfigureConnectedAnchor = CharacterJoint.autoConfigureConnectedAnchor;
            ConfigurableJoint.connectedAnchor = CharacterJoint.connectedAnchor;
            ConfigurableJoint.secondaryAxis = CharacterJoint.swingAxis;

            ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
            ConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
            ConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;

            ConfigurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
            ConfigurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
            ConfigurableJoint.angularZMotion = ConfigurableJointMotion.Limited;

            ConfigurableJoint.angularXLimitSpring = CharacterJoint.twistLimitSpring;
            ConfigurableJoint.angularYZLimitSpring = CharacterJoint.swingLimitSpring;

            ConfigurableJoint.highAngularXLimit = CharacterJoint.highTwistLimit;
            ConfigurableJoint.lowAngularXLimit = CharacterJoint.lowTwistLimit;
            ConfigurableJoint.angularYLimit = CharacterJoint.swing1Limit;
            ConfigurableJoint.angularZLimit = CharacterJoint.swing2Limit;

            ConfigurableJoint.rotationDriveMode = RotationDriveMode.Slerp;

            ConfigurableJoint.projectionMode = CharacterJoint.enableProjection ? JointProjectionMode.PositionAndRotation : JointProjectionMode.None;
            ConfigurableJoint.projectionDistance = CharacterJoint.projectionDistance;
            ConfigurableJoint.projectionAngle = CharacterJoint.projectionAngle;

            ConfigurableJoint.breakForce = CharacterJoint.breakForce;
            ConfigurableJoint.breakTorque = CharacterJoint.breakTorque;

            ConfigurableJoint.enableCollision = CharacterJoint.enableCollision;
            ConfigurableJoint.enablePreprocessing = CharacterJoint.enablePreprocessing;

            ConfigurableJoint.massScale = CharacterJoint.massScale;
            ConfigurableJoint.connectedMassScale = CharacterJoint.connectedMassScale;

            Component.DestroyImmediate(CharacterJoint);
            Debug.Log(ConfigurableJoint.name + " is converted");
        }
        Debug.Log("Converted successfully. All joints are CONFIGURABLE now.");
    }
    public static void ConfToChar(Transform root)
    {
        var ConfigurableJoints = root.GetComponentsInChildren<ConfigurableJoint>();
        foreach (var ConfigurableJoint in ConfigurableJoints)
        {
            var CharacterJoint = ConfigurableJoint.gameObject.AddComponent<CharacterJoint>();
            CharacterJoint.connectedBody = ConfigurableJoint.connectedBody;
            CharacterJoint.anchor = ConfigurableJoint.anchor;
            CharacterJoint.axis = ConfigurableJoint.axis;
            CharacterJoint.autoConfigureConnectedAnchor = ConfigurableJoint.autoConfigureConnectedAnchor;
            CharacterJoint.connectedAnchor = ConfigurableJoint.connectedAnchor;
            CharacterJoint.swingAxis = ConfigurableJoint.secondaryAxis;

            CharacterJoint.twistLimitSpring = ConfigurableJoint.angularXLimitSpring;
            CharacterJoint.swingLimitSpring = ConfigurableJoint.angularYZLimitSpring;

            CharacterJoint.highTwistLimit = ConfigurableJoint.highAngularXLimit;
            CharacterJoint.lowTwistLimit = ConfigurableJoint.lowAngularXLimit;
            CharacterJoint.swing1Limit = ConfigurableJoint.angularYLimit;
            CharacterJoint.swing2Limit = ConfigurableJoint.angularZLimit;

            CharacterJoint.enableProjection = ConfigurableJoint.projectionMode == JointProjectionMode.PositionAndRotation;
            CharacterJoint.projectionDistance = ConfigurableJoint.projectionDistance;
            CharacterJoint.projectionAngle = ConfigurableJoint.projectionAngle;

            CharacterJoint.breakForce = ConfigurableJoint.breakForce;
            CharacterJoint.breakTorque = ConfigurableJoint.breakTorque;

            CharacterJoint.enableCollision = ConfigurableJoint.enableCollision;
            CharacterJoint.enablePreprocessing = ConfigurableJoint.enablePreprocessing;

            CharacterJoint.massScale = ConfigurableJoint.massScale;
            CharacterJoint.connectedMassScale = ConfigurableJoint.connectedMassScale;

            Component.DestroyImmediate(ConfigurableJoint);
            Debug.Log(CharacterJoint.name + " is converted");
        }
        Debug.Log("Converted successfully. All joints are CHARACTER now.");
    }


    [MenuItem("CONTEXT/Rigidbody/Convert child joints (Character->Configurable)")]
    static void ConvertCharacterJointsToConfigurable()
    {
        CharToConf(Selection.activeTransform);
    }

    [MenuItem("CONTEXT/Rigidbody/Convert child joints (Configurable->Character)")]
    static void ConvertConfigurableJointsToCharacter()
    {
        ConfToChar(Selection.activeTransform);
    }
}
