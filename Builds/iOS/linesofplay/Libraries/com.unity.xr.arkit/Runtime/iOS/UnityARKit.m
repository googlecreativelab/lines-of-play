#import <Foundation/Foundation.h>
#include "IUnityInterface.h"
#include "UnityAppController.h"

void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityARKitXRPlugin_PluginLoad(IUnityInterfaces* unityInterfaces);
extern void UnityARKit_SetRootView(UIView* view);

void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityARKit_EnsureRootViewIsSetup()
{
    UnityARKit_SetRootView(_UnityAppController.rootView);
}

@interface UnityARKit : NSObject

+ (void)loadPlugin;

@end

@implementation UnityARKit

+ (void)loadPlugin
{
    // This registers our plugin with Unity
    UnityRegisterRenderingPluginV5(UnityARKitXRPlugin_PluginLoad, NULL);

    // This sets up some data our plugin will need later
    UnityARKit_EnsureRootViewIsSetup();
}

@end
