using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/* INSTRUCTIONS
To use the "List2D_Blah" classes, in your script, make a variable that is a List of the List2D_Blah class you need, and then access it in code using "List2D_Blah[x].inner[y]";

Also, by doing it this way, you can effectivly make a 2D List (a List of Lists) appear in the inspector.

I used the base classes for as many as possible. This means that whether you need a List2D for a SpriteRenderer, or a MeshRenderer,
you would use the List2D_Renderer, because both of those (and all other renderers) inherit from the Renderer class. The same is true for the Colliders, Selectables (which are any UI Element that allows input from the player
like, Sliders, Buttons, Toggles, Input Fields, etc...), any Components not listed in the code below (use the "List2D_OtherComponent" class), other data types not listed (use the List2D_Object class)

Also, since the classes are all layed out the same, if you need one of a data type that isn't on here, it is quite simple to add it, though I tried to add as many as I could think of.
Information Source: https://answers.unity.com/questions/148447/how-do-make-a-public-array-of-arrays-appear-in-the.html 
*/
/*EXAMPLES
[To Create]
public List<List2D_Int> myList;

[To Access]
myList[x].inner.[y] = 2;
or
int blah = myList[2].inner[4];
or
myList[x].Add(new List2D_Int);
or
myList[x].inner.Add(4);
etc...
 
*/

#region Data Types
[Serializable] public class List2D_Float
{ public List<float> inner = new List<float>(); }
[Serializable] public class List2D_Int
{ public List<int> inner = new List<int>(); }
[Serializable] public class List2D_Double
{ List<double> inner = new List<double>(); }
[Serializable] public class List2D_Decimal
{ public List<decimal> inner = new List<decimal>(); }
[Serializable] public class List2D_Int16
{ public List<Int16> inner = new List<Int16>(); }
[Serializable] public class List2D_Int32
{ public List<Int32> inner = new List<Int32>(); }
[Serializable] public class List2D_Int64
{ public List<Int64> inner = new List<Int64>(); }
[Serializable] public class List2D_Byte
{ public List<byte> inner = new List<byte>(); }
[Serializable] public class List2D_Vector2
{ public List<Vector2> inner = new List<Vector2>(); }
[Serializable] public class List2D_Vector3
{ public List<Vector3> inner = new List<Vector3>(); }
[Serializable] public class List2D_Vector4
{ public List<Vector4> inner = new List<Vector4>(); }
[Serializable] public class List2D_Quaternion
{ public List<Quaternion> inner = new List<Quaternion>(); }
[Serializable] public class List2D_Time
{ public List<Time> inner = new List<Time>(); }
[Serializable] public class List2D_DateTime
{ public List<DateTime> inner = new List<DateTime>(); }
[Serializable] public class List2D_String
{ public List<String> inner = new List<string>(); }
[Serializable] public class List2D_Char
{ public List<char> inner = new List<char>(); }
[Serializable] public class List2D_Bool
{ public List<bool> inner = new List<bool>(); }
[Serializable] public class List2D_Object
{ public List<object> inner = new List<object>(); }
[Serializable] public class List2D_Ray
{ public List<Ray> inner = new List<Ray>(); }
[Serializable] public class List2D_Ray2D
{ public List<Ray2D> inner = new List<Ray2D>(); }
[Serializable] public class List2D_RaycastHit
{ public List<RaycastHit> inner = new List<RaycastHit>(); }
[Serializable] public class List2D_RaycastHit2D
{ public List<RaycastHit2D> inner = new List<RaycastHit2D>(); }
[Serializable] public class List2D_Color
{ public List<Color> inner = new List<Color>(); }
[Serializable] public class List2D_Collision
{ public List<Collision> inner = new List<Collision>(); }
[Serializable] public class List2D_Collision2D
{ public List<Collision2D> inner = new List<Collision2D>(); }
[Serializable] public class List2D_Enum1
{ public List<IEnumerable> inner = new List<IEnumerable>(); }
[Serializable] public class List2D_Enum2
{ public List<IEnumerator> inner = new List<IEnumerator>(); }

#endregion

# region Asset Types
[Serializable] public class List2D_PhysicsMaterial
{ public List<PhysicMaterial> inner = new List<PhysicMaterial>(); }
[Serializable] public class List2D_PhysicsMaterial2D
{ public List<PhysicsMaterial2D> inner = new List<PhysicsMaterial2D>(); }
[Serializable] public class List2D_Sprite
{ public List<Sprite> inner = new List<Sprite>(); }
[Serializable] public class List2D_Mesh
{ public List<Mesh> inner = new List<Mesh>(); }
[Serializable] public class List2D_AudioClip
{ public List<AudioClip> inner = new List<AudioClip>(); }
[Serializable] public class List2D_Texture
{ public List<Texture> inner = new List<Texture>(); }
[Serializable] public class List2D_Texture2D
{ public List<Texture2D> inner = new List<Texture2D>(); }
[Serializable] public class List2D_Texture3D
{ public List<Texture3D> inner = new List<Texture3D>(); }
[Serializable] public class List2D_Material
{ public List<Material> inner = new List<Material>(); }
#endregion

# region Components
[Serializable] public class List2D_GameObject
{ public List<GameObject> inner = new List<GameObject>(); }
[Serializable] public class List2D_Tranform
{ public List<Transform> inner = new List<Transform>(); }
[Serializable] public class List2D_Camera
{ public List<Camera> inner = new List<Camera>(); }
[Serializable] public class List2D_AudioSource
{ public List<AudioSource> inner = new List<AudioSource>(); }
[Serializable] public class List2D_RigidBody
{ public List<Rigidbody> inner = new List<Rigidbody>(); }
[Serializable] public class List2D_RigidBody2D
{ public List<Rigidbody2D> inner = new List<Rigidbody2D>(); }
[Serializable] public class List2D_Collider
{ public List<Collider> inner = new List<Collider>(); }
[Serializable] public class List2D_Collider2D
{ public List<Collider2D> inner = new List<Collider2D>(); }
[Serializable] public class List2D_Renderer
{ public List<Renderer> inner = new List<Renderer>(); }
[Serializable] public class List2D_Rect
{ public List<Rect> inner = new List<Rect>(); }
[Serializable] public class List2D_RectTransform
{ public List<RectTransform> inner = new List<RectTransform>(); }
[Serializable] public class List2D_Image
{ public List<Image> inner = new List<Image>(); }
[Serializable] public class List2D_Selectable
{ public List<Selectable> inner = new List<Selectable>(); }
[Serializable] public class List2D_Text
{ public List<Text> inner = new List<Text>(); }
[Serializable] public class List2D_OtherComponents
{ public List<Component> inner = new List<Component>(); }
#endregion