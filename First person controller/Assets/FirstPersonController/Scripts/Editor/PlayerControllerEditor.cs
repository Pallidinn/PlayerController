using Cinemachine;
using Cinemachine.Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.HighDefinition;
using UnityEditorInternal;
using UnityEngine;
using static CharacterController;
using static UnityEngine.UI.Image;

[CustomEditor(typeof(CharacterController))]
public class CharacterControllerEditor : Editor
{
    //Camera settings
    SerializedProperty useHeadBobCurves;

    //Movement settings
    SerializedProperty dynamicCrouch;

    //Jump settings
    SerializedProperty jumpHeight;
    SerializedProperty maxJumps;

    //Gravity settings
    SerializedProperty groundCheckOrigin;
    SerializedProperty groundCheckDistance;
    SerializedProperty groundAngleCheckOrigin;
    SerializedProperty groundAngleCheckDistance;

    SerializedProperty attractor;

    //audio
    SerializedProperty enableDefaultSounds;
    SerializedProperty _footstepAudioClips;
    SerializedProperty _jumpingAudioClips;
    SerializedProperty _landingAudioClips;


    //Foldout groups
    bool cameraSettingsDD = false;
    bool movmentSettingsDD = false;
    bool jumpSettingsDD = false;
    bool gravitySettingsDD = false;
    bool groundChecksDD = false;
    bool objectAssignmentDD = false;
    bool AudioSettingsDD = false;



    private void OnEnable() {
        //Camera settings
        useHeadBobCurves = serializedObject.FindProperty("useHeadBobCurves");

        //Movement
        dynamicCrouch = serializedObject.FindProperty("dynamicCrouch");


        //Jump setttings
        jumpHeight = serializedObject.FindProperty("jumpHeight");
        maxJumps = serializedObject.FindProperty("maxJumps");

        //Gravity setttings
        groundCheckOrigin = serializedObject.FindProperty("groundCheckOrigin");
        groundCheckDistance = serializedObject.FindProperty("groundCheckDistance");
        groundAngleCheckOrigin = serializedObject.FindProperty("groundAngleCheckOrigin");
        groundAngleCheckDistance = serializedObject.FindProperty("groundAngleCheckDistance");

        attractor = serializedObject.FindProperty("attractor");

        _footstepAudioClips = serializedObject.FindProperty("footstepAudioClips");
        _jumpingAudioClips = serializedObject.FindProperty("jumpingAudioClips");
        _landingAudioClips = serializedObject.FindProperty("landingAudioClips");

        enableDefaultSounds = serializedObject.FindProperty("enableDefaultSounds");


    }

