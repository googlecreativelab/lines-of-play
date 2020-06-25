This sample demonstrates how to use ARCore's Depth From Motion feature.

To use this sample, add '/MotionStereo/Scenes/ObjectBasedOcclusion.unity' as
your project's start scene and build for the target device. The default scene is
configured to display an AR object at a fixed distance from the camera.

Depth data is retrieved each frame from ARCore and set as a material property
on the android using 'Scripts/AttachDepthTexture'. It is used as a clip test in
the customized surface shader 'Materials/ObjectOcclusionShader.shader'.

To help understand the depth data, 'Prefabs/GreyScalePreview', when attached
to the scene camera, will display a grey-scale version of the depth data.

The scene '/MotionStereo/Scenes/ObjectBasedOcclusion.unity' applies occlusions
to all 3D objects using an image effect found in 'Scripts/OcclusionsImageEffect'
which is added to the camera.
