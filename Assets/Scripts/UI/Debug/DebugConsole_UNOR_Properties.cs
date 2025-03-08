/*
# U-NOR (Unity Notated Object Reference) v0.0 - Ideatic Evaluation

## Overview

U-NOR (Unity Notated Object Reference) is a serialization format designed to describe and manipulate Unity objects. It extends JSON with Unity-specific object referencing, component management, hierarchical traversal, and instantiation mechanisms.

---

## Syntax and Structure

- **Base Format:** U-NOR follows JSON conventions with `{}` for objects and `[]` for lists.
- **Keys:**
  - Standard property names or special references using `()` (for components) and `.` (for nested properties).
  - `"one.two"` accesses property `"two"` of the value at `"one"`. If any property in the chain is missing, the key is ignored.
  - **Component References:**
    - `"(...)"` instantiates a component. Example: `{ "(...)": @<res:MyScript> }`
    - `"(ComponentName)"` overrides an existing component. Example: `{ "(Rigidbody)": @<res:CustomPhysics> }`

  - **Nested Property Access:**
    - `"property.component"`
    - `"component.property"`
    - `"property.(component)"`
  - As stated further down if the implementation does not allow standalone U-NOR values and no root keys are allowed one can use "#" as a valid key, so spec wise any string is allowed, but strings with "." is generally seen as nested properties.


- **Values:** Standard JSON types (numbers, strings, booleans, lists, objects), Unity-specific references, or instantiations.

---

## Object References

U-NOR supports structured referencing for Unity scene objects, resources, asset bundles, and events:

### **Direct Object References**

- **Scene Object:** `<object_name>` → `{ "player": <MainCharacter> }`
- **Resource Object:** `<res:resource_name_or_path>` → `{ "enemyPrefab": <res:Enemies/Orc> }`
  - **Resources:** Can reference anything in the Unity **Resources** folder, including prefabs, scriptable objects, textures, and more.
- **Asset Bundle Object:** `<asset_bundle:bundle_name/asset_name>` → `{ "enemyPrefab": <asset_bundle:MainAssets/Orc> }`
  - **Asset Bundles:** Allows referencing assets from Unity's Asset Bundles.
- **Event References:** `[event_name]` → `{ "[OnClick]": <UI/Button>[OnClick] }`
  - **Note:** Event keys are valid **only** if the value is an object reference with the event as the last property (e.g., `"[Event]"`, `"property.[Event]"`, `"(component).[Event]"`).

### **Hierarchical Traversal**

- Use `/` to navigate child objects → `{ "target": <MainCharacter/Weapon/Sword> }`

### **Handling Multiple Matches**

- If multiple objects match a reference, they return as a list.
- Use `~index` to select specific matches:
  - First match: `<object_path~0>`
  - Last match: `<object_path~-1>`
  - Specific index: `<object_path~i>`
  - If the index is out of bounds, the first match is used.

### **Tag-Based Object Retrieval**

- Use `#tag_name` to retrieve objects by tag → `{ "enemy": #BossEnemy }`
- If multiple objects share the tag, a list is returned.
- Indexing applies to tags as well → `{ "enemy": #my_tag~1 }`
- If no object is found, the property is ignored.

---

## Object Instantiation

U-NOR allows instantiating objects dynamically using `@`:

- **From a Resource:** `@<res:resource_name_or_path>`
- **From a Scene Object:** `@<object_name_or_path>`
- **From an Asset Bundle:** `@<asset_bundle:bundle_name/asset_name>`
- **With Additional Properties:** `{ "instance": @<res:Prefab>{"health":100} }`

If referencing a GameObject, it is instantiated. If referencing a component, the containing GameObject is instantiated.

---

## Special Data Types and Rules

- **Accepted Values:**
  - JSON-compatible types: `string`, `number`, `boolean`, `null`, `object`, `array`
  - Unity references: `<object_ref>`, `<res:resource_ref>`, `<asset_bundle:bundle_ref>`, `#tag_name`
  - U-NOR 
  - `inf` and `-inf` are valid numbers.
- **Ignored Properties:** If a referenced object or tag is not found, the property is ignored.
- (It is important to note that any valid U-NOR value that does is not stricty tied to a type of key is valid U-NOR it it self, so "string" is valid U-NOR, however object-references ending in  (component) or [Event] are not. Please note that it is up to implementation if "Standalone U-NOR" is allowed, when using U-NOR to refer properties of a specific object it is generally not permitted. For such cases where U-NOR does not refer properties of a specific object and standalone is not allowed it and no desicion of root keys are found the key "#" is spec allowed as a key.


---

## Key Specific Rules

1. **Component-Specific Keys:**
   - If a key is a **component** or **nested property of a component**, and the value is not an instantiation or `null`, the key is ignored.
   - **Example of a valid component assignment:**
     - `{ "(Rigidbody)": @<res:CustomPhysics> }`
   - **Removing components:** If a key is a **component** (e.g., `"(Rigidbody)"`) and the value is `null`, the component would be removed. **Note:** **Removing components via `null` is no longer part of U-NOR** as per your request.
  
2. **Event Handling:**
   - **[Event]** is a valid key **only** if the value is an object reference with an event as the last property.
     - **Valid:** `{ "[OnClick]": <UI/Button>[OnClick] }`
     - **Valid:** `{ "[OnClick]": <UI/Button>["OnClick"] }`
     - **Invalid:** `<UI/Button>[OnClick]` is not valid. The correct syntax is `<UI/Button>.[OnClick]` (the event `[OnClick]` must come after the object reference).
     - Events are always accessed as the last property, for example: `"[Event]"`, `"property.[Event]"`, or `"(component).[Event]"`.

3. **Non-Instantiating Object References and Property Access:**
   - Non-instantiating object references can include nested properties like `<my_scene_object>.property` or `<my_scene_object>.(component)` (as long as they are not instantiations).
     - **Example of valid property access:** `{ "playerWeapon": <MainCharacter/Weapon>.damage }`
     - **Example of valid component reference:** `{ "playerRigidbody": <MainCharacter>.(Rigidbody) }`
   - This can also be nested, just like accessing properties in keys.
     - **Example:** `{ "playerWeapon": <MainCharacter/Weapon>.damage, "playerHealth": <MainCharacter/Health>.value }`

4. **Component References with Nested Keys:**
   - If a key is referencing a component (e.g., `(component)`), the last property must be `(component)` or `(…)`. Otherwise, the key is ignored.
   - **Valid:** `{ "(Rigidbody)": @<res:CustomPhysics> }`
   - **Valid:** `{ "(Collider)": null }`
   - **Invalid:** `{ "player.health": (Collider) }`

5. **Instantiations and Object References:**
   - Instantiations are valid as values and can be nested. For example, `{ "enemy": @<res:Prefab>{"weapon": @<res:WeaponPrefab>} }` is valid.
   - **Example:** `{ "enemy": @<res:Prefab>{"weapon": @<res:WeaponPrefab>{"damage": 50} } }`

6. **Valid Lists:**
   - Since U-NOR builds on JSON, lists can contain any type, including strings, numbers, object references, and even other lists.
     - **Valid:** `[1, "string", <my_scene_object>, ["another_list"]]`
     - Lists can include mixed types and can also contain other object references.
   - **Important Note**: Since U-NOR builds on JSON's flexible structure, lists can contain mixed types (e.g., numbers, strings, object references, etc.). When implementing U-NOR in a strictly typed language, this flexibility should be taken into account, as the language may require additional handling or validation for such dynamic types within lists.

---

## Example Usage

u-nor
{
  "player": <MainCharacter>,
  "weapon": <MainCharacter/Weapon>,
  "enemy": #EnemyTag,
  "spawnPoint": <res:SpawnLocations~0>,
  "newObject": @<res:Prefab>{"health":100, "speed":5},
  "(Rigidbody)": @<res:CustomPhysics>,
  "stats.health": 100,
  "stats.(CombatHandler).attackPower": 50,
  "specialValues": { "infinity": inf, "negativeInfinity": -inf, "nullValue": null },
  "[OnClick]": <UI/Button>.[OnClick],
  "player.weapon": <MainCharacter/Weapon>,
  "player.(Rigidbody)": null,
  "enemy.spawnPoint": <res:SpawnLocations~1>,
  "enemy": @<res:Prefab>{ "weapon": @<res:WeaponPrefab>{"damage": 50} },
  "player.(Rigidbody)": @<res:CustomPhysics>,
  "nestedObjects": [
    { "weapon": @<res:WeaponPrefab>, "health": 100 },
    { "enemy": @<res:EnemyPrefab> }
  ]
}
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using System.Text;

public class DebugConsole_UNOR_Properties : MonoBehaviour {
    /*
    U-NOR Object Reference:
        Getting an object by reference:
            <my_scene_object> <my_scene_parent_object>/<my_scene_object>
            <res:resource_name> <res:resource_subfolder/resource_name>
            <asset_bundle:bundle_name/asset_name> <asset_bundle:bundle_path/asset_name>

            // Since object-references are by name there could be multiple matches for in-scene-objects
            //   in that case object-references will return a list, to access a specific object in the list use ~i (-1) is allowed.
            <my_scene_object~0> <my_scene_object~1> <my_scene_object~-1>
            // (The syntax is valid for all object-references no matter if they can match multiple objects or not)

            // When getting an object we can also get a property of the object using a property path following a dot following the reference.
            <my_scene_object>.property <my_scene_object>.(component).property
            // Allowing any number of properties just like in keypaths.
            // The last keypath-segment is allowed to be a component but only if the keypath has a component or new-component as their last keypath-segment.
            <my_scene_object>.(component) <my_scene_object>.property.(component)

            // One can also get an event from an object, but only if the last keypath-segment is an event.
            <my_scene_object>.[event_name] <my_scene_object>.property.[event_name]

            // Finally one can use tags to get objects by tag, here indexes are of course allowed too.

        Instantiating an object by reference:
            // We can instantiate from a scene-object, in this case an instance of the object's gameObject property is made.
            @<my_scene_object> @<my_scene_parent_object>/<my_scene_object>
            // We can also instantiate from a resource.
            @<res:resource_name> @<res:resource_subfolder/resource_name>
            // And from an asset bundle.
            @<asset_bundle:bundle_name/asset_name> @<asset_bundle:bundle_path/asset_name>

            // When instantiating an object we can also include properties to set on the object, in this case we add extra U-NOR data at the end:
            @<my_scene_object>{"property":value,"property":value}
            // A space is NOT allowed between the object-reference and the U-NOR data. but spaces are allowed in the U-NOR just like in json.

            // One can not instantiate using a tag.

            // Of course indexes are allowed here too.

    */
    public struct UNOR_ObjectReference {

        private string org_reference;
        private string org_keypathType; // property, component, new-component, event

        public string type;             // object, resource, asset_bundle, tag
#nullable enable
        public int? index;
        public string[]? propertyPath;  // Property path split by "."
        public string? unor;            // If instantiating and unor is present
        public string? reference;       // The reference string (inbetween the <>)
#nullable disable
        public bool isInstantiation;
        public bool isPath;             // If the reference is a hiarchy path/resource path or not

        public UNOR_ObjectReference(string referenceStr, string keypathType = "property") { // property, component, new-component, event

            org_reference = referenceStr;
            org_keypathType = keypathType;

            propertyPath = null;
            unor = null;
            reference = null;

            // Call validation
            if (!Validate(referenceStr, keypathType)) {
                throw new Exception("U-NOR: Invalid object-reference " + referenceStr + "!");
            }

            // Parse the reference string and set the properties of the object.
            if (referenceStr.StartsWith("@")) {
                isInstantiation = true;
                referenceStr = referenceStr.Substring(1);
            } else {
                isInstantiation = false;
            }

            if (referenceStr.StartsWith("<")) {
                referenceStr = referenceStr.Substring(1, referenceStr.Length - 2);
            }

            if (referenceStr.StartsWith("res:")) {
                type = "resource";
                referenceStr = referenceStr.Substring(4);
            } else if (referenceStr.StartsWith("#")) {
                type = "tag";
                referenceStr = referenceStr.Substring(1);
            } else if (referenceStr.StartsWith("asset_bundle:")) {
                type = "asset_bundle";
                referenceStr = referenceStr.Substring(12);
            } else {
                type = "object";
            }

            if (referenceStr.Contains("~")) {
                string[] tilde_parts = referenceStr.Split("~");
                referenceStr = tilde_parts[0];
                index = int.Parse(tilde_parts[1]);
            } else {
                index = null;
            }

            if (referenceStr.Contains("/")) {
                isPath = true;
            } else {
                isPath = false;
            }


            string[] lt_parts = referenceStr.Split(">");
            if (isInstantiation) {
                // If there is unor data set it, do this by splitting by > and check if 2nd part begins with { if so we got unor
                if (lt_parts.Length > 1 && lt_parts[1].StartsWith("{")) {
                    unor = string.Join(">", lt_parts.Skip(1));
                }
            } else {
                // If there is a property path set it, do this by splitting by > and check if 2nd part begins with . if so we got properties
                if (lt_parts.Length > 1 && lt_parts[1].StartsWith(".")) {
                    propertyPath = lt_parts[1].Split(".");
                }
            }
        }

        public static bool Validate(string referenceStr, string keypathType = "property") {
            // Validate the reference string, ex. check if the reference is not an instantiation if so if it has properties if so the type of the last keypath-segment and compare to the keypathType.
            if (!referenceStr.StartsWith("@")) {
                // Check if there are properties if so split and get the last one
                //// split by > if there is a dot at the start of the 2nd element we got properties
                string[] parts = referenceStr.Split(">");
                if (parts.Length > 1 && parts[1].StartsWith(".")) {
                    // Split the properties by "," and get the last one
                    string[] properties = parts[1].Split(",");
                    string lastProperty = properties[properties.Length - 1];
                    // Check if the last property is a component or new-component
                    if (lastProperty.StartsWith("(")) {
                        // Check if the keypathType is component or new-component
                        if (keypathType != "component" && keypathType != "new-component") {
                            return false;
                        }
                    }

                    // Check if the last property is an event
                    if (lastProperty.StartsWith("[")) {
                        // Check if the keypathType is event
                        if (keypathType != "event") {
                            return false;
                        }
                    }
                }
            } else if (referenceStr.StartsWith("@#")) {
                return false; // Instantiation from tag is not allowed
            }
            return true; //MARK: [ToDo] More parsing might be needed to validate the actuall reference.
        }

        private static Component GetComponentByName(GameObject target, string componentName) {
            Type type = Type.GetType(componentName);
            if (type == null) return null;
            return target.GetComponent(type);
        }

        private static bool AddComponentByName(GameObject target, string componentName, object component) {
            Type type = Type.GetType(componentName);
            if (type == null || !(component is Component)) return false;

            Component existingComponent = target.GetComponent(type);
            if (existingComponent != null) {
                GameObject.Destroy(existingComponent);
            }

            target.AddComponent(type);
            return true;
        }

        private static bool RemoveComponentByName(GameObject target, string componentName) {
            Type type = Type.GetType(componentName);
            if (type == null) return false;

            Component component = target.GetComponent(type);
            if (component != null) {
                GameObject.Destroy(component);
                return true;
            }

            return false;
        }

        private static bool CopyComponentByName(GameObject target, string targetComponentName, GameObject source, string sourceComponentName) {
            Type sourceType = Type.GetType(sourceComponentName);
            Type targetType = Type.GetType(targetComponentName);
            if (sourceType == null || targetType == null) return false;

            Component sourceComponent = source.GetComponent(sourceType);
            if (sourceComponent == null) return false;

            Component targetComponent = target.GetComponent(targetType);
            if (targetComponent != null) {
                GameObject.Destroy(targetComponent);
            }

            targetComponent = target.AddComponent(sourceType);
            if (targetComponent == null) return false;

            foreach (var field in sourceType.GetFields()) {
                field.SetValue(targetComponent, field.GetValue(sourceComponent));
            }

            foreach (var prop in sourceType.GetProperties()) {
                if (prop.CanWrite) {
                    prop.SetValue(targetComponent, prop.GetValue(sourceComponent));
                }
            }

            return true;
        }

        public static bool AddEventSubscribersFromObj(GameObject target, string targetEventName, GameObject source, string sourceEventName) {
            if (target == null || source == null)
                return false;

            Component sourceComponent = source.GetComponent<Component>();
            Component targetComponent = target.GetComponent<Component>();

            if (sourceComponent == null || targetComponent == null)
                return false;

            EventInfo sourceEvent = sourceComponent.GetType().GetEvent(sourceEventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (sourceEvent == null)
                return false;

            Delegate sourceDelegate = GetEventSubscribers(sourceComponent, sourceEvent);
            if (sourceDelegate == null)
                return false;

            EventInfo targetEvent = targetComponent.GetType().GetEvent(targetEventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetEvent == null) {
                Debug.LogError($"Target event '{targetEventName}' not found on {target.name}. You may need to define it.");
                return false;
            }

            foreach (Delegate handler in sourceDelegate.GetInvocationList()) {
                targetEvent.AddEventHandler(targetComponent, handler);
            }

            return true;
        }

        private static bool RemoveEventSubscribersFromObj(GameObject target, string eventName) {
            if (target == null)
                return false;

            Component targetComponent = target.GetComponent<Component>();
            if (targetComponent == null)
                return false;

            EventInfo targetEvent = targetComponent.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetEvent == null)
                return false;

            Delegate targetDelegate = GetEventSubscribers(targetComponent, targetEvent);
            if (targetDelegate == null)
                return false;

            foreach (Delegate handler in targetDelegate.GetInvocationList()) {
                targetEvent.RemoveEventHandler(targetComponent, handler);
            }

            return true;
        }

        private static Delegate GetEventSubscribers(Component component, EventInfo eventInfo) {
            FieldInfo field = component.GetType().GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            return field?.GetValue(component) as Delegate;
        }

        private static GameObject[] FindSceneObjects(string path, int? index = null) {
            GameObject[] foundObjects;

            if (path.Contains("/")) // If it's a hierarchy path
            {
                foundObjects = new GameObject[] { GameObject.Find(path) };
            } else // If it's just an object name
              {
                foundObjects = new GameObject[] { GameObject.Find(path) }
                    .Where(t => t.name == path)
                    .ToArray();
            }

            // If no objects are found, return an empty array
            if (foundObjects.Length == 0) {
                return new GameObject[0];
            }

            // If index is provided, return the object at that index (support negative indices)
            if (index.HasValue) {
                int adjustedIndex = index.Value < 0 ? foundObjects.Length + index.Value : index.Value;
                return adjustedIndex >= 0 && adjustedIndex < foundObjects.Length
                    ? new GameObject[] { foundObjects[adjustedIndex] }
                    : new GameObject[] { foundObjects[0] }; // Return the first element if index is out of bounds
            }

            // Return the full list if no index is provided
            return foundObjects;
        }

        private static GameObject[] FindResourceObjects(string path, int? index = null) {
            GameObject[] foundObjects;

            if (path.EndsWith("/*")) {
                // Remove "/*" and find all matching objects in the path
                string pathWithoutWildcard = path.Substring(0, path.Length - 2);
                foundObjects = Resources.LoadAll<GameObject>(pathWithoutWildcard);
            } else {
                // Find a single object by the exact path
                GameObject obj = Resources.Load<GameObject>(path);
                foundObjects = obj != null ? new GameObject[] { obj } : new GameObject[0];
            }

            if (foundObjects.Length == 0) {
                return new GameObject[0]; // Return an empty array if no object is found
            }

            // If index is provided, return the object at that index (support negative indices)
            if (index.HasValue) {
                int adjustedIndex = index.Value < 0 ? foundObjects.Length + index.Value : index.Value;
                return adjustedIndex >= 0 && adjustedIndex < foundObjects.Length
                    ? new GameObject[] { foundObjects[adjustedIndex] }
                    : new GameObject[] { foundObjects[0] }; // Return the first element if index is out of bounds
            }

            // Return the full list if no index is provided
            return foundObjects;
        }

        public static object FindAssetBundleObject(string path) {
            // Split the path by '/'
            string[] pathParts = path.Split('/');

            if (pathParts.Length < 2) {
                Debug.LogError("Invalid path format. Must be <bundle_name_or_relpath>/<asset_name>");
                return null;
            }

            string assetName = pathParts[pathParts.Length - 1];
            string bundlePath = string.Join("/", pathParts, 0, pathParts.Length - 1);

            // Try to load the asset bundle
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundlePath));

            if (assetBundle == null) {
                Debug.LogError("AssetBundle not found or failed to load: " + bundlePath);
                return null;
            }

            // Try to load the asset from the AssetBundle
            object asset = assetBundle.LoadAsset(assetName);

            if (asset == null) {
                Debug.LogError("Asset not found in the bundle: " + assetName);
            }

            assetBundle.Unload(false); // Optionally unload the bundle after loading the asset

            return asset;
        }

        public static object GetProperty(GameObject gameObject, string propertyPath) {
            string[] pathParts = propertyPath.Split('.');
            object currentObject = gameObject;

            foreach (var part in pathParts) {
                if (currentObject == null) return null;

                // Check if part is an Event (e.g., "[SomeEvent]")
                if (part.StartsWith("[") && part.EndsWith("]")) {
                    string eventName = part.Substring(1, part.Length - 2);
                    if (currentObject is Component component) {
                        return GetEventSubscribers(component, component.GetType().GetEvent(eventName));
                    }
                    return null;
                }

                // Check if part is a Component (e.g., "(Rigidbody)")
                if (part.StartsWith("(") && part.EndsWith(")")) {
                    string componentName = part.Substring(1, part.Length - 2);
                    if (currentObject is GameObject obj) {
                        currentObject = GetComponentByName(obj, componentName);
                    } else {
                        PropertyInfo property = currentObject.GetType().GetProperty(componentName);
                        currentObject = property?.GetValue(currentObject);
                    }
                    if (currentObject == null) return null;
                } else {
                    // Regular property or field
                    PropertyInfo property = currentObject.GetType().GetProperty(part);
                    if (property != null) {
                        currentObject = property.GetValue(currentObject);
                    } else {
                        FieldInfo field = currentObject.GetType().GetField(part);
                        if (field != null) {
                            currentObject = field.GetValue(currentObject);
                        } else {
                            return null;
                        }
                    }
                }
            }

            return currentObject;
        }