    public override void OnInspectorGUI() {
        //Base inspectotor 
        base.OnInspectorGUI();  
        EditorGUILayout.Space(20);

        //Reference to the charactrer controller
        CharacterController controller = (CharacterController)target;

        //Create text styles 
        GUIStyle catagoryStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter , fontSize = 15, richText = true};
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 25 };
        
        serializedObject.Update();
        Undo.RecordObject(controller, ("Changed player controller variable"));

        GUILayout.Label("Character controller", headerStyle);

        # region Camera settings
        cameraSettingsDD = EditorGUILayout.BeginFoldoutHeaderGroup(cameraSettingsDD, "Camera settings");
        if (cameraSettingsDD) {

            
            EditorGUILayout.LabelField("General camera settings", catagoryStyle);

            controller.lookSpeed = EditorGUILayout.FloatField(new GUIContent("Look speed", "The speed at which the players camera rotates."), controller.lookSpeed);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Camera angle limits", "The minimum and maximum angle that the players camera can look up or down."));
                GUILayout.Label("Min");
                controller.cameraAngleLimits.x = EditorGUILayout.FloatField(controller.cameraAngleLimits.x);
                GUILayout.Label("Max");
                controller.cameraAngleLimits.y = EditorGUILayout.FloatField(controller.cameraAngleLimits.y);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                controller.cameraFOVCurve = EditorGUILayout.CurveField(new GUIContent("Camera F.O.V. curve", "The 'field of view' of the players camera at certain speeds, The 'X' axis being the player speed and the 'Y' axis being the FOV at the point."), controller.cameraFOVCurve);
                GUILayout.Label(new GUIContent("Adjustment speed", "The speed that the FOV will adjust to the new target FOV for smoother transitions."));
                controller.cameraFOVAdjustSpeed = EditorGUILayout.FloatField(controller.cameraFOVAdjustSpeed);
            GUILayout.EndHorizontal();

            controller.cameraOffset = EditorGUILayout.FloatField(new GUIContent("Camera offset", "This setting adjusts the hight of the camera. The height is offset from the current player height."), controller.cameraOffset);
            controller.cameraArmLenght = EditorGUILayout.FloatField(new GUIContent("Camera arm lenght", "The distance the camera will try to maintain away from the player."), controller.cameraArmLenght);

            EditorGUILayout.Space(15f);

            EditorGUILayout.LabelField("Head bobbing", catagoryStyle);
            EditorGUILayout.PropertyField(useHeadBobCurves, new GUIContent("Use head bob curves", "Whether or not the head bob should use curves as its source for the frequency and amplitude."));
            if (controller.useHeadBobCurves) {
                controller.headBobFrequencyCurve = EditorGUILayout.CurveField(new GUIContent("Head bob frequency", "The speed at which the camera will oscillate. The 'X' axis being the speed of the player and the 'Y' axis being the frequency."), controller.headBobFrequencyCurve);
                controller.headBobAmplitudeCurve = EditorGUILayout.CurveField(new GUIContent("Head bob amplitude", "The strenght at which the camera will oscillate. The 'X' axis being the speed of the player and the 'Y' axis being the amplitude."), controller.headBobAmplitudeCurve);
            }
            else {
                controller.headBobFrequency = EditorGUILayout.FloatField(new GUIContent("Head bob frequency", "The speed at which the camera will oscillate."), controller.headBobFrequency);
                controller.headBobAmplitude = EditorGUILayout.FloatField(new GUIContent("Head bob amplitude", "The strenght at which the camera will oscillate."), controller.headBobAmplitude);
            }

            EditorGUILayout.Space(15f);

            EditorGUILayout.LabelField("Rotation modes", catagoryStyle);
            controller.cameraStyle = (CharacterController.cameraStyles)EditorGUILayout.EnumPopup(new GUIContent("Camera style", "When set to standard the player model will rotate to the direction of movement while only using 1 dimension of the animation graph (Forward and idle), and when set to locked the player will maintain the direction they are looking and use the full set of animations."), controller.cameraStyle);
            controller.TPRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Character rotation speed", "The speed that the player model will rotate to the new rotation."), controller.TPRotationSpeed);
            controller.cameraTarget = EditorGUILayout.ObjectField(new GUIContent("Camera Target", "If left as 'null' then nothing will happen but if there is a selected transform then the camera will remain focused on the object."), controller.cameraTarget, typeof(Transform), true) as Transform;
            


            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Movement settings
        movmentSettingsDD = EditorGUILayout.BeginFoldoutHeaderGroup(movmentSettingsDD, "Movment settings"); 
        if (movmentSettingsDD) {
            EditorGUILayout.LabelField("Speeds", catagoryStyle);
            controller.walkSpeed = EditorGUILayout.FloatField(new GUIContent("Walk speed", "The speed the player will walk."), controller.walkSpeed);
            controller.sprintSpeed = EditorGUILayout.FloatField(new GUIContent("Sprint speed", "The speed the player will move while sprinting."), controller.sprintSpeed);
            controller.walkCrouchSpeed = EditorGUILayout.FloatField(new GUIContent("Crouch speed", "The speed the player will move while crouching."), controller.walkCrouchSpeed);
            controller.sprintCrouchSpeed = EditorGUILayout.FloatField(new GUIContent("Sprint-Crouch speed", "The speed the player will move while both sprinting and crouching."), controller.sprintCrouchSpeed);
     
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Acceleration", catagoryStyle);
            controller.acceleration = EditorGUILayout.FloatField(new GUIContent("Acceleration", "The rate that the player accelerates."), controller.acceleration);
            controller.playerDrag = EditorGUILayout.FloatField(new GUIContent("Player drag", "The amount of drag the player feels when grounded."), controller.playerDrag);

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Stepping", catagoryStyle);
            controller.maxStepHeight = EditorGUILayout.FloatField(new GUIContent("Max step height", "The maximum height that a player can step up too."), controller.maxStepHeight);
            controller.stepSmoothingSpeed = EditorGUILayout.FloatField(new GUIContent("Step smooting speed", "The speed of the step interpolation."), controller.stepSmoothingSpeed);
            controller.maxSlopeAngle = EditorGUILayout.FloatField(new GUIContent("Max slope angle", "The maximum slope detection angle."), controller.maxSlopeAngle);

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Crouching", catagoryStyle);
            controller.walkingHeight = EditorGUILayout.FloatField(new GUIContent("Walking height", "The height that the player is when walking."), controller.walkingHeight);
            controller.crouchingHeight = EditorGUILayout.FloatField(new GUIContent("Crouching height", "The height that player is when crouched."), controller.crouchingHeight);
            controller.crouchSpeed = EditorGUILayout.FloatField(new GUIContent("Crouch speed", "The speed that the player will crouch."), controller.crouchSpeed);
            EditorGUILayout.PropertyField(dynamicCrouch, new GUIContent("Dynamic crouch", "When enabled the player will be able to incrementally stand up as opposed to waiting till the player has the full head room to stand up."));
            if (controller.dynamicCrouch) {
                controller.dynamicCrouchOffset = EditorGUILayout.FloatField(new GUIContent("Dynamic crouch offset", "The amount of extra clearance givin to the players head when using dynamic crouch."), controller.dynamicCrouchOffset);
            }

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Jump settings
        jumpSettingsDD = EditorGUILayout.BeginFoldoutHeaderGroup(jumpSettingsDD, "Jump settings");
        if(jumpSettingsDD) {
            controller.jumpMode = (CharacterController.jumpModes)EditorGUILayout.EnumPopup(new GUIContent("Jump mode", "Changes the style of jumping between various modes."), controller.jumpMode);

            switch (controller.jumpMode) {
                case CharacterController.jumpModes.Standard:
                    EditorGUILayout.PropertyField(jumpHeight, new GUIContent("Jump height", "This is the height that the player will jump. Note that due to the way the physics system works within unity the player will always reach just under this height."));
                    break;

                case CharacterController.jumpModes.Charge:
                    controller.chargeCurve = EditorGUILayout.CurveField(new GUIContent("Jump charge curve", "The height that the player will jump too depending on how long they hold the jump button. The 'X' axis represents the time and the 'Y' axis represents the height."), controller.chargeCurve);
                    break;

                case CharacterController.jumpModes.Hold:
                    EditorGUILayout.PropertyField(jumpHeight, new GUIContent("Jump power", "This is the force applied to the player every frame."));
                    break;
            }

            EditorGUILayout.PropertyField(maxJumps, new GUIContent("Max jumps", "If this number is more than one the player will be able to complete that amount of jumps before landing again. If this value is set to 1 then it will enable 'cyote time'."));
            
            if(controller.maxJumps == 1) controller.coyoteTime = EditorGUILayout.FloatField(new GUIContent("Coyote time", "The amount of time after the player falls off a ledge where they can still jump. Note this is only enabled when the 'Max jumps' is set to 1."), controller.coyoteTime);

            controller.jumpBufferMode = (CharacterController.jumpBufferModes)EditorGUILayout.EnumPopup(new GUIContent("Jump buffer", "When jump buffer is enabled, the player will be able to press the jump button before landing and it will cache the jump to be used straight away after landing."), controller.jumpBufferMode);
            
            if (controller.jumpBufferMode == jumpBufferModes.Single) {
                controller.maxJumpBuffer = EditorGUILayout.FloatField(new GUIContent("Max jump buffer", "The maximum amount of time before landing where a jump will be added to the buffer."), controller.maxJumpBuffer);

            }

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Gravity settings
        gravitySettingsDD = EditorGUILayout.BeginFoldoutHeaderGroup(gravitySettingsDD, "Gravity settings");
        if (gravitySettingsDD) {
            controller.dynamicGravityLimit = EditorGUILayout.FloatField(new GUIContent("Dynamic gravity limit", "The maximum angle in degrees that the player will to attempt to adjust their rotation. Note that beacuase of floating point precision error it sometimes may be necessary to give a slight extra margin of a degree or so."), controller.dynamicGravityLimit);
            controller.gravityChangeSpeed = EditorGUILayout.FloatField(new GUIContent("Gravity change speed", "The speed that the player will rotate to the new gravity direction."), controller.gravityChangeSpeed);
            EditorGUILayout.PropertyField(attractor, new GUIContent("Attractor", "If attractor is assigned a transform it will change the gravity to face in the direction of the origion of the object. This is usefull for things such as planets."));

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Ground checks
        groundChecksDD = EditorGUILayout.BeginFoldoutHeaderGroup(groundChecksDD, "Ground checks");
        if (groundChecksDD) {
            controller.groundCheckOffset = EditorGUILayout.FloatField(new GUIContent("Ground check offset", "The distance off the ground the check will be tested from."), controller.groundCheckOffset);
            controller.groundCheckSize = EditorGUILayout.FloatField(new GUIContent("Ground check size", "The size of the check performed. The bigger this value, the further away the player will atempt to step up onto something"), controller.groundCheckSize);
            controller.horizontalCheckDistance = EditorGUILayout.FloatField(new GUIContent("Horizontal check distance", "The distance checked in the horizontal direction. This is used for testing if a player can walk up a wall."), controller.horizontalCheckDistance);
            controller.verticalCheckDistance = EditorGUILayout.FloatField(new GUIContent("Vertical check distance", "The distance checked in the vertical direction. This is used for testing if a player can be realigned to a new dynamic gravity face."), controller.verticalCheckDistance);
            controller.groundCheckCoolDown = EditorGUILayout.FloatField(new GUIContent("Ground check cooldown", "The amount of time after not being grounded that the player will check if it is grounded again. This is used for not immediately setting the player as grounded for the few frames where the player is still in range of the ground checks."), controller.groundCheckCoolDown);

            EditorGUILayout.Space(15f);

            LayerMask groundMask = EditorGUILayout.MaskField(new GUIContent("Ground layer", "The layer that will be used for testing if the player is standing on something."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(controller.groundLayer), InternalEditorUtility.layers);
            controller.groundLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(groundMask);

            LayerMask dynamicGravityMask = EditorGUILayout.MaskField(new GUIContent("Dynamic gravity layer", "The layer that will be used for testing if the player is touching a dynamic gravity surface."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(controller.dynamicGravityLayer), InternalEditorUtility.layers);
            controller.dynamicGravityLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(dynamicGravityMask);

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Audio settings
        AudioSettingsDD = EditorGUILayout.BeginFoldoutHeaderGroup(AudioSettingsDD, "Audio settings");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if(AudioSettingsDD) {
            controller.walkStepTime = EditorGUILayout.FloatField(new GUIContent("Walk step time", "The amount of time between each sound footstep at walking speed. The frequecy on the footsteps will increase as the players speed increases."), controller.walkStepTime);
            EditorGUILayout.PropertyField(enableDefaultSounds, new GUIContent("Enable default sounds", "If the player is standing on an object with no tag match, The first set os sounds in the list will be used."));

            EditorGUILayout.LabelField("Audio clips", catagoryStyle);

            EditorGUILayout.PropertyField(_footstepAudioClips);
            EditorGUILayout.PropertyField(_jumpingAudioClips);
            EditorGUILayout.PropertyField(_landingAudioClips);

            EditorGUILayout.Space(20);
        }
        #endregion

        #region Object assignment
        objectAssignmentDD = EditorGUILayout.BeginFoldoutHeaderGroup(objectAssignmentDD, "Object assignment");
        if (objectAssignmentDD) {
            controller.playerObject = EditorGUILayout.ObjectField(new GUIContent("Player object", "The GameObject will be used for animations and directional rotation if enabled."), controller.playerObject, typeof(Transform), true) as Transform;
            controller.playerCamera = EditorGUILayout.ObjectField(new GUIContent("Player camera", "A reference to the players camera. This must be a Cinemachine camera."), controller.playerCamera, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            controller.rb = EditorGUILayout.ObjectField(new GUIContent("Rigidbody", "A reference to the players Rigidbody."), controller.rb, typeof(Rigidbody), true) as Rigidbody;
            controller.stepCollider = EditorGUILayout.ObjectField(new GUIContent("Step collider", "A reference to the capsule collider used for detecting stepping."), controller.stepCollider, typeof(CapsuleCollider), true) as CapsuleCollider;

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}



