# Mercury XM
## Mercury Cross-component Messaging Toolkit

Thank you for downloading the *Mercury XM* Toolkit!

*Mercury XM* is a new way to handle cross-component communication in the Unity
  game engine. It integrates seamlessly with the Unity Editor, and is both
  robust and very expandable.

The toolkit supports the *Mercury XM* Framework, which is a messaging
  and organizational framework built around the *Mercury XMP* (*Mercury XM*
Protocol). For information about the protocol, please see this document for
  a detailed explanation.

Unity organizes its rendered scene objects
(known in Unity as
  [GameObjects](https://docs.unity3d.com/ScriptReference/GameObject.html))
using a standard scene graph (known in Unity as the
  [Scene Hierarchy](https://docs.unity3d.com/Manual/Hierarchy.html)).
While Unity's implementation is very powerful,
it is fairly difficult to achieve non-spatial communication between
scriptable components of GameObjects (in Unity, known as
  [MonoBehaviours](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html))
.

As an example, imagine you are trying to show a line that connects one object
(say, a basketball) in one part of the scene, to a completely disconnected
  object (a player) in another part of the scene.
In the scene graph, they wouldn't be siblings, or even share an
  ancestor besides the scene root.
In addition, our line will have a small sphere at each end.
In total, we have 3 elements, the two endpoints, and the line.
Additionally , we want the line's color to gradually transition from the color
  of one endpoint, to the color of another.
Lastly, each component should be disabled when we decide to turn off the visual
  effect.

In many cases, it is simplest to make the spheres at each ends of the line
  be children of the ball and player, respectively.
This allows you to forget about the need to move those spheres with the
  ball and player.
But, what about the line itself?
Does it belong to the basketball or the player?
Conceptually, it's possible that the line belongs to neither, and may sit at the
  same level in the scene graph.

We need to consider how we update the positons of the line's
  endpoints, so that they move with the ball and player.
  We'll need a script in whose Update method we copy the
  [Transform.position](https://docs.unity3d.com/ScriptReference/Transform-position.html)
   from the tracked object, to the endpoint of line. This can be on the line
   or on a different script that would observe all related elements.

To make this even more difficult, let's imagine that you have now decided to
  connect the ball to *each* player on the court with individual lines and then
  another from the ball in the hoop. Now, there will be a control script on each
  line in the effect.

Normally, to disable a GameObject in Unity, you invoke the
  [SetActive](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
  method.
This will disable the GameObject and it's children in the scene hierarchy.
However, in our example, to disable the entire effect, you would need to
  go and disable the endpoint spheres and the line objects.
In a script, you would need to get a handle to the GameObjects, and invoke
  SetActive on each of them individually.  


In this simple example, the toolkit makes it easy to achieve this.

You drop an *MXM Node* (Mercury XM Node) and an *MXM Behavior* (Mercury XM
  Behavior) onto each of the related GameObjects in the effect.

Each *MXM Node* has a *MXM Behavior List*.
In the Line's *MXM Node*, you'll drag and drop the related components:
  the endpoint-spheres and line.
Then in a managing component, you can drag and drop each line's
  *MXM Node*.

In your line control script, you invoke the following method:

```
GetComponent<MxmNode>().MxmInvoke(MxmMethod.SetActive, true,
    new MxmControlBlock(WFLevelFilterHelper.Default, MxmActiveFilter.All,
    default(MxmSelectedFilter), MxmNetworkFilter.Local));
```

This will trigger a special SetActive message on each of the objects involved
  in the effect.

Done!

## Downloading Mercury XM

To get the Mercury XM Toolkit, please visit any of the following sources:

[Mercury XM Official Website](https:www.cs.columbia.edu/cgui/MercuryXM)

Download the *MercuryXM.unitypackage* file from the website.
In Unity, in the menu bar, select **Assets->Import Package->Custom package**.
Then navigate to where you stored the package file: *MercuryXM.unitypackage*.
Alternatively, simply drag and drop the *MercuryXM.unitypackage*
  file into the Assets folder in Unity's project view.
Double click the file and select import when the **Import**
  dialogue window appears.

[Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/)

You'll be able to import the package immediately after downloading.
Once it is finished downloading, double click the file and select import when
the **Import** dialogue window appears.

[GitHub](https://github.com/ColumbiaCGUI)

If you downloaded the source from GitHub, please drag and drop the
        root folder of Mercury XM, *MercuryXM* into the Assets folder of your
        project.

## Getting Started

Now that you have the toolkit installed, you'll probably want to check out a
tutorial.

Please see our tutorials page at the site: [Mercury XM Tutorials](https://www.cs.columbia.edu/cgui/MercuryXM/Tutorials).

## Documentation

Complete Documentation for the toolkit can be found on the toolkit's
[documentation page](https://www.cs.columbia.edu/cgui/MercuryXM/Documentation).

## FAQ
### Q. Does the toolkit work in Unity version 5.4.x, 4.x, 3.x, and earlier?

A. No. The toolkit requires some features that were added in Unity 5. As such,
we provide no support for the toolkit in earlier versions of Unity.

That said, it may work in other versions of Unity 5, but we're not sure.

### Q. What is Unity?

A. Unity is a game engine. Please see here:
[Unity](https://unity3d.com/).

### Q. Can I use the toolkit with Unreal, CryEngine, etc.

A. As much as we like those engines, we built the toolkit to support us in our
work in our lab, where we do all of our work in Unity.

We want to bring the toolkit to other platforms and are looking for
collaborators in doing so. Please contact
[Professor Steven Feiner](feiner@cs.columbia.edu) of Columbia University.