#nullable enable
        public object? Resolve() {
            if (isInstantiation) {
                if (type == "object") {
                    // If instantiating and type is object, check if the object exists in the scene and return a new instance of the object else return null. If unor data apply it before returning the object by calling the ParseAndApply method.
                    GameObject[] matches = FindSceneObjects(reference, index);
                    if (matches.Length == 0) {
                        return null;
                    }

                    List<GameObject> toReturn = new List<GameObject>();
                    foreach (GameObject match in matches) {
                        GameObject gameObject = GameObject.Instantiate(match);

                        if (unor != null) {
                            gameObject = ParseAndApply(gameObject, unor);
                        }

                        toReturn.Add(gameObject);

                    }

                    if (toReturn.Count == 1) {
                        return toReturn[0];
                    }

                    return toReturn;

                } else if (type == "resource") {
                    // If instantiating and type is resource, check if the resource exists and return a new instance of the resource else return null. If unor data apply it before returning the object by calling the ParseAndApply method.
                    GameObject[] matches = FindResourceObjects(reference, index);
                    if (matches.Length == 0) {
                        return null;
                    }

                    List<GameObject> toReturn = new List<GameObject>();
                    foreach (GameObject match in matches) {
                        GameObject gameObject = GameObject.Instantiate(match);

                        if (unor != null) {
                            gameObject = ParseAndApply(gameObject, unor);
                        }

                        toReturn.Add(gameObject);
                    }

                    if (toReturn.Count == 1) {
                        return toReturn[0];
                    }

                    return toReturn;

                } else if (type == "asset_bundle") {
                    // If instantiating and type is asset_bundle, check if the asset_bundle exists and return a new instance of the asset_bundle else return null. If unor data apply it before returning the object by calling the ParseAndApply method.
                    object asset = FindAssetBundleObject(reference);
                    if (asset == null) {
                        return null;
                    }
                    GameObject gameObject = GameObject.Instantiate(asset as GameObject);

                    if (unor != null) {
                        gameObject = ParseAndApply(gameObject, unor);
                    }

                    return gameObject;
                }
            } else {
                if (type == "object") {
                    // If not instantiating and type is object, check if the object exists in the scene and return it else return null. If properties are set get them from the object and return the value if the properties exists else return null.
                    GameObject[] matches = FindSceneObjects(reference, index);
                    if (matches.Length == 0) {
                        return null;
                    }

                    List<object> toReturn = new List<object>();
                    foreach (GameObject match in matches) {
                        if (propertyPath != null) {
                            object propertyValue = GetProperty(match, String.Join(".", propertyPath));
                            if (propertyValue != null) {
                                toReturn.Add(propertyValue);
                            }
                        } else {
                            toReturn.Add(match);
                        }
                    }

                    if (toReturn.Count == 1) {
                        return toReturn[0];
                    }

                    return toReturn;

                } else if (type == "resource") {
                    // If not instantiating and type is resource, check if the resource exists and return it else return null. If properties are set get them from the resource and return the value if the properties exists else return null.
                    GameObject[] matches = FindResourceObjects(reference, index);
                    if (matches.Length == 0) {
                        return null;
                    }

                    List<object> toReturn = new List<object>();
                    foreach (GameObject match in matches) {
                        if (propertyPath != null) {
                            object propertyValue = GetProperty(match, String.Join(".", propertyPath));
                            if (propertyValue != null) {
                                toReturn.Add(propertyValue);
                            }
                        } else {
                            toReturn.Add(match);
                        }
                    }

                    if (toReturn.Count == 1) {
                        return toReturn[0];
                    }

                    return toReturn;

                } else if (type == "asset_bundle") {
                    // If not instantiating and type is asset_bundle, check if the asset_bundle exists and return it else return null. If properties are set get them from the asset_bundle and return the value if the properties exists else return null.
                    object asset = FindAssetBundleObject(reference);
                    if (asset == null) {
                        return null;
                    }

                    if (propertyPath != null) {
                        object propertyValue = GetProperty(asset as GameObject, String.Join(".", propertyPath));
                        if (propertyValue != null) {
                            return propertyValue;
                        }
                    }

                    return asset;
                } else if (type == "tag") {
                    // If not instantiating and type is tag, check if the tag exists and return the object/objects else return null. If properties are set get them from the object and return the value if the properties exists else return null.
                    GameObject[] matches = GameObject.FindGameObjectsWithTag(reference);
                    if (matches.Length == 0) {
                        return null;
                    }
                    List<object> toReturn = new List<object>();
                    foreach (GameObject match in matches) {
                        if (propertyPath != null) {
                            object propertyValue = GetProperty(match, String.Join(".", propertyPath));
                            if (propertyValue != null) {
                                toReturn.Add(propertyValue);
                            }
                        } else {
                            toReturn.Add(match);
                        }
                    }
                    if (toReturn.Count == 1) {
                        return toReturn[0];
                    }
                    return toReturn;
                }
            }
            return null;
        }
#nullable disable
    }

    public static GameObject ParseAndApply(GameObject gameObject, string property) {
        property = property.Trim();
        if (property == "") {
            return gameObject;
        }
        if (!property.StartsWith("{") || !property.EndsWith("}")) {
            Debug.LogError("Invalid U-NOR format: must be an object with key/value pairs.");
            return gameObject;
        }

        Dictionary<string, object> parsedProps;
        try {
            // Create a parser instance and parse the root value.
            Parser parser = new Parser(property);
            object root = parser.ParseValue();
            if (!(root is Dictionary<string, object> dict)) {
                Debug.LogError("U-NOR root must be an object.");
                return gameObject;
            }
            parsedProps = dict;
        }
        catch (Exception ex) {
            Debug.LogError("Error parsing U-NOR data: " + ex.Message);
            return gameObject;
        }

        // For each key-value pair in the parsed dictionary, attempt to apply it.
        foreach (var kvp in parsedProps) {
            ApplyProperty(gameObject, kvp.Key, kvp.Value);
        }

        return gameObject;
    }

    #region Parsing Helpers

    // A small helper class to represent a U-NOR object reference token.
    private class UNORReference {
        public string FullReference;
        public UNORReference(string fullReference) {
            FullReference = fullReference;
        }
    }

    // A simple recursive descent parser for U-NOR (supports JSON types plus inf/-inf and U-NOR object-references).
    private class Parser {
        private readonly string input;
        private int pos;

        public Parser(string input) {
            this.input = input;
            pos = 0;
        }

        public object ParseValue() {
            SkipWhitespace();
            if (pos >= input.Length)
                throw new Exception("Unexpected end of input.");

            char c = input[pos];
            if (c == '{')
                return ParseObject();
            if (c == '[')
                return ParseArray();
            if (c == '"')
                return ParseString();
            if (c == '@' || c == '<' || c == '#')
                return ParseUnorReference();
            // Otherwise, parse a literal (number, true, false, null, inf, etc.)
            return ParseLiteral();
        }

        public Dictionary<string, object> ParseObject() {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            Expect('{');
            SkipWhitespace();
            if (Peek() == '}') {
                Next(); // consume '}'
                return obj;
            }
            while (true) {
                SkipWhitespace();
                // Assume keys are quoted strings.
                string key = ParseString();
                SkipWhitespace();
                Expect(':');
                SkipWhitespace();
                object value = ParseValue();
                obj[key] = value;
                SkipWhitespace();
                char c = Peek();
                if (c == ',') {
                    Next();
                    continue;
                } else if (c == '}') {
                    Next();
                    break;
                } else {
                    throw new Exception("Expected ',' or '}' in object");
                }
            }
            return obj;
        }

        public List<object> ParseArray() {
            List<object> list = new List<object>();
            Expect('[');
            SkipWhitespace();
            if (Peek() == ']') {
                Next();
                return list;
            }
            while (true) {
                SkipWhitespace();
                object value = ParseValue();
                list.Add(value);
                SkipWhitespace();
                char c = Peek();
                if (c == ',') {
                    Next();
                    continue;
                } else if (c == ']') {
                    Next();
                    break;
                } else {
                    throw new Exception("Expected ',' or ']' in array");
                }
            }
            return list;
        }

        public string ParseString() {
            Expect('"');
            StringBuilder sb = new StringBuilder();
            while (pos < input.Length) {
                char c = Next();
                if (c == '"')
                    break;
                if (c == '\\') {
                    if (pos < input.Length) {
                        char esc = Next();
                        switch (esc) {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            default: sb.Append(esc); break;
                        }
                    }
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public object ParseLiteral() {
            int start = pos;
            while (pos < input.Length && !char.IsWhiteSpace(input[pos]) && input[pos] != ',' && input[pos] != '}' && input[pos] != ']') {
                pos++;
            }
            string token = input.Substring(start, pos - start);
            if (token == "null") return null;
            if (token == "true") return true;
            if (token == "false") return false;
            if (token == "inf") return float.PositiveInfinity;
            if (token == "-inf") return float.NegativeInfinity;
            // Try parsing as a number (use invariant culture)
            if (double.TryParse(token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double num))
                return num;
            return token; // Fallback: return as string.
        }

        // Parses a U-NOR object reference. It handles both instantiations (starting with '@') and normal references.
        public object ParseUnorReference() {
            int start = pos;
            char firstChar = input[pos];
            bool isInstantiation = false;

            if (firstChar == '@') {
                isInstantiation = true;
                Next();
            }

            if (Peek() == '<' || firstChar == '#') {
                if (Peek() == '<')
                    Next(); // Consume '<'
                else if (firstChar == '#') {
                    start++; // Consume '#'
                }

                while (pos < input.Length && input[pos] != '>' && input[pos] != ',' && input[pos] != '}' && input[pos] != ']')
                    pos++;

                string reference = input.Substring(start, pos - start);

                if (Peek() == '>')
                    Next(); // Consume '>'

                // Check for additional U-NOR data (instantiation properties) following immediately if present.
                string unorData = "";
                SkipWhitespace();
                if (pos < input.Length && input[pos] == '{') {
                    int objStart = pos;
                    int braceCount = 0;
                    while (pos < input.Length) {
                        if (input[pos] == '{')
                            braceCount++;
                        if (input[pos] == '}') {
                            braceCount--;
                            if (braceCount == 0) {
                                pos++; // include the final brace
                                break;
                            }
                        }
                        pos++;
                    }
                    unorData = input.Substring(objStart, pos - objStart);
                }
                string fullRef = (isInstantiation ? "@" : "") + (firstChar == '#' ? "#" : "<") + reference + (firstChar == '#' ? "" : ">") + unorData;
                return new UNORReference(fullRef);
            } else {
                pos = start;  // Reset position if it's not a valid UNOR reference
                return ParseLiteral(); // Revert to parsing a literal
            }
        }


        private void SkipWhitespace() {
            while (pos < input.Length && char.IsWhiteSpace(input[pos]))
                pos++;
        }

        private char Peek() {
            return pos < input.Length ? input[pos] : '\0';
        }


        private char Next() {
            return input[pos++];
        }

        private void Expect(char expected) {
            SkipWhitespace();
            if (pos >= input.Length || input[pos] != expected)
                throw new Exception("Expected '" + expected + "' at position " + pos);
            pos++;
        }
    }

    #endregion

    #region Property Application Helpers

    // Recursively apply a key (possibly a nested keypath) with its value onto the target object.
    private static void ApplyProperty(object target, string key, object value) {
        string[] segments = key.Split('.');
        object current = target;

        for (int i = 0; i < segments.Length - 1; i++) {
            string segment = segments[i];

            // Handle component reference (e.g., "(SpriteRenderer)")
            if (segment.StartsWith("(") && segment.EndsWith(")")) {
                string compName = segment.Substring(1, segment.Length - 2);
                if (current is GameObject go) {
                    Type compType = Type.GetType(compName) ?? Type.GetType("UnityEngine." + compName + ", UnityEngine");
                    if (compType == null) return;
                    Component comp = go.GetComponent(compType);
                    if (comp == null) return;
                    current = comp;
                } else return;
            } else {
                // Regular property navigation
                Type type = current.GetType();
                var prop = type.GetProperty(segment);
                if (prop != null) {
                    current = prop.GetValue(current);
                } else {
                    var field = type.GetField(segment);
                    if (field != null) {
                        current = field.GetValue(current);
                    } else return;
                }
            }
        }

        string lastSegment = segments.Last();
        Type currentType = current.GetType();
        var finalProp = currentType.GetProperty(lastSegment);

        if (finalProp != null && finalProp.CanWrite) {
            object finalValue = value;

            // If the value is a UNORReference, resolve the object reference
            if (value is UNORReference unorReference) {
                UNOR_ObjectReference objRef = new UNOR_ObjectReference(
                    unorReference.FullReference
                );
                finalValue = objRef.Resolve();
            }

            // Special handling for color conversion
            if (current is SpriteRenderer && lastSegment == "color" && value is string strVal && strVal.StartsWith("$color:")) {
                finalValue = ParseColorFromString(strVal);
            }

            try {
                finalValue = ConvertValue(finalValue, finalProp.PropertyType);
                finalProp.SetValue(current, finalValue);
            }
            catch (Exception e) {
                Debug.LogError($"Error setting property '{key}': {e.Message}");
            }
        } else {
            // Attempt to set the field if the property is not found
            var finalField = currentType.GetField(lastSegment);
            if (finalField != null) {
                object finalValue = value;

                // If the value is a UNORReference, resolve the object reference
                if (value is UNORReference unorReference) {
                    UNOR_ObjectReference objRef = new UNOR_ObjectReference(
                        unorReference.FullReference
                    );
                    finalValue = objRef.Resolve();
                }

                try {
                    finalValue = ConvertValue(finalValue, finalField.FieldType);
                    finalField.SetValue(current, finalValue);
                }
                catch (Exception e) {
                    Debug.LogError($"Error setting field '{key}': {e.Message}");
                }
            } else {
                Debug.LogWarning($"Property or field '{lastSegment}' not found on object of type '{currentType.Name}'");
            }
        }
    }

    private static object ConvertValue(object value, Type targetType) {
        if (value == null) return null;

        if (targetType == typeof(Color) && value is string strVal) {
            return ParseColorFromString(strVal);
        }

        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        try {
            return Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch {
            return value;
        }
    }

    // Parses a string in the format "$color:<color>" into a Unity Color
    private static Color ParseColorFromString(string value) {
        if (value.StartsWith("$color:")) {
            string colorString = value.Substring(7).Trim(); // Extract color name or hex value

            // Try parsing as a named color
            if (ColorUtility.TryParseHtmlString(colorString, out Color parsedColor)) {
                return parsedColor;
            }

            // Try parsing as a standard named color
            switch (colorString.ToLower()) {
                case "red": return Color.red;
                case "green": return Color.green;
                case "blue": return Color.blue;
                case "black": return Color.black;
                case "white": return Color.white;
                case "yellow": return Color.yellow;
                case "cyan": return Color.cyan;
                case "magenta": return Color.magenta;
                case "gray": return Color.gray;
                case "clear": return Color.clear;
                default:
                    Debug.LogWarning($"Unknown color '{colorString}' in U-NOR data.");
                    return Color.white; // Default to white if the color is unknown
            }
        }

        return Color.white; // Default fallback color
    }

    #endregion
}